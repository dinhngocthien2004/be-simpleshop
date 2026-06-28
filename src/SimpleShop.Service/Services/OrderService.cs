using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Dtos;

namespace SimpleShop.Service.Services;

public class OrderService : IOrderService
{
    private readonly ICartRepository _carts;
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;

    public OrderService(ICartRepository carts, IOrderRepository orders, IProductRepository products)
    {
        _carts = carts;
        _orders = orders;
        _products = products;
    }

    public async Task<OrderDto> CheckoutAsync(int userId)
    {
        var cart = await _carts.GetOrCreateByUserIdAsync(userId);
        if (!cart.Items.Any())
            throw new InvalidOperationException("Cart is empty.");

        foreach (var item in cart.Items)
        {
            if (!item.Product.IsActive || !item.Product.Category.IsActive)
                throw new InvalidOperationException($"Product '{item.Product.Name}' is no longer available.");
            if (item.Quantity > item.Product.StockQuantity)
                throw new InvalidOperationException($"Product '{item.Product.Name}' has only {item.Product.StockQuantity} item(s) in stock.");
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = "Placed",
            Items = cart.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ImageUrl = i.Product.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price
            }).ToList()
        };
        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        foreach (var item in cart.Items)
        {
            item.Product.StockQuantity -= item.Quantity;
            item.Product.ModifiedAt = DateTime.UtcNow;
            _products.Update(item.Product);
        }

        await _orders.AddAsync(order);
        _carts.Clear(cart);
        await _orders.SaveChangesAsync();

        return ToDto(order);
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        => (await _orders.GetByUserIdAsync(userId)).Select(ToDto).ToList();

    public async Task<OrderDto> GetByIdAsync(int orderId, int userId, bool isAdmin)
    {
        var order = await _orders.GetByIdForUserAsync(orderId, userId, isAdmin) ?? throw new KeyNotFoundException("Order not found.");
        return ToDto(order);
    }

    private static OrderDto ToDto(Order order)
    {
        var items = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.ProductId,
            i.ProductName,
            i.ImageUrl,
            i.Quantity,
            i.UnitPrice,
            i.Quantity * i.UnitPrice)).ToList();
        return new OrderDto(order.Id, order.OrderDate, order.TotalAmount, order.Status, items);
    }
}

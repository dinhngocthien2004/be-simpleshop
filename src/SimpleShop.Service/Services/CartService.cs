using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Dtos;

namespace SimpleShop.Service.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;

    public CartService(ICartRepository carts, IProductRepository products)
    {
        _carts = carts;
        _products = products;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await _carts.GetOrCreateByUserIdAsync(userId);
        return ToDto(cart);
    }

    public async Task<CartDto> AddAsync(int userId, AddToCartRequest request)
    {
        var product = await _products.GetByIdAsync(request.ProductId) ?? throw new KeyNotFoundException("Product not found.");
        if (!product.IsActive || product.Category?.IsActive != true)
            throw new InvalidOperationException("Product is not available.");

        var cart = await _carts.GetOrCreateByUserIdAsync(userId);
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        var targetQuantity = request.Quantity + (existing?.Quantity ?? 0);
        if (targetQuantity > product.StockQuantity)
            throw new InvalidOperationException($"Only {product.StockQuantity} item(s) are available in stock.");

        if (existing == null)
        {
            await _carts.AddItemAsync(new CartItem { CartId = cart.Id, ProductId = product.Id, Quantity = request.Quantity });
        }
        else
        {
            existing.Quantity = targetQuantity;
        }
        await _carts.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
    {
        var item = await _carts.GetItemAsync(userId, cartItemId) ?? throw new KeyNotFoundException("Cart item not found.");
        if (request.Quantity > item.Product.StockQuantity)
            throw new InvalidOperationException($"Only {item.Product.StockQuantity} item(s) are available in stock.");
        item.Quantity = request.Quantity;
        await _carts.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartDto> RemoveItemAsync(int userId, int cartItemId)
    {
        var item = await _carts.GetItemAsync(userId, cartItemId) ?? throw new KeyNotFoundException("Cart item not found.");
        _carts.RemoveItem(item);
        await _carts.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartDto> ClearAsync(int userId)
    {
        var cart = await _carts.GetOrCreateByUserIdAsync(userId);
        _carts.Clear(cart);
        await _carts.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    private static CartDto ToDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemDto(
            i.Id,
            i.ProductId,
            i.Product.Name,
            i.Product.ImageUrl,
            i.Product.Price,
            i.Quantity,
            i.Product.Price * i.Quantity,
            i.Product.StockQuantity)).ToList();
        return new CartDto(cart.Id, items, items.Sum(i => i.LineSubtotal));
    }
}

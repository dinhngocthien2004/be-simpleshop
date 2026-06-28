using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Service.Dtos;

public record CartItemDto(int Id, int ProductId, string ProductName, string? ImageUrl, decimal UnitPrice, int Quantity, decimal LineSubtotal, int StockQuantity);
public record CartDto(int Id, List<CartItemDto> Items, decimal Total);

public class AddToCartRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, 100000)]
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    [Range(1, 100000)]
    public int Quantity { get; set; }
}

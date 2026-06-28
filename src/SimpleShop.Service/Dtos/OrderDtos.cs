namespace SimpleShop.Service.Dtos;

public record OrderItemDto(int Id, int ProductId, string ProductName, string? ImageUrl, int Quantity, decimal UnitPrice, decimal LineTotal);
public record OrderDto(int Id, DateTime OrderDate, decimal TotalAmount, string Status, List<OrderItemDto> Items);

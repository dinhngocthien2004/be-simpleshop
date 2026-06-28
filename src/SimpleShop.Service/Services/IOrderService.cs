using SimpleShop.Service.Dtos;
namespace SimpleShop.Service.Services;
public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(int userId);
    Task<List<OrderDto>> GetMyOrdersAsync(int userId);
    Task<OrderDto> GetByIdAsync(int orderId, int userId, bool isAdmin);
}

using SimpleShop.Service.Dtos;
namespace SimpleShop.Service.Services;
public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartDto> AddAsync(int userId, AddToCartRequest request);
    Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
    Task<CartDto> RemoveItemAsync(int userId, int cartItemId);
    Task<CartDto> ClearAsync(int userId);
}

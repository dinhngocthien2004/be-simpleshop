using SimpleShop.Repo.Models;
namespace SimpleShop.Repo.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart> GetOrCreateByUserIdAsync(int userId);
    Task<CartItem?> GetItemAsync(int userId, int cartItemId);
    Task AddItemAsync(CartItem item);
    void RemoveItem(CartItem item);
    void Clear(Cart cart);
    Task SaveChangesAsync();
}

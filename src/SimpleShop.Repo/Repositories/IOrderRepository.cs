using SimpleShop.Repo.Models;
namespace SimpleShop.Repo.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task<Order?> GetByIdForUserAsync(int orderId, int userId, bool isAdmin = false);
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}

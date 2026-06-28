using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SimpleShopDbContext _context;
    public OrderRepository(SimpleShopDbContext context) => _context = context;

    public Task<List<Order>> GetByUserIdAsync(int userId) => _context.Orders
        .Include(o => o.Items)
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();

    public Task<Order?> GetByIdForUserAsync(int orderId, int userId, bool isAdmin = false) => _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == orderId && (isAdmin || o.UserId == userId));

    public Task AddAsync(Order order) => _context.Orders.AddAsync(order).AsTask();
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

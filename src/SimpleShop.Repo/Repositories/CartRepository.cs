using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class CartRepository : ICartRepository
{
    private readonly SimpleShopDbContext _context;
    public CartRepository(SimpleShopDbContext context) => _context = context;

    public Task<Cart?> GetByUserIdAsync(int userId) => _context.Carts
        .Include(c => c.Items)
            .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> GetOrCreateByUserIdAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart != null) return cart;

        cart = new Cart { UserId = userId };
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();
        return cart;
    }

    public Task<CartItem?> GetItemAsync(int userId, int cartItemId) => _context.CartItems
        .Include(ci => ci.Cart)
        .Include(ci => ci.Product)
        .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

    public Task AddItemAsync(CartItem item) => _context.CartItems.AddAsync(item).AsTask();
    public void RemoveItem(CartItem item) => _context.CartItems.Remove(item);
    public void Clear(Cart cart) => _context.CartItems.RemoveRange(cart.Items);
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

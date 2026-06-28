using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SimpleShopDbContext _context;
    public CategoryRepository(SimpleShopDbContext context) => _context = context;

    public Task<List<Category>> GetActiveAsync() => _context.Categories
        .Include(c => c.Owner)
        .Where(c => c.IsActive)
        .OrderBy(c => c.Name)
        .ToListAsync();

    public Task<List<Category>> GetAllAsync() => _context.Categories
        .Include(c => c.Owner)
        .OrderBy(c => c.Name)
        .ToListAsync();

    public Task<List<Category>> GetMineAsync(int userId, bool isAdmin) => _context.Categories
        .Include(c => c.Owner)
        .Include(c => c.Products)
        .Where(c => isAdmin || c.OwnerId == userId)
        .OrderBy(c => c.Name)
        .ToListAsync();

    public Task<Category?> GetByIdAsync(int id) => _context.Categories
        .Include(c => c.Owner)
        .Include(c => c.Products)
        .FirstOrDefaultAsync(c => c.Id == id);

    public Task<List<Category>> SearchAsync(string? name, bool includeInactive = false)
    {
        var query = _context.Categories.Include(c => c.Owner).AsQueryable();
        if (!includeInactive) query = query.Where(c => c.IsActive);
        if (!string.IsNullOrWhiteSpace(name))
        {
            var kw = name.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(kw));
        }
        return query.OrderBy(c => c.Name).ToListAsync();
    }

    public Task<bool> HasProductsAsync(int categoryId) => _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    public Task AddAsync(Category category) => _context.Categories.AddAsync(category).AsTask();
    public void Update(Category category) => _context.Categories.Update(category);
    public void Delete(Category category) => _context.Categories.Remove(category);
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

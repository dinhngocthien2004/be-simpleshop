using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SimpleShopDbContext _context;
    public ProductRepository(SimpleShopDbContext context) => _context = context;

    private IQueryable<Product> WithIncludes() => _context.Products
        .Include(p => p.Category)
        .Include(p => p.Owner);

    public Task<List<Product>> GetActiveAsync() => WithIncludes()
        .Where(p => p.IsActive && p.Category.IsActive)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    public Task<List<Product>> GetAllAsync() => WithIncludes()
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    public Task<List<Product>> GetMineAsync(int userId, bool isAdmin) => WithIncludes()
        .Where(p => isAdmin || p.OwnerId == userId)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    public Task<Product?> GetByIdAsync(int id) => WithIncludes().FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Product>> GetByCategoryAsync(int categoryId) => WithIncludes()
        .Where(p => p.CategoryId == categoryId && p.IsActive && p.Category.IsActive)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    public Task<List<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId, bool includeInactive = false)
    {
        var query = WithIncludes();
        if (!includeInactive) query = query.Where(p => p.IsActive && p.Category.IsActive);
        if (!string.IsNullOrWhiteSpace(name))
        {
            var kw = name.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(kw) || (p.Description != null && p.Description.ToLower().Contains(kw)));
        }
        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
        if (categoryId.HasValue) query = query.Where(p => p.CategoryId == categoryId.Value);
        return query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public Task AddAsync(Product product) => _context.Products.AddAsync(product).AsTask();
    public void Update(Product product) => _context.Products.Update(product);
    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

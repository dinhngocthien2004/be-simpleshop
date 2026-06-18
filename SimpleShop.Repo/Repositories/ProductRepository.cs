using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SimpleShopDbContext _context;

    public ProductRepository(SimpleShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllActiveAsync()
        => await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Category != null && p.Category.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        => await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.CategoryID == categoryId && p.Category != null && p.Category.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId)
    {
        var query = _context.Products.Include(p => p.Category).Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.ProductName.ToLower().Contains(name.ToLower()));

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryID == categoryId.Value);

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.ModifiedDate = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> SoftDeleteAsync(Product product)
    {
        product.IsActive = false;
        product.ModifiedDate = DateTime.UtcNow;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return product;
    }
}

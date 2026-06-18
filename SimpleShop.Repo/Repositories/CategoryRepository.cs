using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SimpleShopDbContext _context;

    public CategoryRepository(SimpleShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllActiveAsync()
        => await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.CategoryName).ToListAsync();

    public async Task<IEnumerable<Category>> GetAllAsync()
        => await _context.Categories.OrderBy(c => c.CategoryID).ToListAsync();

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);

    public async Task<IEnumerable<Category>> SearchByNameAsync(string keyword)
        => await _context.Categories
            .Where(c => c.CategoryName.ToLower().Contains(keyword.ToLower()))
            .OrderBy(c => c.CategoryName)
            .ToListAsync();

    public async Task<Category> CreateAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
        => await _context.Products.AnyAsync(p => p.CategoryID == categoryId);
}

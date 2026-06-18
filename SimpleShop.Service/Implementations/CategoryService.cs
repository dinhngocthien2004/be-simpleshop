using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.Service.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Category>> GetAllActiveAsync() => _repo.GetAllActiveAsync();
    public Task<IEnumerable<Category>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Category?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<Category>> SearchByNameAsync(string keyword) => _repo.SearchByNameAsync(keyword);

    public Task<Category> CreateAsync(Category category) => _repo.CreateAsync(category);

    public async Task<Category?> UpdateAsync(int id, Category updated)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        existing.CategoryName = updated.CategoryName;
        existing.CategoryDescription = updated.CategoryDescription;
        existing.IsActive = updated.IsActive;

        return await _repo.UpdateAsync(existing);
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category == null)
            return (false, "Category not found.");

        var hasProducts = await _repo.HasProductsAsync(id);
        if (hasProducts)
            return (false, "Cannot delete category because it has products linked to it.");

        await _repo.DeleteAsync(category);
        return (true, "Category deleted successfully.");
    }
}

using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Dtos;

namespace SimpleShop.Service.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    public CategoryService(ICategoryRepository categories) => _categories = categories;

    public async Task<List<CategoryDto>> GetActiveAsync() => (await _categories.GetActiveAsync()).Select(ToDto).ToList();
    public async Task<List<CategoryDto>> GetAllAsync() => (await _categories.GetAllAsync()).Select(ToDto).ToList();
    public async Task<List<CategoryDto>> GetMineAsync(int userId, bool isAdmin) => (await _categories.GetMineAsync(userId, isAdmin)).Select(ToDto).ToList();

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found.");
        return ToDto(category);
    }

    public async Task<List<CategoryDto>> SearchAsync(string? name, bool includeInactive = false)
        => (await _categories.SearchAsync(name, includeInactive)).Select(ToDto).ToList();

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, int userId)
    {
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            OwnerId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _categories.AddAsync(category);
        await _categories.SaveChangesAsync();
        return await GetByIdAsync(category.Id);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, int userId, bool isAdmin)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found.");
        EnsureCanManage(category, userId, isAdmin);
        category.Name = request.Name.Trim();
        category.Description = request.Description.Trim();
        category.IsActive = request.IsActive;
        category.ModifiedAt = DateTime.UtcNow;
        _categories.Update(category);
        await _categories.SaveChangesAsync();
        return ToDto(category);
    }

    public async Task DeleteAsync(int id, int userId, bool isAdmin)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new KeyNotFoundException("Category not found.");
        EnsureCanManage(category, userId, isAdmin);
        if (await _categories.HasProductsAsync(id))
            throw new InvalidOperationException("Category cannot be deleted because products are linked to it.");
        _categories.Delete(category);
        await _categories.SaveChangesAsync();
    }

    private static void EnsureCanManage(Category category, int userId, bool isAdmin)
    {
        if (!isAdmin && category.OwnerId != userId)
            throw new UnauthorizedAccessException("You can manage only your own categories.");
    }

    private static CategoryDto ToDto(Category c) => new(
        c.Id,
        c.Name,
        c.Description,
        c.IsActive,
        c.OwnerId,
        c.Owner?.Name ?? string.Empty,
        c.CreatedAt,
        c.Products?.Count ?? 0);
}

using SimpleShop.Service.Dtos;
namespace SimpleShop.Service.Services;
public interface ICategoryService
{
    Task<List<CategoryDto>> GetActiveAsync();
    Task<List<CategoryDto>> GetAllAsync();
    Task<List<CategoryDto>> GetMineAsync(int userId, bool isAdmin);
    Task<CategoryDto> GetByIdAsync(int id);
    Task<List<CategoryDto>> SearchAsync(string? name, bool includeInactive = false);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, int userId);
    Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, int userId, bool isAdmin);
    Task DeleteAsync(int id, int userId, bool isAdmin);
}

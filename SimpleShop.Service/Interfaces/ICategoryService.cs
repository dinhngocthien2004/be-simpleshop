using SimpleShop.Repo.Models;

namespace SimpleShop.Service.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> SearchByNameAsync(string keyword);
    Task<Category> CreateAsync(Category category);
    Task<Category?> UpdateAsync(int id, Category category);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}

using SimpleShop.Repo.Models;
namespace SimpleShop.Repo.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetActiveAsync();
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetMineAsync(int userId, bool isAdmin);
    Task<Category?> GetByIdAsync(int id);
    Task<List<Category>> SearchAsync(string? name, bool includeInactive = false);
    Task<bool> HasProductsAsync(int categoryId);
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category);
    Task SaveChangesAsync();
}

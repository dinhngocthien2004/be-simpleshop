using SimpleShop.Repo.Models;
namespace SimpleShop.Repo.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetActiveAsync();
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetMineAsync(int userId, bool isAdmin);
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<List<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId, bool includeInactive = false);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
    Task SaveChangesAsync();
}

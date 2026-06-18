using SimpleShop.Repo.Models;

namespace SimpleShop.Repo.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllActiveAsync();
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<Product> SoftDeleteAsync(Product product);
}

using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.Service.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Product>> GetAllActiveAsync() => _repo.GetAllActiveAsync();
    public Task<IEnumerable<Product>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Product?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId) => _repo.GetByCategoryAsync(categoryId);
    public Task<IEnumerable<Product>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId)
        => _repo.SearchAsync(name, minPrice, maxPrice, categoryId);

    public Task<Product> CreateAsync(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        return _repo.CreateAsync(product);
    }

    public async Task<Product?> UpdateAsync(int id, Product updated)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        existing.ProductName = updated.ProductName;
        existing.Description = updated.Description;
        existing.Price = updated.Price;
        existing.StockQuantity = updated.StockQuantity;
        existing.ImageUrl = updated.ImageUrl;
        existing.CategoryID = updated.CategoryID;
        existing.IsActive = updated.IsActive;
        existing.ModifiedDate = DateTime.UtcNow;

        return await _repo.UpdateAsync(existing);
    }

    public async Task<(bool Success, string Message)> SoftDeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null)
            return (false, "Product not found.");

        await _repo.SoftDeleteAsync(product);
        return (true, "Product deactivated successfully.");
    }
}

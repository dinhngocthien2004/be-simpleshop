using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Dtos;

namespace SimpleShop.Service.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public async Task<List<ProductDto>> GetActiveAsync() => (await _products.GetActiveAsync()).Select(ToDto).ToList();
    public async Task<List<ProductDto>> GetAllAsync() => (await _products.GetAllAsync()).Select(ToDto).ToList();
    public async Task<List<ProductDto>> GetMineAsync(int userId, bool isAdmin) => (await _products.GetMineAsync(userId, isAdmin)).Select(ToDto).ToList();

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product not found.");
        return ToDto(product);
    }

    public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId) => (await _products.GetByCategoryAsync(categoryId)).Select(ToDto).ToList();

    public async Task<List<ProductDto>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId, bool includeInactive = false)
        => (await _products.SearchAsync(name, minPrice, maxPrice, categoryId, includeInactive)).Select(ToDto).ToList();

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, int userId, bool isAdmin)
    {
        var category = await _categories.GetByIdAsync(request.CategoryId) ?? throw new KeyNotFoundException("Category not found.");
        if (!isAdmin && category.OwnerId != userId)
            throw new UnauthorizedAccessException("You can assign products only to your own categories.");

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            CategoryId = request.CategoryId,
            OwnerId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _products.AddAsync(product);
        await _products.SaveChangesAsync();
        return await GetByIdAsync(product.Id);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request, int userId, bool isAdmin)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product not found.");
        EnsureCanManage(product, userId, isAdmin);
        var category = await _categories.GetByIdAsync(request.CategoryId) ?? throw new KeyNotFoundException("Category not found.");
        if (!isAdmin && category.OwnerId != userId)
            throw new UnauthorizedAccessException("You can assign products only to your own categories.");

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.ModifiedAt = DateTime.UtcNow;
        _products.Update(product);
        await _products.SaveChangesAsync();
        return await GetByIdAsync(product.Id);
    }

    public async Task SoftDeleteAsync(int id, int userId, bool isAdmin)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new KeyNotFoundException("Product not found.");
        EnsureCanManage(product, userId, isAdmin);
        product.IsActive = false;
        product.ModifiedAt = DateTime.UtcNow;
        _products.Delete(product);
        await _products.SaveChangesAsync();
    }

    private static void EnsureCanManage(Product product, int userId, bool isAdmin)
    {
        if (!isAdmin && product.OwnerId != userId)
            throw new UnauthorizedAccessException("You can manage only your own products.");
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id,
        p.Name,
        p.Description,
        p.Price,
        p.StockQuantity,
        p.ImageUrl,
        p.IsActive,
        p.CategoryId,
        p.Category?.Name ?? string.Empty,
        p.OwnerId,
        p.Owner?.Name ?? string.Empty,
        p.CreatedAt,
        p.ModifiedAt);
}

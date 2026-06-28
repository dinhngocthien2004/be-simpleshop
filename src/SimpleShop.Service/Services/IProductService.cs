using SimpleShop.Service.Dtos;
namespace SimpleShop.Service.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetActiveAsync();
    Task<List<ProductDto>> GetAllAsync();
    Task<List<ProductDto>> GetMineAsync(int userId, bool isAdmin);
    Task<ProductDto> GetByIdAsync(int id);
    Task<List<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<List<ProductDto>> SearchAsync(string? name, decimal? minPrice, decimal? maxPrice, int? categoryId, bool includeInactive = false);
    Task<ProductDto> CreateAsync(CreateProductRequest request, int userId, bool isAdmin);
    Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request, int userId, bool isAdmin);
    Task SoftDeleteAsync(int id, int userId, bool isAdmin);

}

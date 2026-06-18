using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.DTOs;
using SimpleShop.Repo.Models;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    /// <summary>Get all active products (public storefront).</summary>
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var products = await _service.GetAllActiveAsync();
        return Ok(products.Select(MapToDto));
    }

    /// <summary>Get all products including inactive (Admin only).</summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products.Select(MapToDto));
    }

    /// <summary>Get product by ID (public).</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound(new { message = $"Product {id} not found." });
        return Ok(MapToDto(product));
    }

    /// <summary>Get active products by category (public).</summary>
    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var products = await _service.GetByCategoryAsync(categoryId);
        return Ok(products.Select(MapToDto));
    }

    /// <summary>Search products (public). All params optional.</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? categoryId)
    {
        var results = await _service.SearchAsync(name, minPrice, maxPrice, categoryId);
        return Ok(results.Select(MapToDto));
    }

    /// <summary>Create a new product (Admin only).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var product = new Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            CategoryID = dto.CategoryID,
            IsActive = dto.IsActive
        };

        var created = await _service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.ProductID }, MapToDto(created));
    }

    /// <summary>Update an existing product (Admin only).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, new Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            CategoryID = dto.CategoryID,
            IsActive = dto.IsActive
        });

        if (updated == null) return NotFound(new { message = $"Product {id} not found." });
        return Ok(MapToDto(updated));
    }

    /// <summary>Soft-delete a product — sets IsActive = false (Admin only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.SoftDeleteAsync(id);
        if (!success) return NotFound(new { message });
        return Ok(new { message });
    }

    private static ProductResponseDto MapToDto(Product p) => new()
    {
        ProductID = p.ProductID,
        ProductName = p.ProductName,
        Description = p.Description,
        Price = p.Price,
        StockQuantity = p.StockQuantity,
        ImageUrl = p.ImageUrl,
        CategoryID = p.CategoryID,
        CategoryName = p.Category?.CategoryName,
        IsActive = p.IsActive,
        CreatedDate = p.CreatedDate,
        ModifiedDate = p.ModifiedDate
    };
}

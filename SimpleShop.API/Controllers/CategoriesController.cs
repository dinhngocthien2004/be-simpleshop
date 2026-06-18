using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.DTOs;
using SimpleShop.Repo.Models;
using SimpleShop.Service.Interfaces;

namespace SimpleShop.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    /// <summary>Get all active categories (public).</summary>
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var categories = await _service.GetAllActiveAsync();
        return Ok(categories.Select(MapToDto));
    }

    /// <summary>Get all categories including inactive (Admin only).</summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _service.GetAllAsync();
        return Ok(categories.Select(MapToDto));
    }

    /// <summary>Get category by ID (public).</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _service.GetByIdAsync(id);
        if (category == null) return NotFound(new { message = $"Category {id} not found." });
        return Ok(MapToDto(category));
    }

    /// <summary>Search categories by name (Admin only).</summary>
    [HttpGet("search")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Search keyword is required." });

        var results = await _service.SearchByNameAsync(name);
        return Ok(results.Select(MapToDto));
    }

    /// <summary>Create a new category (Admin only).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var category = new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDescription = dto.CategoryDescription,
            IsActive = dto.IsActive
        };

        var created = await _service.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = created.CategoryID }, MapToDto(created));
    }

    /// <summary>Update an existing category (Admin only).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, new Category
        {
            CategoryName = dto.CategoryName,
            CategoryDescription = dto.CategoryDescription,
            IsActive = dto.IsActive
        });

        if (updated == null) return NotFound(new { message = $"Category {id} not found." });
        return Ok(MapToDto(updated));
    }

    /// <summary>Delete a category (Admin only). Fails if products are linked.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.DeleteAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    private static CategoryResponseDto MapToDto(Category c) => new()
    {
        CategoryID = c.CategoryID,
        CategoryName = c.CategoryName,
        CategoryDescription = c.CategoryDescription,
        IsActive = c.IsActive
    };
}

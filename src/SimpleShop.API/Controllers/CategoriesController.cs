using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.Extensions;
using SimpleShop.Service.Dtos;
using SimpleShop.Service.Services;

namespace SimpleShop.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;
    public CategoriesController(ICategoryService categories) => _categories = categories;

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetActive() => Ok(await _categories.GetActiveAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<CategoryDto>>> GetAll() => Ok(await _categories.GetAllAsync());

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<CategoryDto>>> GetMine() => Ok(await _categories.GetMineAsync(User.GetUserId(), User.IsAdmin()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        try { return Ok(await _categories.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<CategoryDto>>> Search([FromQuery] string? name) => Ok(await _categories.SearchAsync(name));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request)
    {
        var category = await _categories.CreateAsync(request, User.GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryRequest request)
    {
        try { return Ok(await _categories.UpdateAsync(id, request, User.GetUserId(), User.IsAdmin())); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categories.DeleteAsync(id, User.GetUserId(), User.IsAdmin());
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
    }
}

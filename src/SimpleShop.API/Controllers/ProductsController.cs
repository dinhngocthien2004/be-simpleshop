using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.Extensions;
using SimpleShop.Service.Dtos;
using SimpleShop.Service.Services;

namespace SimpleShop.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;
    public ProductsController(IProductService products) => _products = products;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetActive() => Ok(await _products.GetActiveAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<ProductDto>>> GetAll() => Ok(await _products.GetAllAsync());

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<ProductDto>>> GetMine() => Ok(await _products.GetMineAsync(User.GetUserId(), User.IsAdmin()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        try { return Ok(await _products.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<List<ProductDto>>> GetByCategory(int categoryId) => Ok(await _products.GetByCategoryAsync(categoryId));

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductDto>>> Search([FromQuery] string? name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? categoryId)
        => Ok(await _products.SearchAsync(name, minPrice, maxPrice, categoryId));

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _products.CreateAsync(request, User.GetUserId(), User.IsAdmin());
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        try { return Ok(await _products.UpdateAsync(id, request, User.GetUserId(), User.IsAdmin())); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
    }


    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            await _products.SoftDeleteAsync(id, User.GetUserId(), User.IsAdmin());
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message }); }
    }

}

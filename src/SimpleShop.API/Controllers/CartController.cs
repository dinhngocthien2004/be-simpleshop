using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.Extensions;
using SimpleShop.Service.Dtos;
using SimpleShop.Service.Services;

namespace SimpleShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cart;
    public CartController(ICartService cart) => _cart = cart;

    [HttpGet]
    public async Task<ActionResult<CartDto>> Get() => Ok(await _cart.GetCartAsync(User.GetUserId()));

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> Add([FromBody] AddToCartRequest request)
    {
        try { return Ok(await _cart.AddAsync(User.GetUserId(), request)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<ActionResult<CartDto>> Update(int cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        try { return Ok(await _cart.UpdateItemAsync(User.GetUserId(), cartItemId, request)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<ActionResult<CartDto>> Remove(int cartItemId)
    {
        try { return Ok(await _cart.RemoveItemAsync(User.GetUserId(), cartItemId)); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear() => Ok(await _cart.ClearAsync(User.GetUserId()));
}

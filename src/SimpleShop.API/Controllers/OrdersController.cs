using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShop.API.Extensions;
using SimpleShop.Service.Dtos;
using SimpleShop.Service.Services;

namespace SimpleShop.API.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout()
    {
        try { return Ok(await _orders.CheckoutAsync(User.GetUserId())); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetMine() => Ok(await _orders.GetMyOrdersAsync(User.GetUserId()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        try { return Ok(await _orders.GetByIdAsync(id, User.GetUserId(), User.IsAdmin())); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}

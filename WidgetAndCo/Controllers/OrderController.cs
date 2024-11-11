using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet(Name = "GetOrders")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), 200)]
    public async Task<IActionResult> GetOrders()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var orders = await orderService.GetOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{orderId:guid}", Name = "GetOrder")]
    [ProducesResponseType(typeof(OrderResponseDto), 200)]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var order = await orderService.GetOrderAsync(userId, orderId);
        return Ok(order);
    }

    [HttpPost(Name = "CreateOrder")]
    [ProducesResponseType(typeof(OrderResponseDto), 200)]
    public async Task<IActionResult> CreateOrder(OrderRequestDto orderRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var order = await orderService.CreateOrderAsync(userId, orderRequest);
        return Ok(order);
    }
}
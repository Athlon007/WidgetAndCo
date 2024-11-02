using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController(IUserService userService): ControllerBase
{
    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [HttpGet(Name = "GetUsers")]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("Me", Name = "GetMe")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userService.GetUserByIdAsync(Guid.Parse(userId ?? throw new InvalidOperationException()));
        return Ok(user);
    }
}
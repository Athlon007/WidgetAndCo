using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpPost("Register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        var user = await userService.RegisterUserAsync(registerUserDto);
        return Ok(user);
    }

    [HttpPost("Login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var token = await userService.LoginUserAsync(loginUserDto);
        if (token is null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }
}
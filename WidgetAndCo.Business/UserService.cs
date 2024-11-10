using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class UserService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper) : IUserService
{
    public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        return user is null ? null : mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(Guid id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        return user is null ? null : mapper.Map<UserResponseDto>(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllUsersAsync();
        return mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task AddUserAsync(User user)
    {
        await userRepository.AddUserAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await userRepository.DeleteUserAsync(id);
    }

    public async Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
        var user = new User
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            PasswordHash = passwordHash
        };

        await userRepository.AddUserAsync(user);
        return mapper.Map<UserResponseDto>(user);
    }

    public async Task<LoginResponseDto?> LoginUserAsync(LoginUserDto loginUserDto)
    {
        var user = await userRepository.GetUserByEmailAsync(loginUserDto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.PasswordHash))
        {
            return null;
        }

        var jwtSettings = configuration.GetSection("JwtSettings");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException()));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expirationHours = int.Parse(jwtSettings["AccessTokenExpirationHours"] ?? throw new InvalidOperationException());

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        // Update last login date.
        user.LastLogin = DateTime.UtcNow;
        await userRepository.UpdateUserAsync(user);

        var responseToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResponseDto(responseToken);
    }

    public bool ValidateToken(string token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException());

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ClockSkew = TimeSpan.Zero
            }, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Guid GetUserIdFromToken(string token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException());

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var output = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        return Guid.Parse(output);
    }
}
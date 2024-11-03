namespace WidgetAndCo.Core.DTOs;

public record UserResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime LastLogin
    )
{
    public UserResponseDto(): this(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue)
    {
    }
}
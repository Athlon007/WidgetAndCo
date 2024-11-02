namespace WidgetAndCo.Core.DTOs;

public record UserResponseDto(Guid Id, string FirstName, string LastName, string Email, string Role, DateTime CreatedAt, DateTime LastLogin);
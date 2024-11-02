using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core.DTOs;

public record LoginUserDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    string Password
    );
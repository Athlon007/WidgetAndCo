using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core.DTOs;

public record RegisterUserDto(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    string Email,

    [Required(ErrorMessage = "First name is required")]
    [MaxLength(100, ErrorMessage = "First name must be at most 100 characters")]
    string FirstName,

    [Required(ErrorMessage = "Last name is required")]
    [MaxLength(100, ErrorMessage = "Last name must be at most 100 characters")]
    string LastName,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    string Password
    );
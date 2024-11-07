using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core.DTOs;

using Microsoft.AspNetCore.Http;

public record ProductRequestDto(
    [Required]
    [MaxLength(100)]
    string Name,

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    decimal Price,

    [MaxLength(500)]
    string Description,

    // must be a JPG or PNG
    IFormFile Image
    );
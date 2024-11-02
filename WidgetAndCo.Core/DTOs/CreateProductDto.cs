using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core.DTOs;

public record CreateProductDto(
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name must be at most 100 characters")]
    string Name,

    [Required(ErrorMessage = "Price is required")]
    decimal Price,

    [MaxLength(500, ErrorMessage = "Description must be at most 500 characters")]
    string Description,

    [Required(ErrorMessage = "Category is required")]
    [MaxLength(100, ErrorMessage = "Category must be at most 100 characters")]
    string Category
    );
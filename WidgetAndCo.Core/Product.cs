using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core;

public class Product
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; }

    public string ImageUrl { get; set; }
}
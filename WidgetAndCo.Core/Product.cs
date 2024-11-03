using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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

    public Product(Guid id, string name, decimal price, string description, string category, string imageUrl)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
        Category = category;
        ImageUrl = imageUrl;
    }
}
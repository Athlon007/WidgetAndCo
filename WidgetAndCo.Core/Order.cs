using System.ComponentModel.DataAnnotations;

namespace WidgetAndCo.Core;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public List<Product> Products { get; set; } = [];
    public decimal Total => Products.Sum(p => p.Price);
}
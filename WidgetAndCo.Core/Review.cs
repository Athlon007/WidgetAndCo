using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WidgetAndCo.Core;

[Index(nameof(ProductId), nameof(UserId), IsUnique = true)]
public class Review
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    public string Description { get; set; }

    [Required]
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }
}
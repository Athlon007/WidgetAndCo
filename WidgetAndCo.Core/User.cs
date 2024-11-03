using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WidgetAndCo.Core;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }
    public RoleEnum Role { get; set; } = RoleEnum.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; }

    // overwrite equals
    public override bool Equals(object? obj)
    {
        if (obj is not User user)
        {
            return false;
        }

        return Id == user.Id &&
               FirstName == user.FirstName &&
               LastName == user.LastName &&
               Email == user.Email &&
               PasswordHash == user.PasswordHash &&
               Role == user.Role &&
               CreatedAt == user.CreatedAt &&
               LastLogin == user.LastLogin;
    }

    // overwrite get hash code
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FirstName, LastName, Email, PasswordHash, Role, CreatedAt, LastLogin);
    }
}
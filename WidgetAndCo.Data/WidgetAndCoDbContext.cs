using Microsoft.EntityFrameworkCore;
using WidgetAndCo.Core;

namespace WidgetAndCo.Data;

public class WidgetStoreDbContext(DbContextOptions<WidgetStoreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
    }

    public async Task SeedDataAsync()
    {
        // Is there no admin user?
        if (!await Users.AnyAsync(u => u.Role == RoleEnum.Admin))
        {
            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123"),
                Role = RoleEnum.Admin,
                CreatedAt = DateTime.UtcNow
            };

            Users.Add(adminUser);
            await SaveChangesAsync();
        }
    }


}
using Microsoft.EntityFrameworkCore;
using WidgetAndCo.Core;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class ProductRepository(WidgetStoreDbContext context) : IProductRepository
{
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await context.Products.ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
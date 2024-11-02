namespace WidgetAndCo.Core.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(Guid id);

    Task<IEnumerable<Product>> GetAllProductsAsync();

    Task AddProductAsync(Product product);

    Task UpdateProductAsync(Product product);

    Task DeleteProductAsync(Guid id);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
}
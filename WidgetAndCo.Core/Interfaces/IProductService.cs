using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IProductService
{
    /**
     * Create a new product
     * @param product The product to create
     * @return The created product
     */
    Task<Product> CreateProduct(CreateProductDto product);

    /**
     * Get a product by id
     * @param id The id of the product to get
     * @return The product
     */
    Task<Product> GetProduct(Guid id);

    /**
     * Get all products
     * @return A list of products
     */
    Task<List<Product>> GetProducts();

    /**
     * Update a product
     * @param id The id of the product to update
     * @param product The updated product
     * @return The updated product
     */
    Task<Product> UpdateProduct(Guid id, Product product);

    /**
     * Delete a product
     * @param id The id of the product to delete
     */
    Task DeleteProduct(Guid id);
}
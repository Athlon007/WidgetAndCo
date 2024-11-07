using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IProductService
{
    /**
     * Create a new product
     * @param product The product to create
     * @return The created product
     */
    Task<ProductResponseDto> CreateProduct(ProductRequestDto product);

    /**
     * Get a product by id
     * @param id The id of the product to get
     * @return The product
     */
    Task<ProductResponseDto> GetProduct(Guid id);

    /**
     * Get all products
     * @return A list of products
     */
    Task<IEnumerable<ProductResponseDto>> GetProducts();

    /**
     * Update a product
     * @param id The id of the product to update
     * @param product The updated product
     * @return The updated product
     */
    Task<ProductResponseDto> UpdateProduct(Guid id, ProductRequestDto product);

    /**
     * Delete a product
     * @param id The id of the product to delete
     */
    Task DeleteProduct(Guid id);
}
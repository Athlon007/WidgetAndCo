using AutoMapper;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class ProductService(
    IProductRepository productRepository,
    IMapper mapper,
    IBlobRepository blobRepository) : IProductService
{
    public async Task<ProductResponseDto> CreateProduct(ProductRequestDto product)
    {
        var newProduct = mapper.Map<Product>(product);

        var blobName = Guid.NewGuid().ToString();
        await blobRepository.UploadBlobAsync(blobName, product.Image.OpenReadStream());
        newProduct.ImageUrl = blobName;

        await productRepository.AddProductAsync(newProduct);

        return mapper.Map<ProductResponseDto>(newProduct);
    }

    public async Task<ProductResponseDto> GetProduct(Guid id)
    {
        var product = await productRepository.GetProductByIdAsync(id);

        // Generate access token
        var imageUri = await blobRepository.GenerateSasTokenAsync(product.ImageUrl);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUri = imageUri
        };
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProducts()
    {
        var products = await productRepository.GetAllProductsAsync();

        // Get access tokens for all images
        var imageUris = new List<Uri>();
        var enumerable = products as Product[] ?? products.ToArray();
        foreach (var product in enumerable)
        {
            var imageUri = await blobRepository.GenerateSasTokenAsync(product.ImageUrl);
            imageUris.Add(imageUri);
        }

        return enumerable.Select((product, index) => new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUri = imageUris[index]
        });
    }

    public async Task<ProductResponseDto> UpdateProduct(Guid id, ProductRequestDto product)
    {
        var existingProduct = await productRepository.GetProductByIdAsync(id);
        if (existingProduct == null)
        {
            throw new Exception("Product not found");
        }

        var updatedProduct = mapper.Map(product, existingProduct);

        // Upload the image to Azure Blob Storage, if a new file is provided
        // Generate random name for file:
        if (product.Image != null)
        {
            // Delete the old image from Azure Blob Storage
            await blobRepository.DeleteBlobAsync(existingProduct.ImageUrl);

            var blobName = Guid.NewGuid().ToString();
            await blobRepository.UploadBlobAsync(blobName, product.Image.OpenReadStream());
            updatedProduct.ImageUrl = blobName;
        }

        await productRepository.UpdateProductAsync(updatedProduct);

        return mapper.Map<ProductResponseDto>(updatedProduct);
    }

    public async Task DeleteProduct(Guid id)
    {
        var product = await productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            throw new Exception("Product not found");
        }

        await productRepository.DeleteProductAsync(product.Id);

        // Delete the image from Azure Blob Storage
        await blobRepository.DeleteBlobAsync(product.ImageUrl);
    }
}
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class ProductService(IProductRepository productRepository): IProductService
{
    public async Task<Product> CreateProduct(CreateProductDto product)
    {
        throw new NotImplementedException();
    }

    public async Task<Product> GetProduct(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Product>> GetProducts()
    {
        throw new NotImplementedException();
    }

    public async Task<Product> UpdateProduct(Guid id, Product product)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteProduct(Guid id)
    {
        throw new NotImplementedException();
    }
}
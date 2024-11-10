using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using WidgetAndCo.Business;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Tests;

public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IBlobRepository> _blobRepositoryMock;

    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new();
        _mapperMock = new();
        _blobRepositoryMock = new();

        _productService = new ProductService(_productRepositoryMock.Object, _mapperMock.Object, _blobRepositoryMock.Object);
    }

    // Example objects
    private Product GetProduct()
    {
        return new Product
        {
            Id = Guid.Parse("50348bf1-5e5b-4139-88c0-14ba8288d906"),
            Name = "Widget",
            Price = 9.99m,
            Description = "A widget",
            ImageUrl = "widget.jpg"
        };
    }

    private ProductRequestDto GetProductRequestDto()
    {
        return new ProductRequestDto
        {
            Name = "Widget",
            Price = 9.99m,
            Description = "A widget",
            Image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())), 0, 0, "Image", "widget.jpg")

        };
    }

    private ProductResponseDto GetProductResponseDto()
    {
        return new ProductResponseDto
        {
            Id = Guid.Parse("50348bf1-5e5b-4139-88c0-14ba8288d906"),
            Name = "Widget",
            Price = 9.99m,
            Description = "A widget",
            ImageUri = new Uri("https://widgetandco.blob.core.windows.net/images/widget.jpg")
        };
    }

    [Test]
    public async Task CreateProduct_ShouldReturnProductResponseDto()
    {
        // Arrange
        var product = GetProduct();
        var productRequestDto = GetProductRequestDto();
        var productResponseDto = GetProductResponseDto();

        _mapperMock.Setup(m => m.Map<Product>(productRequestDto)).Returns(product);
        _blobRepositoryMock.Setup(b => b.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(p => p.AddProductAsync(product)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(productResponseDto);

        // Act
        var result = await _productService.CreateProduct(productRequestDto);

        // Assert
        Assert.AreEqual(productResponseDto, result);
    }

    [Test]
    public async Task CreateProduct_ShouldUploadBlob()
    {
        // Arrange
        var product = GetProduct();
        var productRequestDto = GetProductRequestDto();
        var productResponseDto = GetProductResponseDto();

        _mapperMock.Setup(m => m.Map<Product>(productRequestDto)).Returns(product);
        _blobRepositoryMock.Setup(b => b.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(p => p.AddProductAsync(product)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(productResponseDto);

        // Act
        await _productService.CreateProduct(productRequestDto);

        // Assert
        _blobRepositoryMock.Verify(b => b.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
    }

    [Test]
    public async Task GetProduct_ShouldReturnProductResponseDto()
    {
        // Arrange
        var product = GetProduct();
        var productResponseDto = GetProductResponseDto();

        _productRepositoryMock.Setup(p => p.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _blobRepositoryMock.Setup(b => b.GenerateSasTokenAsync(product.ImageUrl)).ReturnsAsync(new Uri("https://widgetandco.blob.core.windows.net/images/widget.jpg"));

        // Act
        var result = await _productService.GetProduct(product.Id);

        // Assert
        Assert.AreEqual(productResponseDto, result);
    }

    [Test]
    public async Task GetProducts_ShouldReturnProductResponseDtos()
    {
        // Arrange
        var product = GetProduct();
        var productResponseDto = GetProductResponseDto();

        _productRepositoryMock.Setup(p => p.GetAllProductsAsync()).ReturnsAsync(new List<Product> { product });
        _blobRepositoryMock.Setup(b => b.GenerateSasTokenAsync(product.ImageUrl)).ReturnsAsync(new Uri("https://widgetandco.blob.core.windows.net/images/widget.jpg"));

        // Act
        var result = await _productService.GetProducts();

        // Assert
        Assert.AreEqual(productResponseDto, result.First());
    }

    [Test]
    public async Task UpdateProduct_ShouldReturnProductResponseDto()
    {
        // Arrange
        var product = GetProduct();
        var productRequestDto = GetProductRequestDto();
        var productResponseDto = GetProductResponseDto();

        _productRepositoryMock.Setup(p => p.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _blobRepositoryMock.Setup(b => b.GenerateSasTokenAsync(product.ImageUrl)).ReturnsAsync(new Uri("https://widgetandco.blob.core.windows.net/images/widget.jpg"));
        _productRepositoryMock.Setup(p => p.UpdateProductAsync(product)).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ProductResponseDto>(product)).Returns(productResponseDto);
        _mapperMock.Setup(m => m.Map(productRequestDto, product)).Returns(product);

        _blobRepositoryMock.Setup(b => b.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);

        // Act
        var result = await _productService.UpdateProduct(product.Id, productRequestDto);

        // Assert
        Assert.AreEqual(productResponseDto, result);
    }

    [Test]
    public async Task DeleteProduct_ShouldDeleteProduct()
    {
        // Arrange
        var product = GetProduct();

        _productRepositoryMock.Setup(p => p.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _productRepositoryMock.Setup(p => p.DeleteProductAsync(product.Id)).Returns(Task.CompletedTask);

        // Act
        await _productService.DeleteProduct(product.Id);

        // Assert
        _productRepositoryMock.Verify(p => p.DeleteProductAsync(product.Id), Times.Once);
    }

    [Test]
    public async Task DeleteProduct_ShouldNotDeleteProduct_WhenProductDoesNotExist()
    {
        // Arrange
        var product = GetProduct();

        _productRepositoryMock.Setup(p => p.GetProductByIdAsync(product.Id)).ReturnsAsync((Product?)null);

        // Act
        try
        {
            await _productService.DeleteProduct(product.Id);

            // Assert
            Assert.Fail("Exception not thrown");
        } catch (Exception e)
        {
            // Assert
            Assert.AreEqual("Product not found", e.Message);
        }
    }

    [Test]
    public async Task DeleteProduct_ShouldDeleteImage()
    {
        // Arrange
        var product = GetProduct();

        _productRepositoryMock.Setup(p => p.GetProductByIdAsync(product.Id)).ReturnsAsync(product);
        _productRepositoryMock.Setup(p => p.DeleteProductAsync(product.Id)).Returns(Task.CompletedTask);
        _blobRepositoryMock.Setup(b => b.DeleteBlobAsync(product.ImageUrl)).Returns(Task.CompletedTask);

        // Act
        await _productService.DeleteProduct(product.Id);

        // Assert
        _blobRepositoryMock.Verify(b => b.DeleteBlobAsync(product.ImageUrl), Times.Once);
    }
}
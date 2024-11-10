using AutoMapper;
using WidgetAndCo.Business;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Tests;

public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IMapper> _mapperMock;

    private IOrderService _orderService;

    [SetUp]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();

        _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object, _mapperMock.Object);
    }

    public Order GetOrder()
    {
        return new Order()
        {
            PartitionKey = "PartitionKey",
            RowKey = "RowKey",
            Products = new List<Product>()
            {
                new Product()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Product 1",
                    Price = 10m
                },
                new Product()
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Product 2",
                    Price = 20m
                }
            }
        };
    }

    public OrderRequestDto GetOrderRequestDto()
    {
        return new OrderRequestDto("UserId", new List<Guid>()
        {
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Guid.Parse("00000000-0000-0000-0000-000000000002")
        });
    }

    public OrderResponseDto GetOrderResponseDto()
    {
        return new OrderResponseDto(Guid.Parse("00000000-0000-0000-0000-000000000001"), "UserId", new List<Guid>()
        {
            Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Guid.Parse("00000000-0000-0000-0000-000000000002")
        }, 30m);
    }

    [Test]
    public async Task CreateOrderAsync_WithValidOrderRequestDto_ReturnsOrderResponseDto()
    {
        // Arrange
        var order = GetOrder();
        var orderRequestDto = GetOrderRequestDto();
        var orderResponseDto = GetOrderResponseDto();

        _mapperMock.Setup(m => m.Map<Order>(orderRequestDto)).Returns(order);
        _mapperMock.Setup(m => m.Map<OrderResponseDto>(order)).Returns(orderResponseDto);
        _orderRepositoryMock.Setup(or => or.StoreOrderAsync(It.IsAny<OrderRequestDto>())).ReturnsAsync(order);

        // Act
        var result = await _orderService.CreateOrderAsync(orderRequestDto);

        // Assert
        Assert.AreEqual(orderResponseDto, result);
    }

    [Test]
    public async Task GetOrdersAsync_WithValidUserId_ReturnsOrderResponseDtos()
    {
        // Arrange
        var order = GetOrder();
        var orderResponseDto = GetOrderResponseDto();
        var orders = new List<Order>() { order };
        var orderResponseDtos = new List<OrderResponseDto>() { orderResponseDto };

        _orderRepositoryMock.Setup(or => or.GetOrdersAsync(It.IsAny<Guid>())).ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponseDto>>(orders)).Returns(orderResponseDtos);
        _productRepositoryMock.Setup(pr => pr.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Product()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "Product 1",
            Price = 10m
        });

        // Act
        var result = await _orderService.GetOrdersAsync(Guid.NewGuid());

        // Assert
        Assert.AreEqual(orderResponseDtos, result);
    }

}
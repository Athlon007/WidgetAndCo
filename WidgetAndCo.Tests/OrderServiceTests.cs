using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;
using WidgetAndCo.Business;

namespace WidgetAndCo.Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IOrderProductRepository> _orderProductRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderProductRepositoryMock = new Mock<IOrderProductRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _orderProductRepositoryMock.Object,
                _productRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Test]
        public async Task CreateOrderAsync_ShouldReturnOrderResponseDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderRequest = new OrderRequestDto { ProductIds = new Guid[] { Guid.NewGuid() } };
            var storedOrder = new Order { RowKey = Guid.NewGuid().ToString(), Products = new List<Product>() };
            _orderRepositoryMock.Setup(repo => repo.StoreOrderAsync(userId, orderRequest));
            _orderProductRepositoryMock.Setup(repo => repo.AddOrderProductAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                                       .Returns(Task.CompletedTask);
            var expectedOrderResponse = new OrderResponseDto(Guid.Parse(storedOrder.RowKey), "", orderRequest.ProductIds.ToList(), 0m);
            _mapperMock.Setup(m => m.Map<OrderResponseDto>(It.IsAny<Order>())).Returns(expectedOrderResponse);

            // Act
            try {
                await _orderService.CreateOrderAsync(userId, orderRequest);
            } catch (Exception) {
                Assert.Fail("Exception thrown");
            }
        }

        [Test]
        public async Task GetOrderAsync_ShouldReturnOrderResponseDtoWithProducts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var order = new Order { RowKey = orderId.ToString(), Products = new List<Product>() };
            var orderProducts = new List<OrderProduct> { new OrderProduct { RowKey = Guid.NewGuid().ToString() } };
            _orderRepositoryMock.Setup(repo => repo.GetOrderAsync(userId, orderId)).ReturnsAsync(order);
            _orderProductRepositoryMock.Setup(repo => repo.GetOrderProductsAsync(orderId)).ReturnsAsync(orderProducts);
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Product { Price = 10m });
            var expectedOrderResponse = new OrderResponseDto(Guid.Parse(order.RowKey), "", orderProducts.Select(op => Guid.Parse(op.RowKey)).ToList(), 10m);

            // Act
            var result = await _orderService.GetOrderAsync(userId, orderId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedOrderResponse.OrderId, result.OrderId);
            Assert.AreEqual(expectedOrderResponse.Total, result.Total);
        }

        [Test]
        public async Task GetOrdersAsync_ShouldReturnListOfOrderResponseDtos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orders = new List<Order> {
                new Order { RowKey = Guid.NewGuid().ToString(), Products = new List<Product>() },
                new Order { RowKey = Guid.NewGuid().ToString(), Products = new List<Product>() }
            };
            var orderProducts = new List<OrderProduct> { new OrderProduct { RowKey = Guid.NewGuid().ToString() } };
            _orderRepositoryMock.Setup(repo => repo.GetOrdersAsync(userId)).ReturnsAsync(orders);
            _orderProductRepositoryMock.Setup(repo => repo.GetOrderProductsAsync(It.IsAny<Guid>())).ReturnsAsync(orderProducts);
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Product { Price = 20m });

            // Act
            var result = await _orderService.GetOrdersAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(orders.Count, result.Count());
            foreach (var orderResponse in result)
            {
                Assert.IsNotEmpty(orderResponse.ProductIds);
                Assert.AreEqual(20m, orderResponse.Total);
            }
        }

        [Test]
        public async Task CalculateTotal_ShouldReturnCorrectTotal()
        {
            // Arrange
            var order = new Order { Products = new List<Product> { new Product { Id = Guid.NewGuid() } } };
            _productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<Guid>()))
                                  .ReturnsAsync(new Product { Price = 15m });

            // Act
            var total = await _orderService.CalculateTotal(order);

            // Assert
            Assert.AreEqual(15m, total);
        }
    }
}
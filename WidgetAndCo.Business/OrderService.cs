using AutoMapper;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class OrderService(IOrderRepository orderRepository, IOrderProductRepository orderProductRepository, IProductRepository productRepository, IMapper mapper) : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(Guid userId, OrderRequestDto orderRequest)
    {
        var order = await orderRepository.StoreOrderAsync(userId, orderRequest);

        foreach (var productId in orderRequest.ProductIds)
        {
            await orderProductRepository.AddOrderProductAsync(Guid.Parse(order.RowKey), productId);
            order.Products.Add(new Product
            {
                Id = productId,
            });
        }

        return mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> GetOrderAsync(Guid userId, Guid orderId)
    {
        var order = await orderRepository.GetOrderAsync(userId, orderId);

        foreach (var orderProduct in await orderProductRepository.GetOrderProductsAsync(orderId))
        {
            order.Products.Add(new Product
            {
                Id = Guid.Parse(orderProduct.RowKey),
            });
        }

        var total = await CalculateTotal(order);

        return new OrderResponseDto(Guid.Parse(order.RowKey),
            order.PartitionKey,
            order.Products.Select(p => p.Id)
                .ToList(),
            total);

    }

    public async Task<decimal> CalculateTotal(Order order)
    {
        var total = 0m;
        // For each product in the order, get the product details
        foreach (var t in order.Products)
        {
            var product = await productRepository.GetProductByIdAsync(t.Id);
            if (product == null) continue;
            total += product.Price;
        }

        return total;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(Guid userId)
    {
        var orders = await orderRepository.GetOrdersAsync(userId);
        decimal[] totals = new decimal[orders.Count()];
        foreach (var order in orders)
        {
            foreach (var orderProduct in await orderProductRepository.GetOrderProductsAsync(Guid.Parse(order.RowKey)))
            {
                order.Products.Add(new Product
                {
                    Id = Guid.Parse(orderProduct.RowKey),
                });
            }

            var total = await CalculateTotal(order);
            totals[orders.ToList().IndexOf(order)] = total;
        }

        return orders.Select(order => new OrderResponseDto(Guid.Parse(order.RowKey),
            order.PartitionKey,
            order.Products.Select(p => p.Id)
                .ToList(),
            totals[orders.ToList().IndexOf(order)]));
    }
}
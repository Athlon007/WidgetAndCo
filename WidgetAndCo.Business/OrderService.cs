using AutoMapper;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepositry, IMapper mapper) : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequest)
    {
        var order = await orderRepository.StoreOrderAsync(orderRequest);
        return mapper.Map<OrderResponseDto>(order);
    }

    public async Task<OrderResponseDto> GetOrderAsync(Guid userId, Guid orderId)
    {
        var order = await orderRepository.GetOrderAsync(userId, orderId);

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
        for (var i = 0; i < order.Products.Count; i++)
        {
            var product = await productRepositry.GetProductByIdAsync(order.Products[i].Id);
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
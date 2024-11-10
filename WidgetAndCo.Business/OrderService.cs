using AutoMapper;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class OrderService(IOrderRepository orderRepository, IMapper mapper) : IOrderService
{
    public async Task<OrderResponseDto> CreateOrderAsync(OrderRequestDto orderRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<OrderResponseDto> GetOrderAsync(Guid userId, Guid orderId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
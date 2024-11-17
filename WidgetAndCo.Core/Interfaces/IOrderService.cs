using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IOrderService
{
    Task CreateOrderAsync(Guid userId, OrderRequestDto orderRequest);
    Task<OrderResponseDto> GetOrderAsync(Guid userId, Guid orderId);
    Task<IEnumerable<OrderResponseDto>> GetOrdersAsync(Guid userId);
}
using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IOrderRepository
{
    Task StoreOrderAsync(Guid userId, OrderRequestDto order);
    Task<IEnumerable<Order>> GetOrdersAsync(Guid userId);
    Task<Order> GetOrderAsync(Guid userId, Guid orderId);
}
namespace WidgetAndCo.Core.Interfaces;

public interface IOrderProductRepository
{
    Task AddOrderProductAsync(Guid orderId, Guid productId);
    Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(Guid orderId);
}
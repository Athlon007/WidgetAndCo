using Azure.Data.Tables;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class OrderRepository(TableClient tableClient) : IOrderRepository
{
    public async Task<Order> StoreOrderAsync(Guid userId, OrderRequestDto order)
    {
        await tableClient.CreateIfNotExistsAsync();

        var orderEntity = new Order
        {
            PartitionKey = userId.ToString(),
            RowKey = Guid.NewGuid().ToString()
        };

        return orderEntity;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Guid userId)
    {
        await tableClient.CreateIfNotExistsAsync();

        var output = new List<Order>();

        await foreach (Order order in tableClient.QueryAsync<Order>(filter: $"PartitionKey eq '{userId}'"))
        {
            output.Add(new Order
            {
                PartitionKey = order.PartitionKey,
                RowKey = order.RowKey,
            });
        }

        return output;
    }

    public async Task<Order> GetOrderAsync(Guid userId, Guid orderId)
    {
        await tableClient.CreateIfNotExistsAsync();

        var order = await tableClient.GetEntityAsync<Order>(userId.ToString(), orderId.ToString());

        return new Order
        {
            PartitionKey = order.Value.PartitionKey,
            RowKey = order.Value.RowKey,
        };
    }
}
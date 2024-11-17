using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class OrderRepository : IOrderRepository
{
    private readonly TableClient _tableClient;

    public OrderRepository(IConfiguration configuration)
    {
        var connectionString = configuration["AzureWebJobsStorage"] ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set.");
        var orderTableName = configuration["OrderTableName"] ?? throw new InvalidOperationException("OrderTableName environment variable is not set.");

        _tableClient = new TableClient(connectionString, orderTableName);
    }

    public async Task StoreOrderAsync(Guid userId, OrderRequestDto order)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var orderEntity = new Order
        {
            PartitionKey = userId.ToString(),
            RowKey = Guid.NewGuid().ToString()
        };

        await _tableClient.AddEntityAsync(orderEntity);
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Guid userId)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var output = new List<Order>();

        await foreach (Order order in _tableClient.QueryAsync<Order>(filter: $"PartitionKey eq '{userId}'"))
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
        await _tableClient.CreateIfNotExistsAsync();

        var order = await _tableClient.GetEntityAsync<Order>(userId.ToString(), orderId.ToString());

        return new Order
        {
            PartitionKey = order.Value.PartitionKey,
            RowKey = order.Value.RowKey,
        };
    }
}
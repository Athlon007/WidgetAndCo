using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Core;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class OrderProductRepository : IOrderProductRepository
{
    private readonly TableClient _tableClient;

    public OrderProductRepository(IConfiguration configuration)
    {
        var connectionString = configuration["AzureWebJobsStorage"] ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable is not set.");
        var orderProductTableName = configuration["OrderProductTableName"] ?? throw new InvalidOperationException("OrderProductTableName environment variable is not set.");

        _tableClient = new TableClient(connectionString, orderProductTableName);
    }

    public async Task AddOrderProductAsync(Guid orderId, Guid productId)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var orderProduct = new OrderProduct
        {
            PartitionKey = orderId.ToString(),
            RowKey = productId.ToString(),
        };

        await _tableClient.AddEntityAsync(orderProduct);
    }

    public async Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(Guid orderId)
    {
        await _tableClient.CreateIfNotExistsAsync();

        var output = new List<OrderProduct>();

        await foreach (OrderProduct orderProduct in _tableClient.QueryAsync<OrderProduct>(filter: $"PartitionKey eq '{orderId}'"))
        {
            output.Add(orderProduct);
        }

        return output;
    }
}
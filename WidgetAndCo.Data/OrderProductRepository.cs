using Azure.Data.Tables;
using WidgetAndCo.Core;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class OrderProductRepository(TableClient tableClient) : IOrderProductRepository
{
    public async Task AddOrderProductAsync(Guid orderId, Guid productId)
    {
        await tableClient.CreateIfNotExistsAsync();

        var orderProduct = new OrderProduct
        {
            PartitionKey = orderId.ToString(),
            RowKey = productId.ToString(),
        };

        await tableClient.AddEntityAsync(orderProduct);
    }

    public async Task<IEnumerable<OrderProduct>> GetOrderProductsAsync(Guid orderId)
    {
        await tableClient.CreateIfNotExistsAsync();

        var output = new List<OrderProduct>();

        await foreach (OrderProduct orderProduct in tableClient.QueryAsync<OrderProduct>(filter: $"PartitionKey eq '{orderId}'"))
        {
            output.Add(orderProduct);
        }

        return output;
    }
}
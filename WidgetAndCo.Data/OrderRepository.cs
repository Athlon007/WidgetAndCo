using Azure.Data.Tables;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class OrderRepository(TableClient orderTableClient, TableClient orderProductTableClient) : IOrderRepository
{
    public async Task<Order> StoreOrderAsync(OrderRequestDto order)
    {
        await orderTableClient.CreateIfNotExistsAsync();
        await orderProductTableClient.CreateIfNotExistsAsync();

        var orderEntity = new Order
        {
            PartitionKey = order.UserId,
            RowKey = Guid.NewGuid().ToString()
        };

        await orderTableClient.AddEntityAsync(orderEntity);

        foreach (var productId in order.ProductIds)
        {
            var orderProduct = new OrderProduct
            {
                PartitionKey = orderEntity.RowKey,
                RowKey = productId.ToString(),
            };

            await orderProductTableClient.AddEntityAsync(orderProduct);
        }

        orderEntity.Products = order.ProductIds.Select(id => new Product
        {
            Id = id
        }).ToList();

        return orderEntity;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Guid userId)
    {
        var output = new List<Order>();

        await foreach (Order order in orderTableClient.QueryAsync<Order>(filter: $"PartitionKey eq '{userId}'"))
        {
            var orderProducts = new List<Product>();

            await foreach (OrderProduct orderProduct in orderProductTableClient.QueryAsync<OrderProduct>(filter: $"PartitionKey eq '{order.RowKey}'"))
            {
                orderProducts.Add(new Product
                {
                    Id = Guid.Parse(orderProduct.RowKey)
                });
            }

            output.Add(new Order
            {
                PartitionKey = order.PartitionKey,
                RowKey = order.RowKey,
                Products = orderProducts
            });
        }

        return output;
    }

    public async Task<Order> GetOrderAsync(Guid userId, Guid orderId)
    {
        var order = await orderTableClient.GetEntityAsync<Order>(userId.ToString(), orderId.ToString());

        var orderProducts = new List<Product>();

        await foreach (OrderProduct orderProduct in orderProductTableClient.QueryAsync<OrderProduct>(filter: $"PartitionKey eq '{order.Value.RowKey}'"))
        {
            orderProducts.Add(new Product
            {
                Id = Guid.Parse(orderProduct.RowKey)
            });
        }

        return new Order
        {
            PartitionKey = order.Value.PartitionKey,
            RowKey = order.Value.RowKey,
            Products = orderProducts
        };
    }
}
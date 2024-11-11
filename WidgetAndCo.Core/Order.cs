using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Azure;
using Azure.Data.Tables;

namespace WidgetAndCo.Core;

public class Order : ITableEntity
{
    public string PartitionKey { get; set; } // Corresponds to user id

    public string RowKey { get; set; } // Order ID

    [IgnoreDataMember]
    public List<Product> Products { get; set; } = [];
    public decimal Total => Products.Sum(p => p.Price);

    public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public ETag ETag { get; set; } = new ETag("*");

    public Order()
    {
    }

    public Order(string userId, List<Product> products)
    {
        PartitionKey = userId;
        RowKey = Guid.NewGuid().ToString();
        Products = products;
    }
}
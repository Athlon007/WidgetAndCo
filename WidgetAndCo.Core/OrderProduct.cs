using Azure;
using Azure.Data.Tables;

namespace WidgetAndCo.Core;

public class OrderProduct : ITableEntity
{
    public string PartitionKey { get; set; } // Corresponds to order id
    public string RowKey { get; set; } // Corresponds to product id
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
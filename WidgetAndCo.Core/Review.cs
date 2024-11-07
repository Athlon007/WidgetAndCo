using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Azure;
using Azure.Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace WidgetAndCo.Core;

public class Review : ITableEntity
{
    public string PartitionKey { get; set; } // Corresponds to product id
    public string RowKey { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }

    public string Description { get; set; }
    public int Rating { get; set; }

    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Review()
    {
    }

    public Review(Guid userId, string title, string description, int rating)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Rating = rating;
    }
}
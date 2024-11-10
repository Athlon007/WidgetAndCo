using Azure;
using Azure.Data.Tables;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Data;

public class ReviewRepository(TableClient tableClient) : IReviewRepository
{
    public async Task StoreReviewAsync(ReviewDelegateDto reviewDelegate)
    {
        // Store review in Azure Table Storage
        await tableClient.CreateIfNotExistsAsync();

        var reviewEntity = new Review
        {
            PartitionKey = reviewDelegate.ProductId.ToString(),
            RowKey = Guid.NewGuid().ToString(),
            UserId = reviewDelegate.UserId,
            Title = reviewDelegate.Title,
            Description = reviewDelegate.Description,
            Rating = reviewDelegate.Rating,
            Timestamp = DateTimeOffset.UtcNow,
            ETag = new ETag("*")
        };

        await tableClient.AddEntityAsync(reviewEntity);
    }

    public async Task<IEnumerable<Review>> GetReviewsAsync(Guid productId)
    {
        // Get reviews from Azure Table Storage
        var reviews = new List<Review>();

        await foreach (Review review in tableClient.QueryAsync<Review>(filter: $"PartitionKey eq '{productId}'"))
        {
            reviews.Add(new Review
            {
                RowKey = Guid.Parse(review.RowKey).ToString(),
                UserId = review.UserId,
                Title = review.Title,
                Description = review.Description,
                Rating = review.Rating
            });
        }

        return reviews;
    }

    public async Task<Review> GetReviewAsync(Guid productId, Guid reviewId)
    {
        // Get review from Azure Table Storage
        var review = await tableClient.GetEntityAsync<Review>(productId.ToString(), reviewId.ToString());

        return new Review
        {
            RowKey = Guid.Parse(review.Value.RowKey).ToString(),
            UserId = review.Value.UserId,
            Title = review.Value.Title,
            Description = review.Value.Description,
            Rating = review.Value.Rating
        };
    }
}
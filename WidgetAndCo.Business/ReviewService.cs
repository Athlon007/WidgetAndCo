using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class ReviewService(IConfiguration configuration) : IReviewService
{
    private readonly HttpClient _client = new();

    public async Task DelegateStoreReviewAsync(ReviewRequestDto reviewRequest, Guid userId)
    {
        var reviewDelegate = new ReviewDelegateDto(userId,
                reviewRequest.ProductId,
                reviewRequest.Title,
                reviewRequest.Description,
                reviewRequest.Rating);

        // Delegate the Function to the Function App
        var json = JsonSerializer.Serialize(reviewDelegate);
        var response = await _client.PostAsync(configuration["FunctionsUrls:StoreReview"],
            new StringContent(json,
                Encoding.UTF8,
                "application/json")
            );

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to store review");
        }
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetReviews(Guid productId)
    {
        throw new NotImplementedException();
    }

    public async Task<ReviewResponseDto> GetReview(Guid productId, Guid reviewId)
    {
        throw new NotImplementedException();
    }
}
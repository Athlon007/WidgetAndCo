using System.Text;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Business;

public class ReviewService(IReviewRepository reviewRepository, IMapper mapper) : IReviewService
{
    public async Task StoreReview(ReviewRequestDto reviewRequest, Guid userId)
    {
        // Store review in Azure Table Storage
        await reviewRepository.StoreReviewAsync(new ReviewDelegateDto(userId,
            reviewRequest.ProductId,
            reviewRequest.Title,
            reviewRequest.Description,
            reviewRequest.Rating));
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetReviews(Guid productId)
    {
        var reviews = await reviewRepository.GetReviewsAsync(productId);
        return reviews.Select(review => new ReviewResponseDto
        {
            ProductId = productId.ToString(),
            ReviewId = review.RowKey,
            Title = review.Title,
            Description = review.Description,
            Rating = review.Rating
        });
    }

    public async Task<ReviewResponseDto> GetReview(Guid productId, Guid reviewId)
    {
        var review = await reviewRepository.GetReviewAsync(productId, reviewId);
        return mapper.Map<ReviewResponseDto>(review);
    }
}
using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IReviewService
{
    Task DelegateStoreReviewAsync(ReviewRequestDto reviewRequest, Guid userId);
    Task<IEnumerable<ReviewResponseDto>> GetReviews(Guid productId);
    Task<ReviewResponseDto> GetReview(Guid productId, Guid reviewId);
}
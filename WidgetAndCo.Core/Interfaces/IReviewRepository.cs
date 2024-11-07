using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IReviewRepository
{
    Task StoreReviewAsync(ReviewDelegateDto reviewDelegate);
    Task<IEnumerable<ReviewResponseDto>> GetReviewsAsync(Guid productId);
    Task<ReviewResponseDto> GetReviewAsync(Guid productId, Guid reviewId);
}
using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IReviewRepository
{
    Task StoreReviewAsync(ReviewDelegateDto reviewDelegate);
    Task<IEnumerable<Review>> GetReviewsAsync(Guid productId);
    Task<Review> GetReviewAsync(Guid productId, Guid reviewId);
}
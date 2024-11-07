using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpPost(Name = "CreateReview")]
    public async Task<IActionResult> CreateReview(ReviewRequestDto reviewRequest)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        await reviewService.DelegateStoreReviewAsync(reviewRequest, userId);

        return Ok();
    }
}
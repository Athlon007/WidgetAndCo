using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.ReviewProcessingFunction;

public class GetReview(ILogger<GetReview> logger, IReviewService reviewService)
{
    [Function("GetReview")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        // Get product ID and review ID from query string
        var productId = req.Query["productId"];
        var reviewId = req.Query["reviewId"];

        Guid partitionKey;
        Guid rowKey;

        if (string.IsNullOrEmpty(productId) || !Guid.TryParse(productId, out partitionKey))
        {
            return new BadRequestObjectResult("Please pass a product id in the query string");
        }

        if (string.IsNullOrEmpty(reviewId) || !Guid.TryParse(reviewId, out rowKey))
        {
            return new BadRequestObjectResult("Please pass a review id in the query string");
        }

        logger.LogInformation($"Product ID: {partitionKey}\nReview ID: {rowKey}");

        var reviews = reviewService.GetReview(partitionKey, rowKey);

        return new OkObjectResult(reviews);
    }

}
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.ReviewProcessingFunction;

public class GetReviewsFunction(ILogger<GetReviewsFunction> logger, IReviewService reviewService)
{

    [Function("GetReviews")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetReviews/{productId}")] HttpRequest req, string productId)
    {
        Guid partitionKey;

        if (string.IsNullOrEmpty(productId) || !Guid.TryParse(productId, out partitionKey))
        {
            return new BadRequestObjectResult("Please pass a product id in the query string");
        }

        logger.LogInformation($"Product ID: {partitionKey}");

        var reviews = await reviewService.GetReviews(partitionKey);

        return new OkObjectResult(reviews);
    }

}
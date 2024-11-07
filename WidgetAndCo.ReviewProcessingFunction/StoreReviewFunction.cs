using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.ReviewProcessingFunction;

public class StoreReviewFunction(ILogger<StoreReviewFunction> logger, IReviewRepository reviewRepository)
{
    [Function("StoreReview")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        // Get information from request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var reviewRequest = JsonSerializer.Deserialize<ReviewDelegateDto>(requestBody);

        if (reviewRequest == null)
        {
            return new BadRequestObjectResult("Please pass a valid review in the request body");
        }

        // Store review in Azure Table Storage
        await reviewRepository.StoreReviewAsync(reviewRequest);

        return new OkObjectResult("Review stored successfully");
    }

}
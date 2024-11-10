using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.ReviewProcessingFunction;

public class StoreReviewFunction(
    ILogger<StoreReviewFunction> logger,
    IReviewService reviewService,
    IUserService userService)
{
    [Function("StoreReview")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        // Get JWT from request headers
        var jwt = req.Headers.Authorization.ToString().Replace("Bearer ", "");

        // Validate JWT
        if (!userService.ValidateToken(jwt))
        {
            return new UnauthorizedResult();
        }

        logger.LogInformation("Authorized user");

        var userId = userService.GetUserIdFromToken(jwt);

        logger.LogInformation($"User ID: {userId}");

        // Get information from request body
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var reviewRequest = JsonSerializer.Deserialize<ReviewRequestDto>(requestBody);

        if (reviewRequest == null)
        {
            return new BadRequestObjectResult("Please pass a valid review in the request body");
        }

        logger.LogInformation($"Review request: {JsonSerializer.Serialize(reviewRequest)}");

        // Store review in Azure Table Storage
        await reviewService.StoreReview(reviewRequest, userId);

        logger.LogInformation("Review stored successfully");

        return new OkObjectResult("Review stored successfully");
    }
}
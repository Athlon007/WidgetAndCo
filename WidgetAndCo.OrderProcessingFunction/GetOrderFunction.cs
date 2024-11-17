using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.OrderProcessingFunction;

public class GetOrderFunction(ILogger<GetOrderFunction> logger, IOrderService orderService, IUserService userService)
{
    [Function("GetOrderFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, string orderId)
    {
        // Get JWT from request headers
        var jwt = req.Headers.Authorization.ToString().Replace("Bearer ", "");

        // Validate JWT
        if (!userService.ValidateToken(jwt))
        {
            return new UnauthorizedResult();
        }

        var userId = userService.GetUserIdFromToken(jwt);

        Guid orderIdGuid;
        if (!Guid.TryParse(orderId, out orderIdGuid))
        {
            return new BadRequestObjectResult("Please pass a valid order ID in the query string");
        }

        logger.LogInformation("Authorized user");

        var order = await orderService.GetOrderAsync(userId, orderIdGuid);
        return new OkObjectResult(order);
    }

}
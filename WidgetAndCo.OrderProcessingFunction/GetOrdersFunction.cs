using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.OrderProcessingFunction;

public class GetOrdersFunction(ILogger<GetOrdersFunction> logger, IOrderService orderService, IUserService userService)
{
    [Function("GetOrders")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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

        var orders = await orderService.GetOrdersAsync(userId);
        return new OkObjectResult(orders);
    }

}
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.OrderProcessingFunction;

public class CreateOrderFunction(ILogger<CreateOrderFunction> logger, IOrderService orderService, IUserService userService)
{
    [Function("CreateOrder")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        // Get JWT from request headers
        var jwt = req.Headers.Authorization.ToString().Replace("Bearer ", "");

        // Validate JWT
        if (!userService.ValidateToken(jwt))
        {
            return new UnauthorizedResult();
        }

        var userId = userService.GetUserIdFromToken(jwt);

        var orderRequestString = await new StreamReader(req.Body).ReadToEndAsync();
        var orderRequest = JsonConvert.DeserializeObject<OrderRequestDto>(orderRequestString) ?? throw new ArgumentNullException("Please pass a valid order in the request body");

        await orderService.CreateOrderAsync(userId, orderRequest);
        return new OkResult();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductController(IProductService productService): ControllerBase
{
    [HttpGet(Name = "GetProducts")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await productService.GetProducts();
        return Ok(products);
    }
}
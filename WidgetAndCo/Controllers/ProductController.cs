using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductController(IProductService productService): ControllerBase
{
    [HttpGet(Name = "GetProducts")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await productService.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id:guid}", Name = "GetProduct")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await productService.GetProduct(id);
        return Ok(product);
    }

    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromForm] ProductRequestDto product)
    {
        var newProduct = await productService.CreateProduct(product);
        return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
    }

    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [HttpPut("{id:guid}", Name = "UpdateProduct")]
    [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductRequestDto product)
    {
        var updatedProduct = await productService.UpdateProduct(id, product);
        return Ok(updatedProduct);
    }

    [Authorize(Roles = nameof(RoleEnum.Admin))]
    [HttpDelete("{id:guid}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await productService.DeleteProduct(id);
        return NoContent();
    }
}
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.Application.DTOs;
using MusicoStore.Application.Interfaces.IRepositories;
using MusicoStore.Domain.Entities;

namespace MusicoStore.WebApi.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await repo.GetAllAsync(ct);
        return Ok(products.Adapt<List<ProductDto>>());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var product = await repo.GetAsync(id, ct);
        if (product == null) return NotFound();

        return Ok(product.Adapt<ProductDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product dto, CancellationToken ct)
    {
        var created = await repo.AddAsync(dto, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Product dto, CancellationToken ct)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        await repo.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await repo.DeleteAsync(id, ct);
        return NoContent();
    }
}

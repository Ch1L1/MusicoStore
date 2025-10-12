using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.Infrastructure.Repository;
using MusicoStore.WebApi.Models;

namespace MusicoStore.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController(IRepository<Product> productRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<Product> products = await productRepository.GetAllAsync(ct);
        return Ok(products.Select(p => new
        {
            ProductId = p.Id,
            p.Name,
            p.Description,
            p.CurrentPrice,
            p.CurrencyCode,
            Category = new
            {
                CategoryId = p.ProductCategory?.Id,
                CategoryName = p.ProductCategory?.Name,
            }
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(id, ct);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            ProductId = product.Id,
            product.Name,
            product.Description,
            product.CurrentPrice,
            product.CurrencyCode,
            Category = new
            {
                CategoryId = product.ProductCategory?.Id,
                CategoryName = product.ProductCategory?.Name,
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            CurrentPrice = model.CurrentPrice,
            CurrencyCode = model.CurrencyCode,
            ProductCategoryId = model.ProductCategoryId
        };

        Product created = await productRepository.AddAsync(product, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await productRepository.UpdateAsync(new Product
        {
            Id = id,
            Name = model.Name,
            Description = model.Description,
            CurrentPrice = model.CurrentPrice,
            CurrencyCode = model.CurrencyCode,
            ProductCategoryId = model.ProductCategoryId
        }, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(id, ct);
        if (product == null)
        {
            return NotFound($"Product with id: \'{id}\' not found");
        }

        await productRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

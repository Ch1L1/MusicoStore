using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.WebApi.Models;

namespace MusicoStore.WebApi.Controllers;

public class ProductCategoryController(IRepository<ProductCategory> categoryRepository) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<ProductCategory> categories = await categoryRepository.GetAllAsync(ct);
        return Ok(categories.Select(c => new
        {
            CategoryId = c.Id,
            c.Name,
            Products = c.Products?.Select(p => new
            {
                ProductId = p.Id,
                p.Name,
                p.Description,
                p.CurrentPrice,
                p.CurrencyCode,
                ManufacturerName = p.Manufacturer?.Name
            })
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        ProductCategory? category = await categoryRepository.GetByIdAsync(id, ct);
        if (category == null)
        {
            return NotFound($"Category with id '{id}' not found");
        }

        return Ok(new
        {
            CategoryId = category.Id,
            category.Name,
            Products = category.Products?.Select(p => new
            {
                ProductId = p.Id,
                p.Name,
                p.Description,
                p.CurrentPrice,
                p.CurrencyCode,
                ManufacturerName = p.Manufacturer?.Name
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCategoryModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var productCategory = new ProductCategory
        {
            Name = model.Name
        };

        ProductCategory created = await categoryRepository.AddAsync(productCategory, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductCategoryModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await categoryRepository.UpdateAsync(new ProductCategory
        {
            Id = id,
            Name = model.Name
        }, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        ProductCategory? category = await categoryRepository.GetByIdAsync(id, ct);
        if (category == null)
        {
            return NotFound($"Category with id '{id}' not found");
        }

        await categoryRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

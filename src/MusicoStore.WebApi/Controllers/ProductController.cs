using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.Infrastructure.Models;
using MusicoStore.Infrastructure.Repository;
using MusicoStore.WebApi.Models;

namespace MusicoStore.WebApi.Controllers;

public class ProductsController(ProductRepository productRepository) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterRequest request, CancellationToken ct)
    {
        var criteria = new ProductFilterCriteria
        {
            Name = request.Name,
            Description = request.Description,
            MaxPrice = request.MaxPrice,
            Category = request.Category,
            Manufacturer = request.Manufacturer
        };

        IReadOnlyList<Product> products = await productRepository.FilterAsync(criteria, ct);

        var result = products.Select(p => new
        {
            ProductId = p.Id,
            p.Name,
            p.Description,
            p.CurrentPrice,
            p.CurrencyCode,
            Category = new
            {
                CategoryId = p.ProductCategory?.Id,
                CategoryName = p.ProductCategory?.Name
            },
            Manufacturer = new
            {
                ManufacturerId = p.Manufacturer?.Id,
                ManufacturerName = p.Manufacturer?.Name
            }
        });

        return Ok(result);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(id, ct);
        if (product == null)
        {
            return NotFound($"Product with id '{id}' not found");
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
            },
            Manufacturer = new
            {
                ManufacturerId = product.Manufacturer?.Id,
                ManufacturerName = product.Manufacturer?.Name
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
            ProductCategoryId = model.ProductCategoryId,
            ManufacturerId = model.ManufacturerId
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
            ProductCategoryId = model.ProductCategoryId,
            ManufacturerId = model.ManufacturerId
        }, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(id, ct);
        if (product == null)
        {
            return NotFound($"Product with id '{id}' not found");
        }

        await productRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

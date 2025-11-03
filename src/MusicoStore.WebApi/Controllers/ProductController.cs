using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Models;
using MusicoStore.DataAccessLayer.Repository;
using MusicoStore.WebApi.Models;
using AutoMapper;
using MusicoStore.WebApi.Models.Dtos;

namespace MusicoStore.WebApi.Controllers;

public class ProductsController(ProductRepository productRepository, IMapper mapper) : ApiControllerBase
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

        var result = mapper.Map<IEnumerable<ProductDto>>(products);
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

        var dto = mapper.Map<ProductDto>(product);
        return Ok(dto);
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
        var dto = mapper.Map<ProductDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
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

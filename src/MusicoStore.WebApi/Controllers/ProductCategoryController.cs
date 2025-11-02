using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.WebApi.Models;
using AutoMapper;
using MusicoStore.WebApi.Models.Dtos;

namespace MusicoStore.WebApi.Controllers;

public class ProductCategoryController(IRepository<ProductCategory> categoryRepository, IMapper mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<ProductCategory> categories = await categoryRepository.GetAllAsync(ct);
        var result = mapper.Map<IEnumerable<ProductCategoryDto>>(categories);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        ProductCategory? category = await categoryRepository.GetByIdAsync(id, ct);
        if (category == null)
        {
            return NotFound($"Category with id '{id}' not found");
        }

        var dto = mapper.Map<ProductCategoryDto>(category);
        return Ok(dto);
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
        var dto = mapper.Map<ProductCategoryDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
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

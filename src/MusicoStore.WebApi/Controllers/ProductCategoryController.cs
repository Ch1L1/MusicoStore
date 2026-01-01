using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Constants;

namespace MusicoStore.WebApi.Controllers;

public class ProductCategoryController(IProductCategoryService productCategoryService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<ProductCategoryDTO> productCategories = await productCategoryService.FindAllAsync(ct);
        return Ok(productCategories);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!productCategoryService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Category", $"id '{id}'"));
        }

        ProductCategoryDTO productCategory = await productCategoryService.FindByIdAsync(id, ct);
        return Ok(productCategory);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCategoryDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ProductCategoryDTO res = await productCategoryService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = res.CategoryId }, res);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductCategoryDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!productCategoryService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Category", $"id '{id}'"));
        }

        await productCategoryService.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!productCategoryService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Category", $"id '{id}'"));
        }

        await productCategoryService.DeleteByIdAsync(id, ct);
        return NoContent();
    }
}

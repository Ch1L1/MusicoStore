using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

[Route("api/products/{productId:int}/categories")]
public class ProductCategoryAssignmentController(
    IProductCategoryAssignmentService assignmentService)
    : ApiControllerBase
{
    /// <summary>
    /// Get all categories assigned to a product.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories(
        int productId,
        CancellationToken ct)
    {
        List<ProductCategoryDTO> categories =
            await assignmentService.GetCategoriesForProductAsync(productId, ct);

        return Ok(categories);
    }

    /// <summary>
    /// Assign a category to a product.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AssignCategory(
        int productId,
        AssignProductCategoryDTO dto,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await assignmentService.AssignCategoryAsync(
            productId,
            dto.CategoryId,
            dto.IsPrimary,
            ct);

        return NoContent();
    }

    /// <summary>
    /// Remove a category from a product.
    /// </summary>
    [HttpDelete("{categoryId:int}")]
    public async Task<IActionResult> RemoveCategory(
        int productId,
        int categoryId,
        CancellationToken ct)
    {
        await assignmentService.RemoveCategoryAsync(
            productId,
            categoryId,
            ct);

        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.Constants;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

public class ProductsController(IProductService productService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Filter([FromQuery] ProductFilterRequestDTO req, CancellationToken ct)
    {
        List<ProductDTO> products = await productService.FilterAsync(req, ct);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!productService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }

        ProductDTO product = await productService.FindByIdAsync(id, ct);
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDTO req, [FromQuery] int customerId, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ProductDTO created = await productService.CreateAsync(req, customerId, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProductDTO req, [FromQuery] int customerId,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!productService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }

        await productService.UpdateAsync(id, req, customerId, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] int customerId, CancellationToken ct)
    {
        if (!productService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }

        await productService.DeleteByIdAsync(id, customerId, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file, [FromQuery] int customerId,
        CancellationToken ct)
    {
        if (!productService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        await using Stream stream = file.OpenReadStream();

        var fileDto = new FileDTO
        {
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType
        };

        try
        {
            var relativePath = await productService.UploadImageAsync(id, fileDto, customerId, ct);

            var imageUrl = $"{Request.Scheme}://{Request.Host}/{relativePath}";
            return Ok(new { imageUrl, relativePath });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    [HttpDelete("{id:int}/image")]
    public async Task<IActionResult> DeleteImage(int id, [FromQuery] int customerId, CancellationToken ct)
    {
        if (!productService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Product", $"id '{id}'"));
        }

        try
        {
            await productService.DeleteImageAsync(id, customerId, ct);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }
}

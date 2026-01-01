using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Constants;

namespace MusicoStore.WebApi.Controllers;

public class ManufacturerController(IManufacturerService manufacturerService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<ManufacturerDTO> manufacturers = await manufacturerService.FindAllAsync(ct);
        return Ok(manufacturers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!manufacturerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Manufacturer", $"id '{id}'"));
        }

        ManufacturerDTO manufacturer = await manufacturerService.FindByIdAsync(id, ct);
        return Ok(manufacturer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateManufacturerDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ManufacturerDTO created = await manufacturerService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.ManufacturerId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateManufacturerDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!manufacturerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Manufacturer", $"id '{id}'"));
        }

        await manufacturerService.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!manufacturerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Manufacturer", $"id '{id}'"));
        }

        await manufacturerService.DeleteByIdAsync(id, ct);
        return NoContent();
    }
}

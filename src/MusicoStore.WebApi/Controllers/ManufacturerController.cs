using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.WebApi.Models;
using AutoMapper;
using MusicoStore.WebApi.Models.Dtos;

namespace MusicoStore.WebApi.Controllers;

public class ManufacturerController(IRepository<Manufacturer> manufacturerRepository, IMapper mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<Manufacturer> manufacturers = await manufacturerRepository.GetAllAsync(ct);
        var result = mapper.Map<IEnumerable<ManufacturerDto>>(manufacturers);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Manufacturer? manufacturer = await manufacturerRepository.GetByIdAsync(id, ct);
        if (manufacturer == null)
        {
            return NotFound($"Manufacturer with id '{id}' not found");
        }

        var dto = mapper.Map<ManufacturerDto>(manufacturer);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ManufacturerModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var manufacturer = new Manufacturer
        {
            Name = model.Name,
            AddressId = model.AddressId
        };

        Manufacturer created = await manufacturerRepository.AddAsync(manufacturer, ct);
        var dto = mapper.Map<ManufacturerDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ManufacturerModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await manufacturerRepository.UpdateAsync(new Manufacturer
        {
            Id = id,
            Name = model.Name,
            AddressId = model.AddressId
        }, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        Manufacturer? manufacturer = await manufacturerRepository.GetByIdAsync(id, ct);
        if (manufacturer == null)
        {
            return NotFound($"Manufacturer with id '{id}' not found");
        }

        await manufacturerRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.Infrastructure.Repository;
using MusicoStore.WebApi.Models;

namespace MusicoStore.WebApi.Controllers;

public class ManufacturerController(IRepository<Manufacturer> manufacturerRepository) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<Manufacturer> manufacturers = await manufacturerRepository.GetAllAsync(ct);
        return Ok(manufacturers.Select(m => new
        {
            ManufacturerId = m.Id,
            m.Address,
            Products = m.Products?.Select(p => new
            {
                ProductId = p.Id,
                p.Name,
                p.Description,
                p.CurrentPrice,
                p.CurrencyCode,
                CategoryName = p.ProductCategory?.Name
            })
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Manufacturer? manufacturer = await manufacturerRepository.GetByIdAsync(id, ct);
        if (manufacturer == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            ManufacturerId = manufacturer.Id,
            manufacturer.Address,
            Products = manufacturer.Products?.Select(p => new
            {
                ProductId = p.Id,
                p.Name,
                p.Description,
                p.CurrentPrice,
                p.CurrencyCode,
                CategoryName = p.ProductCategory?.Name
            })
        });
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
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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
            return NotFound();
        }

        await manufacturerRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

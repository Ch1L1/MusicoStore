using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.Infrastructure.Repository;
using MusicoStore.WebApi.Models;

namespace MusicoStore.WebApi.Controllers;

public class AddressController(IRepository<Address> addressRepository) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<Address> addresses = await addressRepository.GetAllAsync(ct);
        return Ok(addresses.Select(a => new
        {
            AddressId = a.Id,
            a.StreetName,
            a.StreetNumber,
            a.City,
            a.PostalNumber,
            a.CountryCode
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Address? address = await addressRepository.GetByIdAsync(id, ct);
        if (address == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            AddressId = address.Id,
            address.StreetName,
            address.StreetNumber,
            address.City,
            address.PostalNumber,
            address.CountryCode
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddressModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var address = new Address
        {
            City = model.City,
            StreetName = model.StreetName,
            StreetNumber = model.StreetNumber,
            CountryCode = model.CountryCode,
            PostalNumber = model.PostalNumber
        };

        Address created = await addressRepository.AddAsync(address, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        Address? address = await addressRepository.GetByIdAsync(id, ct);
        if (address == null)
        {
            return NotFound();
        }

        await addressRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

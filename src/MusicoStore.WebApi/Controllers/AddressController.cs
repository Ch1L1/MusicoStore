using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.WebApi.Models;
using AutoMapper;
using MusicoStore.WebApi.Models.Dtos;

namespace MusicoStore.WebApi.Controllers;

public class AddressController(IRepository<Address> addressRepository, IMapper mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        IReadOnlyList<Address> addresses = await addressRepository.GetAllAsync(ct);
        var result = mapper.Map<IEnumerable<AddressDto>>(addresses);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        Address? address = await addressRepository.GetByIdAsync(id, ct);
        if (address == null)
        {
            return NotFound($"Address with id '{id}' not found");
        }

        var dto = mapper.Map<AddressDto>(address);
        return Ok(dto);
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
        var dto = mapper.Map<AddressDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        Address? address = await addressRepository.GetByIdAsync(id, ct);
        if (address == null)
        {
            return NotFound($"Address with id '{id}' not found");
        }

        await addressRepository.DeleteAsync(id, ct);
        return NoContent();
    }
}

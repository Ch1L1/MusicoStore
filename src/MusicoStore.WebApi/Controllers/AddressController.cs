using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Constants;

namespace MusicoStore.WebApi.Controllers;

public class AddressController(IAddressService addressService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<AddressDTO> addresses = await addressService.FindAllAsync(ct);
        return Ok(addresses);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!addressService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Address", $"id '{id}'"));
        }

        AddressDTO address = await addressService.FindByIdAsync(id, ct);
        return Ok(address);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAddressDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        AddressDTO res = await addressService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = res.AddressId }, res);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!addressService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Address", $"id '{id}'"));
        }

        await addressService.DeleteByIdAsync(id, ct);
        return NoContent();
    }
}

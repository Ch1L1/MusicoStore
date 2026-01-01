using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Customer;
using MusicoStore.Domain.Constants;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

public class CustomerController(ICustomerService customerService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<CustomerDTO> customers = await customerService.FindAllAsync(ct);
        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!customerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Customer", $"id '{id}'"));
        }

        CustomerDTO customer = await customerService.FindByIdAsync(id, ct);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CustomerDTO created = await customerService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!customerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Customer", $"id '{id}'"));
        }

        await customerService.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!customerService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Customer", $"id '{id}'"));
        }

        await customerService.DeleteByIdAsync(id, ct);
        return NoContent();
    }
}

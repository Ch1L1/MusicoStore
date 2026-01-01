using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.CustomerAddress;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;


public class CustomerAddressController : ApiControllerBase
{
    private readonly ICustomerAddressService _service;

    public CustomerAddressController(ICustomerAddressService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress(
        int customerId,
        [FromBody] UpsertCustomerAddressDTO dto,
        CancellationToken ct)
    {
        var result = await _service.AddAddressAsync(customerId, dto, ct);
        return Ok(result);
    }
}

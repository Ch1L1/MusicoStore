using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.Constants;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

public class OrderController(IOrderService orderService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<OrderDTO> orders = await orderService.FindAllAsync(ct);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!orderService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Order", $"id '{id}'"));
        }

        OrderDTO order = await orderService.FindByIdAsync(id, ct);
        return Ok(order);
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId, CancellationToken ct)
    {
        List<OrderDTO> orders = await orderService.FindByCustomerIdAsync(customerId, ct);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDTO dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        OrderDTO created = await orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
    }

    [HttpPost("change-state")]
    public async Task<IActionResult> ChangeState(ChangeOrderStateDTO dto, CancellationToken ct)
    {
        await orderService.ChangeStateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!orderService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Order", $"id '{id}'"));
        }

        await orderService.DeleteByIdAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{orderId:int}/apply-gift-card")]
    public async Task<IActionResult> ApplyGiftCard(int orderId, ApplyGiftCardDTO dto, CancellationToken ct)
    {
        await orderService.ApplyGiftCardAsync(orderId, dto.CouponCode, ct);
        return NoContent();
    }

    [HttpDelete("{orderId:int}/remove-gift-card")]
    public async Task<IActionResult> RemoveGiftCard(int orderId, CancellationToken ct)
    {
        await orderService.RemoveGiftCardAsync(orderId, ct);
        return NoContent();
    }

}

using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.GiftCard;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.WebApi.Controllers;

[Route("api/v1/gift-cards")]
public class GiftCardController(IGiftCardService giftCardService)
    : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateGiftCardDTO dto,
        CancellationToken ct)
    {
        var created = await giftCardService.CreateAsync(dto, ct);
        return Ok(created);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var giftCard = await giftCardService.GetByIdAsync(id, ct);
        return Ok(giftCard);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await giftCardService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/generate-coupon")]
    public async Task<IActionResult> GenerateCoupon(
        int id,
        CancellationToken ct)
    {
        var coupon = await giftCardService.GenerateCouponAsync(id, ct);
        return Ok(coupon);
    }
}

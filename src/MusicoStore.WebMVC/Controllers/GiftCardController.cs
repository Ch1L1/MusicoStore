using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.GiftCard;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.GiftCard;

namespace WebMVC.Controllers;

[Authorize(Roles = "Employee")]
[Route("admin/giftcards")]
public class GiftCardController(IGiftCardService giftCardService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        List<GiftCardDTO> dtos = await giftCardService.FindAllAsync(ct);
        return View(dtos);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new GiftCardCreateViewModel());
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(GiftCardCreateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        CreateGiftCardDTO? dto = mapper.Map<CreateGiftCardDTO>(model);
        await giftCardService.CreateAsync(dto, ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        GiftCardDTO dto = await giftCardService.GetByIdAsync(id, ct);
        
        var vm = new GiftCardDetailViewModel
        {
            GiftCard = dto
        };

        return View(vm);
    }

    [HttpPost("{id:int}/generate")]
    public async Task<IActionResult> GenerateCoupon(int id, CancellationToken ct)
    {
        await giftCardService.GenerateCouponAsync(id, ct);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await giftCardService.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}

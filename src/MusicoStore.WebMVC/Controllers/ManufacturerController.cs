using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Manufacturer;

namespace WebMVC.Controllers;

[Route("manufacturers")]
public class ManufacturerController(
    IManufacturerService manufacturerService,
    IAddressService addressService,
    IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        List<ManufacturerDTO> dtos = await manufacturerService.FindAllAsync(ct);
        List<ManufacturerViewModel>? vms = mapper.Map<List<ManufacturerViewModel>>(dtos);
        return View(vms);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new ManufacturerFormViewModel();
        await LoadAddressDropdown(vm, ct);
        return View(vm);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(ManufacturerFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadAddressDropdown(model, ct);
            return View(model);
        }

        try
        {
            CreateManufacturerDTO? dto = mapper.Map<CreateManufacturerDTO>(model);
            await manufacturerService.CreateAsync(dto, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error creating manufacturer: " + ex.Message);
            await LoadAddressDropdown(model, ct);
            return View(model);
        }
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        ManufacturerDTO dto = await manufacturerService.FindByIdAsync(id, ct);

        ManufacturerFormViewModel? vm = mapper.Map<ManufacturerFormViewModel>(dto);
        await LoadAddressDropdown(vm, ct);

        return View(vm);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ManufacturerFormViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadAddressDropdown(model, ct);
            return View(model);
        }

        try
        {
            UpdateManufacturerDTO? dto = mapper.Map<UpdateManufacturerDTO>(model);
            await manufacturerService.UpdateAsync(id, dto, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error updating manufacturer: " + ex.Message);
            await LoadAddressDropdown(model, ct);
            return View(model);
        }
    }

    private async Task LoadAddressDropdown(ManufacturerFormViewModel model, CancellationToken ct)
    {
        List<AddressDTO> addresses = await addressService.FindAllAsync(ct);

        model.Addresses = addresses.Select(a => new SelectListItem
        {
            Value = a.AddressId.ToString(),
            Text = $"{a.StreetName} {a.StreetNumber}, {a.City} ({a.CountryCode})"
        }).ToList();
    }
}

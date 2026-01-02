using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Category;

namespace WebMVC.Controllers;

[Route("categories")]
public class CategoryController(IProductCategoryService categoryService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        List<ProductCategoryDTO> dtos = await categoryService.FindAllAsync(ct);
        List<ProductCategoryViewModel>? vms = mapper.Map<List<ProductCategoryViewModel>>(dtos);
        return View(vms);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new ProductCategoryFormViewModel());
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(ProductCategoryFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            CreateProductCategoryDTO? dto = mapper.Map<CreateProductCategoryDTO>(model);
            await categoryService.CreateAsync(dto, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error creating category: " + ex.Message);
            return View(model);
        }
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        ProductCategoryDTO dto = await categoryService.FindByIdAsync(id, ct);
        ProductCategoryFormViewModel? vm = mapper.Map<ProductCategoryFormViewModel>(dto);
        return View(vm);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ProductCategoryFormViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            UpdateProductCategoryDTO? dto = mapper.Map<UpdateProductCategoryDTO>(model);
            await categoryService.UpdateAsync(id, dto, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error updating category: " + ex.Message);
            return View(model);
        }
    }
}

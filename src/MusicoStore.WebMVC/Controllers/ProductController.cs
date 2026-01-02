using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Products;

namespace WebMVC.Controllers;

[Route("products")]
public class ProductController(
    IProductService productService,
    IProductCategoryService categoryService,
    IProductCategoryAssignmentService assignmentService,
    IManufacturerService manufacturerService,
    IMapper mapper,
    UserManager<LocalIdentityUser> userManager,
    SignInManager<LocalIdentityUser> signInManager) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> AllProducts([FromQuery] ProductFilterRequestDTO filter, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        filter ??= new ProductFilterRequestDTO();
        var paged = await productService.FilterPagedAsync(filter, page, pageSize, ct);
        var productVms = mapper.Map<List<ProductViewModel>>(paged.Items);

        var vm = new ProductListViewModel
        {
            Products = productVms,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling((double)paged.TotalCount / pageSize),
            PageSize = pageSize
        };

        return View(vm);
    }

    [HttpGet("suggest")]
    [AllowAnonymous]
    public async Task<IActionResult> Suggest([FromQuery] string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Json(new SearchResultDTO());
        }

        string q = query.Trim();

        var productFilter = new ProductFilterRequestDTO { Name = q };
        List<ProductDTO> products = await productService.FilterAsync(productFilter, ct);

        List<ProductCategoryDTO> categories = await categoryService.FindAllAsync(ct);
        string qLower = q.ToLowerInvariant();
        categories = categories
            .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name!.ToLowerInvariant().Contains(qLower))
            .ToList();

        List<ManufacturerDTO> manufacturers = await manufacturerService.FindAllAsync(ct);
        manufacturers = manufacturers
            .Where(m => !string.IsNullOrEmpty(m.Name) && m.Name!.ToLowerInvariant().Contains(qLower))
            .ToList();

        var result = new SearchResultDTO
        {
            Products = products,
            Categories = categories,
            Manufacturers = manufacturers
        };

        return Json(result);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var viewModel = new ProductCreateViewModel();
        await LoadCreateDropdowns(viewModel, ct);
        return View(viewModel);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(ProductCreateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadCreateDropdowns(model, ct);
            return View(model);
        }

        LocalIdentityUser? user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        try
        {
            CreateProductDTO dto = mapper.Map<CreateProductDTO>(model);
            ProductDTO createdProduct = await productService.CreateAsync(dto, user.CustomerId, ct);
            await assignmentService.AssignCategoryAsync(
                createdProduct.ProductId,
                model.ProductCategoryId,
                isPrimary: true,
                ct);

            return RedirectToAction(nameof(AllProducts));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Couldn't create product: " + ex.Message);
            await LoadCreateDropdowns(model, ct);
            return View(model);
        }
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        ProductDTO product = await productService.FindByIdAsync(id, ct);

        List<ProductCategoryDTO> assignedCategories = await assignmentService.GetCategoriesForProductAsync(id, ct);
        ProductCategoryDTO? primaryCategory = assignedCategories.FirstOrDefault(c => c.IsPrimary)
                                              ?? assignedCategories.FirstOrDefault();

        ProductEditViewModel? vm = mapper.Map<ProductEditViewModel>(product);

        if (primaryCategory != null)
        {
            vm.ProductCategoryId = primaryCategory.CategoryId;
        }

        await LoadEditDropdowns(vm, ct);
        return View(vm);
    }

    [Authorize(Roles = "Employee")]
    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, ProductEditViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadEditDropdowns(model, ct);
            return View(model);
        }

        LocalIdentityUser? user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        try
        {
            UpdateProductDTO? updateDto = mapper.Map<UpdateProductDTO>(model);
            await productService.UpdateAsync(id, updateDto, user.CustomerId, ct);

            List<ProductCategoryDTO> currentCategories = await assignmentService.GetCategoriesForProductAsync(id, ct);
            ProductCategoryDTO? currentPrimary = currentCategories.FirstOrDefault(c => c.IsPrimary);

            if (currentPrimary == null || currentPrimary.CategoryId != model.ProductCategoryId)
            {
                await assignmentService.AssignCategoryAsync(
                    id,
                    model.ProductCategoryId,
                    isPrimary: true,
                    ct);
            }

            return RedirectToAction(nameof(AllProducts));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Product update failed: " + ex.Message);
            await LoadEditDropdowns(model, ct);
            return View(model);
        }
    }

    private async Task LoadCreateDropdowns(ProductCreateViewModel model, CancellationToken ct)
    {
        model.Categories = await GetCategorySelectList(ct);
        model.Manufacturers = await GetManufacturerSelectList(ct);
    }

    private async Task LoadEditDropdowns(ProductEditViewModel model, CancellationToken ct)
    {
        model.Categories = await GetCategorySelectList(ct);
        model.Manufacturers = await GetManufacturerSelectList(ct);
    }

    private async Task<IEnumerable<SelectListItem>> GetCategorySelectList(CancellationToken ct)
    {
        List<ProductCategoryDTO> categories = await categoryService.FindAllAsync(ct);
        return categories.Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.Name
        }).ToList();
    }

    private async Task<IEnumerable<SelectListItem>> GetManufacturerSelectList(CancellationToken ct)
    {
        List<ManufacturerDTO> manufacturers = await manufacturerService.FindAllAsync(ct);
        return manufacturers.Select(m => new SelectListItem
        {
            Value = m.ManufacturerId.ToString(),
            Text = m.Name
        }).ToList();
    }
}

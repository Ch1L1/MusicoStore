using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Products;

namespace WebMVC.Controllers;

[Route("products")]
public class ProductController(IProductService productService, IMapper mapper) : Controller
{
    public async Task<IActionResult> AllProducts(CancellationToken ct)
    {
        List<ProductDTO> products = await productService.FilterAsync(new ProductFilterRequestDTO(), ct);
        List<ProductViewModel> model = mapper.Map<List<ProductViewModel>>(products);
        return View(model);
    }
}

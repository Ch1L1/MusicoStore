using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.DTOs.Product;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Orders;

namespace WebMVC.Controllers;

[Authorize(Roles = "Employee")]
[Route("orders")]
public class OrderController(IOrderService orderService, IProductService productService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        List<OrderDTO> dtos = await orderService.FindAllAsync(ct);

        var vms = dtos.Select(o => new OrderListViewModel
        {
            OrderId = o.OrderId,
            TotalAmount = o.TotalAmount,
            CreatedAt = o.CreatedAt,
            CurrentState = o.CurrentState,
            HasGiftCard = !string.IsNullOrEmpty(o.GiftCardCouponCode),
            CustomerId = o.CustomerId
        }).OrderByDescending(x => x.CreatedAt).ToList();

        return View(vms);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            OrderDTO dto = await orderService.FindByIdAsync(id, ct);
            List<string> states = await orderService.GetOrderStates(ct);

            var statesToSelect =
                new List<SelectListItem>(states.Select(((state, i) => new SelectListItem(state, $"{i + 1}"))).ToList());

            var vm = new OrderDetailViewModel
            {
                Order = dto,
                AvailableStates = statesToSelect
            };
            return View(vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("change-state")]
    public async Task<IActionResult> ChangeState(int orderId, int newStateId, CancellationToken ct)
    {
        try
        {
            await orderService.ChangeStateAsync(new ChangeOrderStateDTO { OrderId = orderId, NewStateId = newStateId },
                ct);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error changing state: " + ex.Message);
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var vm = new OrderCreateViewModel();

        vm.Items.Add(new OrderItemCreateViewModel { Quantity = 1 });

        await LoadFormDropdowns(vm, CancellationToken.None);
        return View(vm);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(OrderCreateViewModel model, CancellationToken ct)
    {
        model.Items.RemoveAll(i => i.ProductId == 0);

        if (model.Items.Count == 0)
        {
            ModelState.AddModelError("", "Order items cannot be empty");
        }

        if (!ModelState.IsValid)
        {
            await LoadFormDropdowns(model, ct);
            return View(model);
        }

        try
        {
            var createDto = new CreateOrderDTO
            {
                CustomerId = model.CustomerId,
                Items = model.Items.Select(i => new CreateOrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            OrderDTO createdOrder = await orderService.CreateAsync(createDto, ct);

            if (string.IsNullOrWhiteSpace(model.CouponCode))
            {
                return RedirectToAction(nameof(Details), new { id = createdOrder.OrderId });
            }

            try
            {
                await orderService.ApplyGiftCardAsync(createdOrder.OrderId, model.CouponCode.Trim(), ct);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.GetHashCode().ToString(),
                    $"Order created, but error applying git card: {ex.Message}");
            }

            return RedirectToAction(nameof(Details), new { id = createdOrder.OrderId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(ex.GetHashCode().ToString(), "Error creating order: " + ex.Message);
            await LoadFormDropdowns(model, ct);
            return View(model);
        }
    }

    private async Task LoadFormDropdowns(OrderCreateViewModel model, CancellationToken ct)
    {
        List<ProductDTO> products = await productService.FilterAsync(new ProductFilterRequestDTO(), ct);

        model.AvailableProducts = products.Select(p => new SelectListItem
        {
            Value = p.ProductId.ToString(),
            Text = $"{p.Name} ({p.CurrentPrice} {p.CurrencyCode})"
        }).OrderBy(x => x.Text).ToList();
    }
}

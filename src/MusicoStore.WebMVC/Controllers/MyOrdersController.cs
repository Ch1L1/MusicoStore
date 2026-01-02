using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.Domain.DTOs.Order;
using MusicoStore.Domain.Interfaces.Service;
using WebMVC.Models.Orders;

namespace WebMVC.Controllers;

[Authorize]
public class MyOrdersController(
    IOrderService orderService,
    UserManager<LocalIdentityUser> userManager)
    : Controller
{
    public async Task<IActionResult> MyOrders(CancellationToken ct)
    {
        LocalIdentityUser? user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        List<OrderDTO> orders = await orderService
            .FindByCustomerIdAsync(user.CustomerId, ct);

        var vm = orders.Select(o => new MyOrderViewModel
        {
            OrderId = o.OrderId,
            CreatedAt = o.CreatedAt,
            Status = o.CurrentState,
            TotalAmount = o.TotalAmount
        }).ToList();

        return View(vm);
    }
}

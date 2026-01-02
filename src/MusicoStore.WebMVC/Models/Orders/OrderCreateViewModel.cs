using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebMVC.Models.Orders;

public class OrderCreateViewModel
{
    public int CustomerId { get; set; }
    public string? CouponCode { get; set; }
    public List<OrderItemCreateViewModel> Items { get; set; } = [new()];
    public IEnumerable<SelectListItem>? AvailableProducts { get; set; }
}

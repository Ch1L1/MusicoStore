using Microsoft.AspNetCore.Mvc.Rendering;
using MusicoStore.Domain.DTOs.Order;

namespace WebMVC.Models.Orders;

public class OrderDetailViewModel
{
    public OrderDTO Order { get; set; }
    public int NewState { get; set; }
    public IEnumerable<SelectListItem> AvailableStates { get; set; } // TODO
}

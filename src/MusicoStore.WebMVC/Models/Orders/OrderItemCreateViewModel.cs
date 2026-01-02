namespace WebMVC.Models.Orders;

public class OrderItemCreateViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

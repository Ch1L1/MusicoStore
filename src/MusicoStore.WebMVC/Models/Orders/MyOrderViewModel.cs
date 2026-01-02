namespace WebMVC.Models.Orders;

public class MyOrderViewModel
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
}

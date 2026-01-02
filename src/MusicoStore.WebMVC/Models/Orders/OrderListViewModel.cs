namespace WebMVC.Models.Orders;

public class OrderListViewModel
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CurrentState { get; set; }
    public decimal TotalAmount { get; set; }
    public bool HasGiftCard { get; set; }
}

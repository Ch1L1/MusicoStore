namespace MusicoStore.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsEmployee { get; set; }

    public ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<ProductEditLog> EditedProducts { get; set; } = new List<ProductEditLog>();
}

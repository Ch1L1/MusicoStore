namespace MusicoStore.Domain.DTOs.Customer;

public class UpdateCustomerDTO
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool Employee { get; set; }
}

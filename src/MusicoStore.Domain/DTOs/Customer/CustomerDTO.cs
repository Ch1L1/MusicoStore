using MusicoStore.Domain.DTOs.CustomerAddress;

namespace MusicoStore.Domain.DTOs.Customer;

public class CustomerDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool Employee { get; set; }
    public List<CustomerAddressSummaryForCustomerDTO> Addresses { get; set; } = [];
}

using WebMVC.Models.Address;

namespace WebMVC.Models.Manufacturer;

public class ManufacturerViewModel
{
    public int ManufacturerId { get; set; }
    public string Name { get; set; }
    public AddressViewModel Address { get; set; }
}

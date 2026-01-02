using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebMVC.Models.Manufacturer;

public class ManufacturerFormViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public int AddressId { get; set; }
    public IEnumerable<SelectListItem>? Addresses { get; set; }
}

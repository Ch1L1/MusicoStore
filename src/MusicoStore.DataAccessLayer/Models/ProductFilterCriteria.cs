namespace MusicoStore.DataAccessLayer.Models;

public class ProductFilterCriteria
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Category { get; set; }
    public string? Manufacturer { get; set; }
}

using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.Domain.DTOs.Product;

public class CreateProductDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal CurrentPrice { get; set; }
    public required Currency CurrencyCode { get; set; }
    public required int ProductCategoryId { get; set; }
    public required int ManufacturerId { get; set; }
}

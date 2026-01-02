using System.ComponentModel.DataAnnotations;

namespace MusicoStore.Domain.DTOs.ProductCategory;

public class AssignProductCategoryDTO
{
    [Required]
    public int CategoryId { get; set; }

    public bool IsPrimary { get; set; }
}

using MusicoStore.Domain.DTOs.ProductCategory;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IProductCategoryAssignmentService
{
    Task<List<ProductCategoryDTO>> GetCategoriesForProductAsync(int productId, CancellationToken ct);

    Task AssignCategoryAsync(int productId, int categoryId, bool isPrimary, CancellationToken ct);

    Task RemoveCategoryAsync(int productId, int categoryId, CancellationToken ct);
}

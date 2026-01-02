using MusicoStore.Domain.Entities;

namespace MusicoStore.Domain.Interfaces.Repository;

public interface IProductCategoryAssignmentRepository
{
    Task<IReadOnlyList<ProductCategoryAssignment>> GetForProductAsync(
        int productId,
        CancellationToken ct);

    Task<ProductCategoryAssignment?> GetAsync(
        int productId,
        int categoryId,
        CancellationToken ct);

    Task<ProductCategoryAssignment?> GetPrimaryForProductAsync(
        int productId,
        CancellationToken ct);

    Task<bool> ExistsAsync(
        int productId,
        int categoryId,
        CancellationToken ct);

    Task AddAsync(
        ProductCategoryAssignment assignment,
        CancellationToken ct);

    Task RemoveAsync(
        ProductCategoryAssignment assignment,
        CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}

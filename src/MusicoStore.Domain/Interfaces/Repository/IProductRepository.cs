using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Records;

namespace MusicoStore.Domain.Interfaces.Repository;

public interface IProductRepository : IRepository<Product>
{
    public Task<Product?> GetByIdWithoutLogsAsync(int id, CancellationToken ct);

    public Task<IReadOnlyList<Product>> FilterAsync(ProductFilterCriteria filter, CancellationToken ct);
}

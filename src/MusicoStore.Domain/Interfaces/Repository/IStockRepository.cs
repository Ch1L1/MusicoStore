using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Records;

namespace MusicoStore.Domain.Interfaces.Repository;

public interface IStockRepository : IRepository<Stock>
{
    public Task<IReadOnlyList<Stock>> FilterAsync(StockFilterCriteria filter, CancellationToken ct);
}

using MusicoStore.Domain.Entities;

namespace MusicoStore.Domain.Interfaces.Repository;

public interface IOrderStatusLogRepository : IRepository<OrderStatusLog>
{
    public Task<string?> GetLatestStateNameAsync(int orderId, CancellationToken ct);
}

namespace MusicoStore.Infrastructure.Repository;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct);
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct);
    Task UpdateAsync(TEntity entity, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}


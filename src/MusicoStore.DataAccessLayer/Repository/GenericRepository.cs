using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public abstract class GenericRepository<TEntity>(AppDbContext db) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly AppDbContext Db = db;
    protected DbSet<TEntity> Set => Db.Set<TEntity>();

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct)
        => await Set.AsNoTracking().ToListAsync(ct);

    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken ct)
        => Set.AsNoTracking()
              .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);

    public virtual bool DoesEntityExist(int id)
        => Set.Any(e => EF.Property<int>(e, "Id") == id);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
    {
        Set.Add(entity);
        await Db.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        Set.Update(entity);
        await Db.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct)
    {
        var entity = await Set.FindAsync([id], ct);
        if (entity is not null)
        {
            Set.Remove(entity);
            await Db.SaveChangesAsync(ct);
        }
    }
}
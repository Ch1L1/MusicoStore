using MusicoStore.Domain.Entities;

namespace MusicoStore.Application.Interfaces.IRepositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetAsync(int id, CancellationToken ct);
    Task<Product> AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
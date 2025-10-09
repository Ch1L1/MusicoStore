using MusicoStore.Domain.Entities;

namespace MusicoStore.Application.Abstractions.Interfaces;

public interface IProductCategoryRepository
{
    Task<IEnumerable<ProductCategory>> GetAllAsync();
    Task<ProductCategory?> GetByIdAsync(int id);
    Task<ProductCategory> AddAsync(ProductCategory category);
    Task<ProductCategory> UpdateAsync(ProductCategory category);
    Task DeleteAsync(int id);
}

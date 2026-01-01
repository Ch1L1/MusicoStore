using MusicoStore.Domain.DTOs.ProductCategory;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IProductCategoryService
{
    public Task<List<ProductCategoryDTO>> FindAllAsync(CancellationToken ct);
    public Task<ProductCategoryDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<ProductCategoryDTO> CreateAsync(CreateProductCategoryDTO dto, CancellationToken ct);
    public Task UpdateAsync(int id, UpdateProductCategoryDTO dto, CancellationToken ct);
    public Task DeleteByIdAsync(int id, CancellationToken ct);
}

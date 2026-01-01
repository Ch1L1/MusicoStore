using MusicoStore.Domain.DTOs;
using MusicoStore.Domain.DTOs.Product;

namespace MusicoStore.Domain.Interfaces.Service;

public interface IProductService
{
    public Task<List<ProductDTO>> FilterAsync(ProductFilterRequestDTO filter, CancellationToken ct);
    public Task<ProductDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<ProductDTO> CreateAsync(CreateProductDTO dto, int editedByCustomerId, CancellationToken ct);
    public Task UpdateAsync(int id, UpdateProductDTO dto, int editedByCustomerId, CancellationToken ct);
    public Task DeleteByIdAsync(int id, int editedByCustomerId, CancellationToken ct);
    public Task<string> UploadImageAsync(int id, FileDTO fileDto, int editedByCustomerId, CancellationToken ct);
    public Task DeleteImageAsync(int id, int editedByCustomerId, CancellationToken ct);
}

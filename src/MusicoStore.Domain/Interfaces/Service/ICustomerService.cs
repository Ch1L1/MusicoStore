using MusicoStore.Domain.DTOs.Customer;

namespace MusicoStore.Domain.Interfaces.Service;

public interface ICustomerService
{
    public Task<List<CustomerDTO>> FindAllAsync(CancellationToken ct);
    public Task<CustomerDTO> FindByIdAsync(int id, CancellationToken ct);
    public bool DoesExistById(int id);
    public Task<CustomerDTO> CreateAsync(CreateCustomerDTO dto, CancellationToken ct);
    public Task UpdateAsync(int id, UpdateCustomerDTO dto, CancellationToken ct);
    public Task DeleteByIdAsync(int id, CancellationToken ct);
}

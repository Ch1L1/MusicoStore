using AutoMapper;
using MusicoStore.Domain.DTOs.Customer;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class CustomerService(IRepository<Customer> customerRepository, IMapper mapper)
    : ICustomerService
{
    public async Task<List<CustomerDTO>> FindAllAsync(CancellationToken ct)
    {
        IReadOnlyList<Customer> customers = await customerRepository.GetAllAsync(ct);
        return mapper.Map<List<CustomerDTO>>(customers);
    }

    public async Task<CustomerDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        Customer? customer = await customerRepository.GetByIdAsync(id, ct);
        return mapper.Map<CustomerDTO>(customer);
    }

    public bool DoesExistById(int id)
        => customerRepository.DoesEntityExist(id);

    public async Task<CustomerDTO> CreateAsync(CreateCustomerDTO dto, CancellationToken ct)
    {
        Customer customer = mapper.Map<Customer>(dto);
        Customer created = await customerRepository.AddAsync(customer, ct);
        return mapper.Map<CustomerDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateCustomerDTO dto, CancellationToken ct)
    {
        Customer customer = mapper.Map<Customer>(dto);
        customer.Id = id;
        await customerRepository.UpdateAsync(customer, ct);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
        => await customerRepository.DeleteAsync(id, ct);
}

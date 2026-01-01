using AutoMapper;
using MusicoStore.Domain.DTOs.CustomerAddress;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class CustomerAddressService(
    ICustomerAddressRepository customerAddressRepository,
    IRepository<Customer> customerRepository,
    IRepository<Address> addressRepository,
    IMapper mapper)
    : ICustomerAddressService
{
    public async Task<CustomerAddressSummaryForCustomerDTO> AddAddressAsync(
        int customerId, UpsertCustomerAddressDTO dto, CancellationToken ct)
    {
        if (!customerRepository.DoesEntityExist(customerId))
        {
            throw new ArgumentException($"Customer {customerId} not found.");
        }

        bool hasExisting = dto.ExistingAddressId.HasValue;
        bool hasNew = dto.NewAddress is not null;

        if (hasExisting == hasNew)
        {
            throw new ArgumentException("Provide either ExistingAddressId or NewAddress.");
        }

        Address address;

        if (hasExisting)
        {
            address = await addressRepository.GetByIdAsync(dto.ExistingAddressId!.Value, ct)
                ?? throw new ArgumentException("Existing address not found.");
        }
        else
        {
            var duplicate = await customerAddressRepository
                .FindDuplicateAddressAsync(dto.NewAddress!, ct);

            if (duplicate != null)
            {
                address = duplicate;
            }
            else
            {
                address = mapper.Map<Address>(dto.NewAddress!);
                address = await addressRepository.AddAsync(address, ct);
            }
        }

        if (dto.IsMainAddress)
        {
            await customerAddressRepository.UnsetAllMainAddressesAsync(customerId, ct);
        }

        var existingCustomerAddress = await customerAddressRepository
            .GetByCustomerAndAddressAsync(customerId, address.Id, ct);

        if (existingCustomerAddress != null)
        {
            if (dto.IsMainAddress && !existingCustomerAddress.IsMainAddress)
            {
                existingCustomerAddress.IsMainAddress = true;
                await customerAddressRepository.UpdateAsync(existingCustomerAddress, ct);
            }

            return mapper.Map<CustomerAddressSummaryForCustomerDTO>(existingCustomerAddress);
        }

        var entity = new CustomerAddress
        {
            CustomerId = customerId,
            AddressId = address.Id,
            IsMainAddress = dto.IsMainAddress
        };

        var created = await customerAddressRepository.AddAsync(entity, ct);

        return mapper.Map<CustomerAddressSummaryForCustomerDTO>(created);
    }
}

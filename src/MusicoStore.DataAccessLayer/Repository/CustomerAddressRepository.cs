using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.DTOs.Address;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class CustomerAddressRepository(AppDbContext db) : GenericRepository<CustomerAddress>(db), ICustomerAddressRepository
{
    public override async Task<IReadOnlyList<CustomerAddress>> GetAllAsync(CancellationToken ct)
        => await Db.CustomerAddresses
            .Include(ca => ca.Customer)
            .Include(ca => ca.Address)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<CustomerAddress?> GetByIdAsync(int id, CancellationToken ct)
        => Db.CustomerAddresses
            .Include(ca => ca.Customer)
            .Include(ca => ca.Address)
            .Include(ca => ca.Orders)
            .AsNoTracking()
            .FirstOrDefaultAsync(ca => ca.Id == id, ct);

    public async Task<IReadOnlyList<CustomerAddress>> GetForCustomerAsync(int customerId, CancellationToken ct)
        => await Db.CustomerAddresses
            .Where(ca => ca.CustomerId == customerId)
            .Include(ca => ca.Address)
            .ToListAsync(ct);

    public Task<CustomerAddress?> GetMainAddressForCustomerAsync(int customerId, CancellationToken ct)
        => Db.CustomerAddresses
            .Include(ca => ca.Address)
            .FirstOrDefaultAsync(ca =>
                ca.CustomerId == customerId &&
                ca.IsMainAddress == true, ct);

    public Task<CustomerAddress?> GetByCustomerAndAddressAsync(int customerId, int addressId, CancellationToken ct)
        => Db.CustomerAddresses
            .FirstOrDefaultAsync(ca =>
                ca.CustomerId == customerId &&
                ca.AddressId == addressId, ct);

    public Task<Address?> FindDuplicateAddressAsync(CreateAddressDTO dto, CancellationToken ct)
        => Db.Addresses.FirstOrDefaultAsync(a =>
            a.StreetName == dto.StreetName &&
            a.StreetNumber == dto.StreetNumber &&
            a.City == dto.City &&
            a.PostalNumber == dto.PostalNumber &&
            a.CountryCode == dto.CountryCode, ct);

    public async Task UnsetAllMainAddressesAsync(int customerId, CancellationToken ct)
    {
        var list = await Db.CustomerAddresses
            .Where(ca => ca.CustomerId == customerId && ca.IsMainAddress)
            .ToListAsync(ct);

        foreach (var ca in list)
        {
            ca.IsMainAddress = false;
        }


        if (list.Any())
        {
            await Db.SaveChangesAsync(ct);
        }
    }
}

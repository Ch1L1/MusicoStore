using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MusicoStore.Domain.DTOs.Manufacturer;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class ManufacturerService(IRepository<Manufacturer> manufacturerRepository, IMapper mapper, IMemoryCache cache)
    : IManufacturerService
{
    private static readonly MemoryCacheEntryOptions CacheOptions =
        new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

    private const string AllCategoriesCacheKey = "manufacturers_all";
    private static string CategoryByIdCacheKey(int id) => $"manufacturers_{id}";

    public async Task<List<ManufacturerDTO>> FindAllAsync(CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            AllCategoriesCacheKey,
            async entry =>
            {
                entry.SetOptions(CacheOptions);
                IReadOnlyList<Manufacturer> manufacturers = await manufacturerRepository.GetAllAsync(ct);
                return mapper.Map<List<ManufacturerDTO>>(manufacturers);
            })!;
    }

    public async Task<ManufacturerDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        return await cache.GetOrCreateAsync(
            CategoryByIdCacheKey(id),
            async entry =>
            {
                entry.SetOptions(CacheOptions);
                Manufacturer? manufacturer = await manufacturerRepository.GetByIdAsync(id, ct);
                return mapper.Map<ManufacturerDTO>(manufacturer);
            })!;
    }

    public bool DoesExistById(int id)
        => manufacturerRepository.DoesEntityExist(id);

    public async Task<ManufacturerDTO> CreateAsync(
        CreateManufacturerDTO dto, CancellationToken ct)
    {
        Manufacturer created = await manufacturerRepository.AddAsync(mapper.Map<Manufacturer>(dto), ct);

        cache.Remove(AllCategoriesCacheKey);

        return mapper.Map<ManufacturerDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateManufacturerDTO dto, CancellationToken ct)
    {
        Manufacturer manufacturer = mapper.Map<Manufacturer>(dto);
        manufacturer.Id = id;

        await manufacturerRepository.UpdateAsync(manufacturer, ct);

        cache.Remove(AllCategoriesCacheKey);
        cache.Remove(CategoryByIdCacheKey(id));
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
    {
        await manufacturerRepository.DeleteAsync(id, ct);

        cache.Remove(AllCategoriesCacheKey);
        cache.Remove(CategoryByIdCacheKey(id));
    }
}

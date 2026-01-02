using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class ProductCategoryService(IRepository<ProductCategory> productCategoryRepository, IMapper mapper, IMemoryCache cache) : IProductCategoryService
{

    private static readonly MemoryCacheEntryOptions CacheOptions =
        new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

    private const string AllCategoriesCacheKey = "product_categories_all";
    private static string CategoryByIdCacheKey(int id) => $"product_category_{id}";

    public async Task<List<ProductCategoryDTO>> FindAllAsync(CancellationToken ct)
    {
        if (cache.TryGetValue(AllCategoriesCacheKey, out List<ProductCategoryDTO>? cached))
        {
            return cached!;
        }

        IReadOnlyList<ProductCategory> categories = await productCategoryRepository.GetAllAsync(ct);

        var mapped = mapper.Map<List<ProductCategoryDTO>>(categories);

        cache.Set(AllCategoriesCacheKey, mapped, CacheOptions);

        return mapped;
    }

    public async Task<ProductCategoryDTO> FindByIdAsync(int id, CancellationToken ct)
    {
        string cacheKey = CategoryByIdCacheKey(id);

        if (cache.TryGetValue(cacheKey, out ProductCategoryDTO? cached))
        {
            return cached!;
        }

        ProductCategory? category =
            await productCategoryRepository.GetByIdAsync(id, ct);

        var mapped = mapper.Map<ProductCategoryDTO>(category);

        cache.Set(cacheKey, mapped, CacheOptions);

        return mapped;
    }

    public bool DoesExistById(int id)
        => productCategoryRepository.DoesEntityExist(id);

    public async Task<ProductCategoryDTO> CreateAsync(CreateProductCategoryDTO dto, CancellationToken ct)
    {
        ProductCategory entity = mapper.Map<ProductCategory>(dto);
        ProductCategory created = await productCategoryRepository.AddAsync(entity, ct);

        cache.Remove(AllCategoriesCacheKey);

        return mapper.Map<ProductCategoryDTO>(created);
    }

    public async Task UpdateAsync(int id, UpdateProductCategoryDTO dto, CancellationToken ct)
    {
        ProductCategory entity = mapper.Map<ProductCategory>(dto);
        entity.Id = id;

        await productCategoryRepository.UpdateAsync(entity, ct);

        cache.Remove(AllCategoriesCacheKey);
        cache.Remove(CategoryByIdCacheKey(id));
    }

    public async Task DeleteByIdAsync(int id, CancellationToken ct)
    {
        await productCategoryRepository.DeleteAsync(id, ct);

        cache.Remove(AllCategoriesCacheKey);
        cache.Remove(CategoryByIdCacheKey(id));
    }
}

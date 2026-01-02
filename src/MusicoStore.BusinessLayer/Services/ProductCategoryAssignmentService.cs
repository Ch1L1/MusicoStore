using Microsoft.Extensions.Caching.Memory;
using MusicoStore.Domain.DTOs.ProductCategory;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Interfaces.Service;

namespace MusicoStore.BusinessLayer.Services;

public class ProductCategoryAssignmentService(
    IProductRepository productRepository,
    IRepository<ProductCategory> productCategoryRepository,
    IProductCategoryAssignmentRepository assignmentRepository,
    IMemoryCache cache)
    : IProductCategoryAssignmentService
{

    private static readonly MemoryCacheEntryOptions CacheOptions =
        new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

    private static string CacheKeyForProduct(int productId)
        => $"product_{productId}_categories";

    public async Task<List<ProductCategoryDTO>> GetCategoriesForProductAsync(int productId, CancellationToken ct)
    {
        string cacheKey = CacheKeyForProduct(productId);

        if (cache.TryGetValue(cacheKey, out List<ProductCategoryDTO>? cached))
        {
            return cached!;
        }

        if (!productRepository.DoesEntityExist(productId))
        {
            throw new KeyNotFoundException($"Product with id {productId} not found.");
        }

        IReadOnlyList<ProductCategoryAssignment> assignments = await assignmentRepository.GetForProductAsync(productId, ct);

        var result = assignments
            .Select(a => new ProductCategoryDTO
            {
                CategoryId = a.ProductCategoryId,
                Name = a.ProductCategory.Name,
                IsPrimary = a.IsPrimary
            })
            .ToList();

        cache.Set(cacheKey, result, CacheOptions);

        return result;
    }

    public async Task AssignCategoryAsync(int productId, int categoryId, bool isPrimary, CancellationToken ct)
    {
        if (!productRepository.DoesEntityExist(productId))
        {
            throw new KeyNotFoundException($"Product with id {productId} not found.");
        }

        if (!productCategoryRepository.DoesEntityExist(categoryId))
        {
            throw new KeyNotFoundException($"Category with id {categoryId} not found.");
        }

        if (await assignmentRepository.ExistsAsync(productId, categoryId, ct))
        {
            throw new InvalidOperationException("This category is already assigned to the product.");
        }

        IReadOnlyList<ProductCategoryAssignment> existingAssignments = await assignmentRepository.GetForProductAsync(productId, ct);

        if (!existingAssignments.Any())
        {
            isPrimary = true;
        }

        if (isPrimary)
        {
            ProductCategoryAssignment? currentPrimary = await assignmentRepository.GetPrimaryForProductAsync(productId, ct);

            if (currentPrimary != null)
            {
                currentPrimary.IsPrimary = false;
                await assignmentRepository.SaveChangesAsync(ct);
            }
        }

        var assignment = new ProductCategoryAssignment
        {
            ProductId = productId,
            ProductCategoryId = categoryId,
            IsPrimary = isPrimary
        };

        await assignmentRepository.AddAsync(assignment, ct);

        cache.Remove(CacheKeyForProduct(productId));
    }

    public async Task RemoveCategoryAsync(int productId, int categoryId, CancellationToken ct)
    {
        ProductCategoryAssignment? assignment = await assignmentRepository.GetAsync(productId, categoryId, ct);

        if (assignment is null)
        {
            throw new KeyNotFoundException("The category is not assigned to this product.");
        }

        IReadOnlyList<ProductCategoryAssignment> assignments = await assignmentRepository.GetForProductAsync(productId, ct);

        if (assignments.Count == 1)
        {
            throw new InvalidOperationException("A product must have at least one category.");
        }

        if (assignment.IsPrimary)
        {
            throw new InvalidOperationException("Cannot remove the primary category. Assign another primary category first.");
        }

        await assignmentRepository.RemoveAsync(assignment, ct);

        cache.Remove(CacheKeyForProduct(productId));
    }
}

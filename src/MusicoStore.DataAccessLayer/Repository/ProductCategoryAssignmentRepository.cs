using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductCategoryAssignmentRepository(AppDbContext db) : IProductCategoryAssignmentRepository
{
    public async Task<IReadOnlyList<ProductCategoryAssignment>> GetForProductAsync(
        int productId,
        CancellationToken ct)
        => await db.ProductCategoryAssignments
            .Include(a => a.ProductCategory)
            .Where(a => a.ProductId == productId)
            .ToListAsync(ct);

    public Task<ProductCategoryAssignment?> GetAsync(
        int productId,
        int categoryId,
        CancellationToken ct)
        => db.ProductCategoryAssignments
            .Include(a => a.ProductCategory)
            .FirstOrDefaultAsync(a =>
                a.ProductId == productId &&
                a.ProductCategoryId == categoryId,
                ct);

    public Task<ProductCategoryAssignment?> GetPrimaryForProductAsync(
        int productId,
        CancellationToken ct)
        => db.ProductCategoryAssignments
            .Include(a => a.ProductCategory)
            .FirstOrDefaultAsync(a =>
                a.ProductId == productId &&
                a.IsPrimary,
                ct);

    public Task<bool> ExistsAsync(
        int productId,
        int categoryId,
        CancellationToken ct)
        => db.ProductCategoryAssignments
            .AnyAsync(a =>
                a.ProductId == productId &&
                a.ProductCategoryId == categoryId,
                ct);

    public async Task AddAsync(
        ProductCategoryAssignment assignment,
        CancellationToken ct)
    {
        db.ProductCategoryAssignments.Add(assignment);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(
        ProductCategoryAssignment assignment,
        CancellationToken ct)
    {
        db.ProductCategoryAssignments.Remove(assignment);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => db.SaveChangesAsync(ct);
}

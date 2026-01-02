using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Records;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductRepository(AppDbContext db) : GenericRepository<Product>(db), IProductRepository
{
    public override async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await Db.Products
            .Include(p => p.CategoryAssignments)
                .ThenInclude(a => a.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Products
            .Include(p => p.CategoryAssignments)
                .ThenInclude(a => a.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product?> GetByIdWithoutLogsAsync(int id, CancellationToken ct)
        => await Db.Products
            .Include(p => p.CategoryAssignments)
                .ThenInclude(a => a.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> FilterAsync(ProductFilterCriteria filter, CancellationToken ct)
    {
        IQueryable<Product> query = GetBaseQueryWithIncludes();
        query = ApplyProductFilters(query, filter);

         return await query.AsNoTracking().ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> FilterPagedAsync(ProductFilterCriteria filter, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Product> query = GetBaseQueryWithIncludes();
        query = ApplyProductFilters(query, filter);

         int totalCount = await query.CountAsync(ct);
         int skip = (page - 1) * pageSize;

        IReadOnlyList<Product> items = await query
            .AsNoTracking()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

         return (items, totalCount); 
    }

    private IQueryable<Product> ApplyProductFilters(IQueryable<Product> query, ProductFilterCriteria filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
        {
            string nameLower = filter.Name.ToLower();
            query = query.Where(p => p.Name != null && p.Name.ToLower().Contains(nameLower));
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            string descLower = filter.Description.ToLower();
            query = query.Where(p => p.Description != null && p.Description.ToLower().Contains(descLower));
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.CurrentPrice <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrEmpty(filter.Category))
        {
            string categoryLower = filter.Category.ToLower();
            query = query.Where(p =>
                p.CategoryAssignments != null &&
                p.CategoryAssignments.Any(a =>
                    a.ProductCategory != null &&
                    a.ProductCategory.Name != null &&
                    a.ProductCategory.Name.ToLower().Contains(categoryLower))); 
        }

        if (!string.IsNullOrEmpty(filter.Manufacturer))
        {
            string manufacturerLower = filter.Manufacturer.ToLower();
            query = query.Where(p =>
                p.Manufacturer != null &&
                p.Manufacturer.Name != null &&
                p.Manufacturer.Name.ToLower().Contains(manufacturerLower)); 
        }

        return query;
    }

    private IQueryable<Product> GetBaseQueryWithIncludes()
    {
        return Db.Products
            .Include(p => p.CategoryAssignments)
                .ThenInclude(a => a.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer);
    }
}

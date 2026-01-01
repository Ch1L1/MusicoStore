using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;
using MusicoStore.Domain.Interfaces.Repository;
using MusicoStore.Domain.Records;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductRepository(AppDbContext db) : GenericRepository<Product>(db), IProductRepository
{
    public override async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
        => await Db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => Db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product?> GetByIdWithoutLogsAsync(int id, CancellationToken ct)
        => await Db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> FilterAsync(ProductFilterCriteria filter, CancellationToken ct)
    {
        IQueryable<Product> query = Db.Products
            .Include(p => p.ProductCategory)
            .Include(p => p.Manufacturer)
            .Include(p => p.EditLogs)
                .ThenInclude(l => l.Customer)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            string nameLower = filter.Name.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(nameLower));
        }

        if (!string.IsNullOrEmpty(filter.Description))
        {
            string descLower = filter.Description.ToLower();
            query = query.Where(p => p.Description.ToLower().Contains(descLower));
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.CurrentPrice <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrEmpty(filter.Category))
        {
            string categoryLower = filter.Category.ToLower();
            query = query.Where(p => p.ProductCategory!.Name.ToLower() == categoryLower);
        }

        if (!string.IsNullOrEmpty(filter.Manufacturer))
        {
            string manufacturerLower = filter.Manufacturer.ToLower();
            query = query.Where(p => p.Manufacturer!.Name.ToLower() == manufacturerLower);
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }
}

using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer.Repository;

public class ProductCategoryRepository(AppDbContext db) : GenericRepository<ProductCategory>(db)
{
    public override async Task<IReadOnlyList<ProductCategory>> GetAllAsync(CancellationToken ct)
        => await Db.ProductCategories
            .Include(c => c.CategoryAssignments)
                .ThenInclude(pca => pca.Product)
                    .ThenInclude(p => p.Manufacturer)
            .AsNoTracking()
            .ToListAsync(ct);

    public override Task<ProductCategory?> GetByIdAsync(int id, CancellationToken ct)
        => Db.ProductCategories
            .Include(c => c.CategoryAssignments)
                .ThenInclude(pca => pca.Product)
                    .ThenInclude(p => p.Manufacturer)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
}

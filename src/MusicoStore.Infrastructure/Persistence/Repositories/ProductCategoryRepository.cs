using Microsoft.EntityFrameworkCore;
using MusicoStore.Application.Abstractions.Interfaces;
using MusicoStore.Domain.Entities;

namespace MusicoStore.Infrastructure.Persistence.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _context;

    public ProductCategoryRepository(AppDbContext db)
    {
        _context = db;
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        => await _context.ProductCategories.Include(c => c.Products).ToListAsync();

    public async Task<ProductCategory?> GetByIdAsync(int id)
        => await _context.ProductCategories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<ProductCategory> AddAsync(ProductCategory category)
    {
        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<ProductCategory> UpdateAsync(ProductCategory category)
    {
        _context.ProductCategories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ProductCategories.FindAsync(id);
        if (entity != null)
        {
            _context.ProductCategories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

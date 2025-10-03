using Microsoft.EntityFrameworkCore;
using MusicoStore.Domain.Entities;

namespace MusicoStore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.ToTable("Product");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).UseIdentityColumn();
            e.Property(p => p.Name).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(300).IsRequired();
            e.Property(p => p.CurrentPrice).HasColumnType("decimal(12,2)").IsRequired();
            e.Property(p => p.CurrencyCode).HasMaxLength(3).IsRequired().HasDefaultValue("USD");
            e.Property(p => p.ProductCategoryId).IsRequired();
        });

        b.Entity<ProductCategory>(e =>
        {
            e.ToTable("ProductCategory");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(50).IsRequired();
        });
    }
}

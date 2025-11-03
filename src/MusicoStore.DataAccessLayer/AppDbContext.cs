using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.DataAccessLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }

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
            e.Property(p => p.CurrencyCode).HasConversion<String>().HasMaxLength(3).IsRequired().HasDefaultValue(Currency.USD);
            e.HasOne(p => p.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Manufacturer)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<ProductCategory>(e =>
        {
            e.ToTable("ProductCategory");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(50).IsRequired();
        });



        b.Entity<Address>(e =>
        {
            e.ToTable("Address");
            e.HasKey(a => a.Id);
            e.Property(a => a.StreetName).HasMaxLength(50).IsRequired();
            e.Property(a => a.StreetNumber).HasMaxLength(20).IsRequired();
            e.Property(a => a.City).HasMaxLength(50).IsRequired();
            e.Property(a => a.PostalNumber).HasMaxLength(6).IsRequired();
            e.Property(a => a.CountryCode).HasMaxLength(3).IsRequired();
        });

        b.Entity<Manufacturer>(e =>
        {
            e.ToTable("Manufacturer");
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).HasMaxLength(50).IsRequired();
        });
    }
}

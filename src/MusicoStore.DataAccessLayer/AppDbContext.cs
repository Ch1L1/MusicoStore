using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer.Enums;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<LocalIdentityUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductEditLog> ProductEditLogs { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderedProduct> OrderedProducts { get; set; }
    public DbSet<OrderStatusLog> OrderStatusLogs { get; set; }
    public DbSet<OrderState> OrderStates { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Product>(e =>
        {
            e.ToTable("Product");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).UseIdentityColumn();
            e.Property(p => p.Name).HasMaxLength(50).IsRequired();
            e.Property(p => p.Description).HasMaxLength(300).IsRequired();
            e.Property(p => p.CurrentPrice).HasColumnType("decimal(12,2)").IsRequired();
            e.Property(p => p.CurrencyCode).HasConversion<String>().HasMaxLength(3).IsRequired()
                .HasDefaultValue(Currency.USD);
            e.HasOne(p => p.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Manufacturer)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.Property(p => p.ImagePath)
                .HasMaxLength(260)
                .IsRequired(false);
        });

        b.Entity<ProductCategory>(e =>
        {
            e.ToTable("ProductCategory");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(50).IsRequired();
        });

        b.Entity<ProductEditLog>(e =>
        {
            e.ToTable("ProductEditLog");
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).UseIdentityColumn();

            e.Property(l => l.EditTime).IsRequired();

            e.HasOne(l => l.Product)
                .WithMany(p => p.EditLogs)
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(l => l.Customer)
                .WithMany(c => c.EditedProducts)
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
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

        b.Entity<Storage>(e =>
        {
            e.ToTable("Storage");
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).HasMaxLength(100).IsRequired();
            e.Property(s => s.PhoneNumber).HasMaxLength(20).IsRequired(false);
            e.Property(s => s.Capacity).IsRequired();

            e.HasOne(s => s.Address)
                .WithMany()
                .HasForeignKey(s => s.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(s => s.Stocks)
                .WithOne(st => st.Storage)
                .HasForeignKey(st => st.StorageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Stock>(e =>
        {
            e.ToTable("Stock");
            e.HasKey(s => s.Id);
            e.Property(s => s.CurrentQuantity).IsRequired();

            e.HasIndex(s => new { s.ProductId, s.StorageId })
                .IsUnique();

            e.HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.Storage)
                .WithMany(st => st.Stocks)
                .HasForeignKey(s => s.StorageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Customer>(e =>
        {
            e.ToTable("Customer");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).UseIdentityColumn();
            e.Property(c => c.FirstName).HasMaxLength(50).IsRequired();
            e.Property(c => c.LastName).HasMaxLength(50).IsRequired();
            e.Property(c => c.Email).HasMaxLength(100).IsRequired();
            e.HasIndex(c => c.Email).IsUnique();
            e.Property(c => c.PhoneNumber).HasMaxLength(20).IsRequired(false);
            e.Property(c => c.IsEmployee).IsRequired().HasDefaultValue(false);

            e.HasMany(c => c.Addresses)
                .WithOne(ca => ca.Customer)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<CustomerAddress>(e =>
        {
            e.ToTable("CustomerAddress");
            e.HasKey(ca => ca.Id);
            e.Property(ca => ca.Id).UseIdentityColumn();
            e.Property(ca => ca.IsMainAddress).IsRequired().HasDefaultValue(false);

            e.HasOne(ca => ca.Address)
                .WithMany()
                .HasForeignKey(ca => ca.AddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Order>(e =>
        {
            e.ToTable("Order");
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).UseIdentityColumn();

            e.HasOne(o => o.CustomerAddress)
                .WithMany(ca => ca.Orders)
                .HasForeignKey(o => o.CustomerAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasMany(o => o.OrderedProducts)
                .WithOne(op => op.Order)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(o => o.StatusLog)
                .WithOne(osl => osl.Order)
                .HasForeignKey(osl => osl.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<OrderedProduct>(e =>
        {
            e.ToTable("OrderedProduct");
            e.HasKey(op => op.Id);
            e.Property(op => op.Id).UseIdentityColumn();
            e.Property(op => op.Quantity).IsRequired();
            e.Property(op => op.PricePerItem).HasColumnType("decimal(12,2)").IsRequired();

            e.HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<OrderState>(e =>
        {
            e.ToTable("OrderState");
            e.HasKey(os => os.Id);
            e.Property(os => os.Id).UseIdentityColumn();
            e.Property(os => os.Name).HasMaxLength(50).IsRequired();
        });

        b.Entity<OrderStatusLog>(e =>
        {
            e.ToTable("OrderStatusLog");

            e.HasKey(osl => osl.Id);
            e.Property(osl => osl.Id).UseIdentityColumn();

            e.Property(osl => osl.LogTime).IsRequired();

            e.HasOne(osl => osl.Order)
                .WithMany(o => o.StatusLog)
                .HasForeignKey(osl => osl.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(osl => osl.OrderState)
                .WithMany(os => os.Logs)
                .HasForeignKey(osl => osl.OrderStateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<LocalIdentityUser>(e =>
        {
            e.Property(u => u.CustomerId).IsRequired();
            e.HasOne(u => u.Customer)
                .WithOne()
                .HasForeignKey<LocalIdentityUser>(u => u.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

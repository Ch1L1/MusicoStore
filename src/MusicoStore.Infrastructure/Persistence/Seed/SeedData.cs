using MusicoStore.Domain.Entities;

namespace MusicoStore.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureDeletedAsync(); // temporary for test purpose
        await db.Database.EnsureCreatedAsync();

        if (db.Products.Any()) return;

        // Categories
        var microphones = new ProductCategory { Name = "Microphones" };
        var guitars = new ProductCategory { Name = "Guitars" };
        var dvds = new ProductCategory { Name = "DVDs" };
        db.ProductCategories.AddRange(microphones, guitars, dvds);
        await db.SaveChangesAsync(ct);

        // Products
        db.Products.AddRange(
            new Product { Name = "Studio Microphone", Description = "Cardioid condenser mic", CurrentPrice = 129.99m, ProductCategoryId = microphones.Id },
            new Product { Name = "Dynamic Microphone", Description = "Handheld vocal mic", CurrentPrice = 79.50m, ProductCategoryId = microphones.Id },
            new Product { Name = "Acoustic Guitar", Description = "Spruce top dreadnought", CurrentPrice = 199.00m, ProductCategoryId = guitars.Id },
            new Product { Name = "Jazz DVD Collection", Description = "Classic jazz performances", CurrentPrice = 39.90m, ProductCategoryId = dvds.Id }
        );
        await db.SaveChangesAsync(ct);
    }
}

using MusicoStore.Domain.Entities;

namespace MusicoStore.Infrastructure.Persistence.Seed;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (db.Products.Any()) return;

        db.Products.AddRange(
            new Product { Name = "Studio Microphone", Description = "Cardioid condenser mic", CurrentPrice = 129.99m },
            new Product { Name = "Acoustic Guitar", Description = "Spruce top dreadnought", CurrentPrice = 199.00m },
            new Product { Name = "Jazz DVD Collection", Description = "Classic jazz performances", CurrentPrice = 39.90m }
        );

        await db.SaveChangesAsync(ct);
    }
}

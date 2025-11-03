using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Enums;

namespace MusicoStore.DataAccessLayer;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureDeletedAsync(); // temporary for test purpose
        await db.Database.EnsureCreatedAsync();

        if (db.Products.Any())
        {
            return;
        }

        // Addresses
        var addrUsa = new Address
        {
            StreetName = "Music Ave",
            StreetNumber = "123",
            City = "Nashville",
            PostalNumber = "37011",
            CountryCode = "USA"
        };

        var addrJapan = new Address
        {
            StreetName = "Harmony Rd",
            StreetNumber = "45B",
            City = "Tokyo",
            PostalNumber = "10001",
            CountryCode = "JPN"
        };

        var addrGermany = new Address
        {
            StreetName = "Tonstraße",
            StreetNumber = "12",
            City = "Berlin",
            PostalNumber = "10115",
            CountryCode = "GER"
        };

        db.Addresses.AddRange(addrUsa, addrJapan, addrGermany);
        await db.SaveChangesAsync(ct);

        // Manufacturers
        var shure = new Manufacturer
        {
            Name = "Shure",
            AddressId = addrUsa.Id
        };
        var yamaha = new Manufacturer
        {
            Name = "Yamaha",
            AddressId = addrJapan.Id
        };
        var neumann = new Manufacturer
        {
            Name = "Neumann",
            AddressId = addrGermany.Id
        };

        db.Manufacturers.AddRange(shure, yamaha, neumann);
        await db.SaveChangesAsync(ct);

        // Categories
        var microphones = new ProductCategory { Name = "Microphones" };
        var guitars = new ProductCategory { Name = "Guitars" };
        var dvds = new ProductCategory { Name = "DVDs" };

        db.ProductCategories.AddRange(microphones, guitars, dvds);
        await db.SaveChangesAsync(ct);

        // Products
        db.Products.AddRange(
            new Product
            {
                Name = "Studio Microphone",
                Description = "Cardioid condenser mic",
                CurrentPrice = 129.99m,
                CurrencyCode = Currency.USD,
                ProductCategoryId = microphones.Id,
                ManufacturerId = shure.Id
            },
            new Product
            {
                Name = "Dynamic Microphone",
                Description = "Handheld vocal mic",
                CurrentPrice = 79.50m,
                CurrencyCode = Currency.USD,
                ProductCategoryId = microphones.Id,
                ManufacturerId = shure.Id
            },
            new Product
            {
                Name = "Acoustic Guitar",
                Description = "Spruce top dreadnought",
                CurrentPrice = 199.00m,
                CurrencyCode = Currency.USD,
                ProductCategoryId = guitars.Id,
                ManufacturerId = yamaha.Id
            },
            new Product
            {
                Name = "Jazz DVD Collection",
                Description = "Classic jazz performances",
                CurrentPrice = 39.90m,
                CurrencyCode = Currency.USD,
                ProductCategoryId = dvds.Id,
                ManufacturerId = neumann.Id
            }
        );

        await db.SaveChangesAsync(ct);
    }
}


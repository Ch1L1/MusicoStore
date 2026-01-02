using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MusicoStore.DataAccessLayer.Enums;
using MusicoStore.DataAccessLayer.Identity;
using MusicoStore.Domain.Entities;

namespace MusicoStore.DataAccessLayer;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db, CancellationToken ct = default)
    {

        if (db.Products.Any())
        {
            return;
        }

        // Make Bogus deterministic (optional)
        Randomizer.Seed = new Random(12345);
        var rng = new Randomizer();

        await SeedOrderStatesAsync(db, ct);
        var addresses = await SeedAddressesAsync(db, ct);
        var manufacturers = await SeedManufacturersAsync(db, addresses, ct);
        var categories = await SeedProductCategoriesAsync(db, ct);
        var products = await SeedProductsAsync(db, categories, manufacturers, ct);
        await SeedProductCategoryAssignmentsAsync(db, products, categories, ct);
        var (customers, customerAddresses) = await SeedCustomersAndAddressesAsync(db, addresses, ct);
        await EnsureMainAddressPerCustomerAsync(db, customers, customerAddresses, ct);
        await SeedOrdersAsync(db, customers, customerAddresses, products, rng, ct);
        var giftCards = await SeedGiftCardsAsync(db, ct);
        await SeedGiftCardCouponsAsync(db, giftCards, ct);
    }

    private static async Task SeedOrderStatesAsync(AppDbContext db, CancellationToken ct)
    {
        if (!db.OrderStates.Any())
        {
            var states = new[]
            {
                new OrderState { Name = "Created" },
                new OrderState { Name = "Paid" },
                new OrderState { Name = "Shipped" },
                new OrderState { Name = "Delivered" },
                new OrderState { Name = "Cancelled" }
            };

            db.OrderStates.AddRange(states);
            await db.SaveChangesAsync(ct);
        }
    }

    private static async Task<List<Address>> SeedAddressesAsync(AppDbContext db, CancellationToken ct)
    {
        var addressFaker = new Faker<Address>("en")
            .RuleFor(a => a.StreetName, f => f.Address.StreetName())
            .RuleFor(a => a.StreetNumber, f => f.Address.BuildingNumber())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.CountryCode, f => f.Address.CountryCode())
            .RuleFor(a => a.PostalNumber, f =>
            {
                var cleaned = new string(f.Address.ZipCode()
                    .Where(char.IsLetterOrDigit)
                    .ToArray());
                return cleaned.Length <= 6
                    ? cleaned
                    : cleaned[..6];
            });

        var addresses = addressFaker.Generate(10);
        db.Addresses.AddRange(addresses);
        await db.SaveChangesAsync(ct);
        return addresses;
    }

    private static async Task<List<Manufacturer>> SeedManufacturersAsync(AppDbContext db, List<Address> addresses, CancellationToken ct)
    {
        var manufacturerFaker = new Faker<Manufacturer>("en")
            .RuleFor(m => m.Name, f => f.Company.CompanyName())
            .RuleFor(m => m.AddressId, f => f.PickRandom(addresses).Id);

        var manufacturers = manufacturerFaker.Generate(5);
        db.Manufacturers.AddRange(manufacturers);
        await db.SaveChangesAsync(ct);
        return manufacturers;
    }

    private static async Task<List<ProductCategory>> SeedProductCategoriesAsync(AppDbContext db, CancellationToken ct)
    {
        var categories = new[]
        {
            new ProductCategory { Name = "Microphones" },
            new ProductCategory { Name = "Guitars" },
            new ProductCategory { Name = "Accessories" }
        };

        db.ProductCategories.AddRange(categories);
        await db.SaveChangesAsync(ct);
        return categories.ToList();
    }

    private static async Task<List<Product>> SeedProductsAsync(AppDbContext db, List<ProductCategory> categories, List<Manufacturer> manufacturers, CancellationToken ct)
    {
        var productFaker = new Faker<Product>("en")
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(8))
            .RuleFor(p => p.CurrentPrice, f => f.Random.Decimal(20m, 1000m))
            .RuleFor(p => p.CurrencyCode, f => Currency.USD)
            .RuleFor(p => p.ManufacturerId, f => f.PickRandom(manufacturers).Id)
            .RuleFor(p => p.ImagePath, f => (string?)null);

        var products = productFaker.Generate(40);
        db.Products.AddRange(products);
        await db.SaveChangesAsync(ct);
        return products;
    }

    private static async Task SeedProductCategoryAssignmentsAsync(AppDbContext db, List<Product> products, List<ProductCategory> categories, CancellationToken ct)
    {
        var rng = new Random();
        var assignments = new List<ProductCategoryAssignment>();

        foreach (var product in products)
        {
            int categoryCount = rng.Next(1, Math.Min(4, categories.Count));
            var selectedCategories = categories
                .OrderBy(_ => rng.Next())
                .Take(categoryCount)
                .ToList();

            bool primaryAssigned = false;
            foreach (var category in selectedCategories)
            {
                bool isPrimary = !primaryAssigned;
                assignments.Add(new ProductCategoryAssignment
                {
                    ProductId = product.Id,
                    ProductCategoryId = category.Id,
                    IsPrimary = isPrimary
                });
                primaryAssigned = true;
            }
        }

        db.ProductCategoryAssignments.AddRange(assignments);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<(List<Customer> customers, List<CustomerAddress> customerAddresses)> SeedCustomersAndAddressesAsync(
        AppDbContext db,
        List<Address> addresses,
        CancellationToken ct)
    {
        var customerFaker = new Faker<Customer>("en")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.IsEmployee, f => f.Random.Bool(0.1f))
            .RuleFor(c => c.PhoneNumber, f =>
            {
                var cleaned = new string(f.Phone.PhoneNumber()
                    .Where(ch => char.IsDigit(ch) || ch is '+' or ' ' or '-' or '(' or ')')
                    .ToArray());

                return cleaned.Length <= 20
                    ? cleaned
                    : cleaned[..20];
            });

        var customers = customerFaker.Generate(10);
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync(ct);

        var customerAddresses = new List<CustomerAddress>();
        var rng = new Random();

        foreach (var customer in customers)
        {
            var mainAddress = new CustomerAddress
            {
                CustomerId = customer.Id,
                AddressId = addresses[rng.Next(addresses.Count)].Id,
                IsMainAddress = true
            };

            customerAddresses.Add(mainAddress);

            int extraCount = rng.Next(0, 3);
            for (int i = 0; i < extraCount; i++)
            {
                customerAddresses.Add(new CustomerAddress
                {
                    CustomerId = customer.Id,
                    AddressId = addresses[rng.Next(addresses.Count)].Id,
                    IsMainAddress = false
                });
            }
        }

        db.CustomerAddresses.AddRange(customerAddresses);
        await db.SaveChangesAsync(ct);

        return (customers, customerAddresses);
    }

    private static async Task EnsureMainAddressPerCustomerAsync(AppDbContext db, List<Customer> customers, List<CustomerAddress> customerAddresses, CancellationToken ct)
    {
        foreach (var customer in customers)
        {
            if (!customerAddresses.Any(ca => ca.CustomerId == customer.Id && ca.IsMainAddress))
            {
                var address = customerAddresses.First(ca => ca.CustomerId == customer.Id);
                address.IsMainAddress = true;
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedOrdersAsync(AppDbContext db, List<Customer> customers, List<CustomerAddress> customerAddresses, List<Product> products, Randomizer rng, CancellationToken ct)
    {
        var createdState = db.OrderStates.First(s => s.Name == "Created");
        var paidState = db.OrderStates.First(s => s.Name == "Paid");
        var shippedState = db.OrderStates.First(s => s.Name == "Shipped");

        var orderFaker = new Faker<Order>("en")
            .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).Id)
            .RuleFor(o => o.CustomerAddressId, f =>
            {
                var custId = f.PickRandom(customers).Id;
                var addr = customerAddresses.First(ca => ca.CustomerId == custId);
                return addr.Id;
            });

        var orders = orderFaker.Generate(20);
        db.Orders.AddRange(orders);
        await db.SaveChangesAsync(ct);

        var orderedProductFaker = new Faker<OrderedProduct>("en")
            .RuleFor(op => op.ProductId, f => f.PickRandom(products).Id)
            .RuleFor(op => op.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(op => op.PricePerItem, (f, op) =>
            {
                var prod = products.First(p => p.Id == op.ProductId);
                return prod.CurrentPrice;
            });

        var statusLogFaker = new Faker<OrderStatusLog>("en");

        var allOrderedProducts = new List<OrderedProduct>();
        var allLogs = new List<OrderStatusLog>();

        foreach (var order in orders)
        {
            int itemCount = new Random().Next(1, 5);
            var orderItems = orderedProductFaker.Generate(itemCount);
            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
            }

            allOrderedProducts.AddRange(orderItems);

            allLogs.Add(new OrderStatusLog
            {
                OrderId = order.Id,
                OrderStateId = createdState.Id,
                LogTime = DateTime.UtcNow.AddMinutes(-60)
            });

            if (rng.Bool(0.7f))
            {
                allLogs.Add(new OrderStatusLog
                {
                    OrderId = order.Id,
                    OrderStateId = paidState.Id,
                    LogTime = DateTime.UtcNow.AddMinutes(-30)
                });
            }

            if (rng.Bool(0.4f))
            {
                allLogs.Add(new OrderStatusLog
                {
                    OrderId = order.Id,
                    OrderStateId = shippedState.Id,
                    LogTime = DateTime.UtcNow.AddMinutes(-10)
                });
            }
        }

        db.OrderedProducts.AddRange(allOrderedProducts);
        db.OrderStatusLogs.AddRange(allLogs);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<List<GiftCard>> SeedGiftCardsAsync(AppDbContext db, CancellationToken ct)
    {
        var giftCards = new List<GiftCard>
    {
        new GiftCard
        {
            Amount = 200,
            CurrencyCode = Currency.CZK,
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ValidTo = DateTime.UtcNow.AddDays(30)
        },
        new GiftCard
        {
            Amount = 500,
            CurrencyCode = Currency.CZK,
            ValidFrom = DateTime.UtcNow,
            ValidTo = DateTime.UtcNow.AddMonths(2)
        }
    };

        db.GiftCards.AddRange(giftCards);
        await db.SaveChangesAsync(ct);
        return giftCards;
    }
    private static async Task SeedGiftCardCouponsAsync(AppDbContext db, List<GiftCard> giftCards, CancellationToken ct)
    {
        var faker = new Faker();

        var coupons = new List<GiftCardCoupon>();

        foreach (var giftCard in giftCards)
        {
            for (int i = 0; i < 5; i++)
            {
                coupons.Add(new GiftCardCoupon
                {
                    GiftCardId = giftCard.Id,
                    CouponCode = faker.Random.AlphaNumeric(10).ToUpper()
                });
            }
        }

        db.GiftCardCoupons.AddRange(coupons);
        await db.SaveChangesAsync(ct);
    }
    public static async Task SeedTestLoginWithOrdersAsync(IServiceProvider services, AppDbContext db)
    {
        var userManager = services.GetRequiredService<UserManager<LocalIdentityUser>>();

        var customerIdWithOrders = db.Orders
            .Select(o => o.CustomerId)
            .First(); 

        const string email = "test@test.com";
        const string password = "Test123!";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            existing.CustomerId = customerIdWithOrders;
            await userManager.UpdateAsync(existing);
            return;
        }

        var user = new LocalIdentityUser
        {
            UserName = email,
            Email = email,
            CustomerId = customerIdWithOrders
        };

        await userManager.CreateAsync(user, password);
    }

    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Employee"))
        {
            await roleManager.CreateAsync(new IdentityRole("Employee"));
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }
    }
}

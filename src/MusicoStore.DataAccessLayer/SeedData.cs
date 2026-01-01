using Bogus;
using Microsoft.EntityFrameworkCore;
using MusicoStore.DataAccessLayer.Enums;
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
        var (customers, customerAddresses) = await SeedCustomersAndAddressesAsync(db, addresses, ct);
        await EnsureMainAddressPerCustomerAsync(db, customers, customerAddresses, ct);
        await SeedOrdersAsync(db, customers, customerAddresses, products, rng, ct);
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
            .RuleFor(p => p.ProductCategoryId, f => f.PickRandom(categories).Id)
            .RuleFor(p => p.ManufacturerId, f => f.PickRandom(manufacturers).Id)
            .RuleFor(p => p.ImagePath, f => (string?)null);

        var products = productFaker.Generate(40);
        db.Products.AddRange(products);
        await db.SaveChangesAsync(ct);
        return products;
    }

    private static async Task<(List<Customer> customers, List<CustomerAddress> customerAddresses)> SeedCustomersAndAddressesAsync(AppDbContext db, List<Address> addresses, CancellationToken ct)
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

                // ensure max 20 characters to satisfy HasMaxLength(20)
                return cleaned.Length <= 20
                    ? cleaned
                    : cleaned[..20];
            });

        var customers = customerFaker.Generate(10);
        db.Customers.AddRange(customers);
        await db.SaveChangesAsync(ct);

        var customerAddressFaker = new Faker<CustomerAddress>("en")
            .RuleFor(ca => ca.CustomerId, f => f.PickRandom(customers).Id)
            .RuleFor(ca => ca.AddressId, f => f.PickRandom(addresses).Id)
            .RuleFor(ca => ca.IsMainAddress, f => f.Random.Bool(0.7f));

        var customerAddresses = customerAddressFaker.Generate(20);
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
}

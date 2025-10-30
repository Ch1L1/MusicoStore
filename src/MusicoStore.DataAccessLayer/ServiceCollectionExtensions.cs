using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicoStore.DataAccessLayer.Abstractions;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Repository;

namespace MusicoStore.DataAccessLayer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration config)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var connStr = isWindows
            ? config.GetConnectionString("DefaultConnection")
            : config.GetConnectionString("LocalMacOSConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connStr));

        // Repositories
        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IRepository<ProductCategory>, ProductCategoryRepository>();
        services.AddScoped<IRepository<Address>, AddressRepository>();
        services.AddScoped<IRepository<Manufacturer>, ManufacturerRepository>();

        services.AddScoped<ProductRepository>();

        return services;
    }
}

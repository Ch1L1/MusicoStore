using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.Infrastructure.Repository;

namespace MusicoStore.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var connStr = isWindows
            ? config.GetConnectionString("DefaultConnection")
            : config.GetConnectionString("LocalMacOSConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connStr));

        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<ProductRepository, ProductRepository>();
        services.AddScoped<IRepository<ProductCategory>, ProductCategoryRepository>();
        services.AddScoped<IRepository<Address>, AddressRepository>();
        services.AddScoped<IRepository<Manufacturer>, ManufacturerRepository>();
        return services;
    }
}

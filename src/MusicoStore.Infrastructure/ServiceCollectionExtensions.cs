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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IRepository<ProductCategory>, ProductCategoryRepository>();
        return services;
    }
}

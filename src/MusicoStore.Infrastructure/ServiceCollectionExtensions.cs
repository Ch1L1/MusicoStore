using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicoStore.Application.Interfaces.IRepositories;
using MusicoStore.Infrastructure.Persistence;
using MusicoStore.Infrastructure.Persistence.Repositories;

namespace MusicoStore.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}

using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}

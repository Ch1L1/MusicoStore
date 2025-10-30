// This file is no longer used. The DI registration has moved to MusicoStore.DataAccessLayer.
// Keeping the file for now to avoid breaking references during incremental work. Consider deleting the Infrastructure project.
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicoStore.DataAccessLayer;
using MusicoStore.DataAccessLayer.Entities;
using MusicoStore.DataAccessLayer.Repository;

namespace MusicoStore.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Redirect to DAL extension for compatibility
        return MusicoStore.DataAccessLayer.ServiceCollectionExtensions.AddDataAccessLayer(services, config);
    }
}

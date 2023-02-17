using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Catalog.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkSqlServer()
            .AddDbContext<CatalogContext>(
                options =>
                {
                    options.UseSqlServer(
                        configuration["ConnectionString"],
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 15,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        });
                });

        return services;
    }

    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CatalogSettings>(configuration);
        return services;
    }
}

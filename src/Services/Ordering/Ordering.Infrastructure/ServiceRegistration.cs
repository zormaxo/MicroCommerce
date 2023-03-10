using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Interfaces.Repositories;
using Ordering.Infrastructure.Context;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(
            opt =>
            {
                opt.UseSqlServer(configuration["OrderDbConnectionString"]);
                opt.EnableSensitiveDataLogging();
            });

        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(configuration["OrderDbConnectionString"]);

        using var dbContext = new OrderDbContext(optionsBuilder.Options, null);
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();

        return services;
    }
}

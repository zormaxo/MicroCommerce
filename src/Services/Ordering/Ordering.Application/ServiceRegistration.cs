using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Ordering.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, Type startup)
    {
        var assm = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceRegistration).Assembly));
        services.AddAutoMapper(assm);

        return services;
    }
}

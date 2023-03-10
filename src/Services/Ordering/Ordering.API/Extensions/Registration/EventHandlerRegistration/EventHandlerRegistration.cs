using Microsoft.Extensions.DependencyInjection;
using Ordering.API.IntegrationEvents.EventHandlers;

namespace Ordering.API.Extensions.Registration.EventHandlerRegistration;

public static class EventHandlerRegistration
{
    public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<OrderCreatedIntegrationEventHandler>();

        return services;
    }
}

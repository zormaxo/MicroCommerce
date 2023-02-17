using EventBus;
using EventBus.Abstraction;

namespace EventBusFactory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        return config.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus.EventBusServiceBus(config, serviceProvider),
            _ => new EventBusRabbitMQ.EventBusRabbitMQ(config, serviceProvider),
        };
    }
}

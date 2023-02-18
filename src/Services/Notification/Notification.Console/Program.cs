using EventBus;
using EventBus.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notification.Console.IntegrationEvents.EventHandlers;
using Notification.Console.IntegrationEvents.Events;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
ConfigureServices(services);

var sp = services.BuildServiceProvider();
IEventBus eventBus = sp.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

Console.WriteLine("Application is Running....");
Console.ReadLine();

static void ConfigureServices(ServiceCollection services)
{
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();
    services.AddLogging(c => c.AddConsole());

    services.AddSingleton(
        sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "NotificationService",
                EventBusType = EventBusType.RabbitMQ,
            };

            return EventBusFactory.EventBusFactory.Create(config, sp);
        });
}
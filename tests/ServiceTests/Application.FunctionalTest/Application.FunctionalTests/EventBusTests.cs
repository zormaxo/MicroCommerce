using Application.FunctionalTests.Events.EventHandlers;
using Application.FunctionalTests.Events.Events;
using EventBus;
using EventBus.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.FunctionalTests;

public class EventBusTests
{
    public EventBusTests()
    {
        services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
    }

    private readonly ServiceCollection services;

    [Test]
    public void Subscribe_Event_On_Rabbitmq_Test()
    {
        services.AddSingleton<IEventBus>(
            sp =>
            {
                return EventBusFactory.EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.Unsubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [Test]
    public void Subscribe_Event_On_Azure_Test()
    {
        services.AddSingleton<IEventBus>(
            sp =>
            {
                return EventBusFactory.EventBusFactory.Create(GetAzureConfig(), sp);
            });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.Unsubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        Task.Delay(2000).Wait();
    }

    [Test]
    public void send_message_to_rabbitmq_test()
    {
        services.AddSingleton<IEventBus>(
            sp =>
            {
                return EventBusFactory.EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    [Test]
    public void send_message_to_azure_test()
    {
        services.AddSingleton<IEventBus>(
            sp =>
            {
                return EventBusFactory.EventBusFactory.Create(GetAzureConfig(), sp);
            });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    private EventBusConfig GetAzureConfig()
    {
        return new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UnitTest",
            DefaultTopicName = "EcommerceTopicName",
            EventBusType = EventBusType.AzureServiceBus,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString =
                "Endpoint=sb://max-commerce.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pMyh/cqaGio3Cnz+4tv2hh9HweH3i+I8F+ASbAxrd3w="
        };
    }

    private EventBusConfig GetRabbitMQConfig()
    {
        return new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UnitTest",
            DefaultTopicName = "EcommerceTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            //Connection = new ConnectionFactory()
            //{
            //    HostName = "localhost",
            //    Port = 5672,
            //    UserName = "guest",
            //    Password = "guest"
            //}
        };
    }
}
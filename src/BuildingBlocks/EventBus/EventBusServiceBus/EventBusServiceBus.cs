﻿using EventBus;
using EventBus.Event;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EventBusServiceBus;

public class EventBusServiceBus : BaseEventBus
{
    private ITopicClient _topicClient;
    private readonly ManagementClient _managementClient;
    private readonly ILogger _logger;

    public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
    {
        _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
        _managementClient = new ManagementClient(config.EventBusConnectionString);
        _topicClient = CreateTopicClient();
    }


    private ITopicClient CreateTopicClient()
    {
        if (_topicClient?.IsClosedOrClosing != false)
        {
            _topicClient = new TopicClient(
                EventBusConfig.EventBusConnectionString,
                EventBusConfig.DefaultTopicName,
                RetryPolicy.Default);
        }

        // Ensure the topic is created
        if (!_managementClient.TopicExistsAsync(_topicClient.TopicName).GetAwaiter().GetResult())
            _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();

        return _topicClient;
    }

    public override void Publish(IntegrationEvent @event)
    {
        var eventName = @event.GetType().Name; // example: OrderCreatedIntegrationEvent
        eventName = ProcessEventName(eventName); // example: OrderCreated;

        var eventStr = JsonConvert.SerializeObject(@event);
        var bodyArr = Encoding.UTF8.GetBytes(eventStr);

        var message = new Message { MessageId = Guid.NewGuid().ToString(), Body = bodyArr, Label = eventName };

        _topicClient.SendAsync(message).Wait();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubsManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptionClient = CreateSubscriptionClientIfNotExists(eventName);

            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }

        _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

        SubsManager.AddSubscription<T, TH>();
    }

    public override void Unsubscribe<T, TH>()
    {
        var eventName = typeof(T).Name;

        try
        {
            // Subscription will be there but we don't subscribe
            var subscriptionClient = CreateSubscriptionClient(eventName);

            subscriptionClient
                .RemoveRuleAsync(eventName)
                .GetAwaiter()
                .GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("The messaging entity {eventName} Could not be found.", eventName);
        }

        _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

        SubsManager.RemoveSubscription<T, TH>();
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler(
            async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);

                // Complete the message so that it is not received again.
                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                {
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            },
            new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        var ex = exceptionReceivedEventArgs.Exception;
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

        _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

        return Task.CompletedTask;
    }


    private ISubscriptionClient CreateSubscriptionClientIfNotExists(String eventName)
    {
        var subClient = CreateSubscriptionClient(eventName);

        var exists = _managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
            .GetAwaiter()
            .GetResult();

        if (!exists)
        {
            _managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName))
                .GetAwaiter()
                .GetResult();
            RemoveDefaultRule(subClient);
        }

        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);

        return subClient;
    }

    private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExits;

        try
        {
            var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName)
                .GetAwaiter()
                .GetResult();
            ruleExits = rule != null;
        }
        catch (MessagingEntityNotFoundException)
        {
            // Azure Management Client doesn't have RuleExists method
            ruleExits = false;
        }

        if (!ruleExits)
        {
            subscriptionClient.AddRuleAsync(
                new RuleDescription { Filter = new CorrelationFilter { Label = eventName }, Name = eventName })
                .GetAwaiter()
                .GetResult();
        }
    }

    private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient
                .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                .GetAwaiter()
                .GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            _logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.", RuleDescription.DefaultRuleName);
        }
    }

    private SubscriptionClient CreateSubscriptionClient(string eventName)
    {
        return new SubscriptionClient(
            EventBusConfig.EventBusConnectionString,
            EventBusConfig.DefaultTopicName,
            GetSubName(eventName));
    }
}

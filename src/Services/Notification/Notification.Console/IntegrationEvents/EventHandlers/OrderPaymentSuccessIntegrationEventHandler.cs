using EventBus.Abstraction;
using Microsoft.Extensions.Logging;
using Notification.Console.IntegrationEvents.Events;

namespace Notification.Console.IntegrationEvents.EventHandlers;

class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
    { _logger = logger; }

    public Task Handle(OrderPaymentSucceededIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, EMail, Push)

        _logger.LogInformation($"Order Payment Success with OrderId: {@event.OrderId}");

        return Task.CompletedTask;
    }
}

using EventBus.Abstraction;
using Microsoft.Extensions.Logging;
using Notification.Console.IntegrationEvents.Events;

namespace Notification.Console.IntegrationEvents.EventHandlers;

class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
    { _logger = logger; }

    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, EMail, Push)

        _logger.LogInformation($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");

        return Task.CompletedTask;
    }
}

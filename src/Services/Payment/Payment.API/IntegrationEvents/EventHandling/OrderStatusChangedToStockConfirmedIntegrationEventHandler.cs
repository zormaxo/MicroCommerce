using EventBus.Abstraction;
using EventBus.Event;
using Microsoft.Extensions.Options;
using Payment.API.IntegrationEvents.Events;

namespace Payment.API.IntegrationEvents.EventHandling;

public class OrderStatusChangedToStockConfirmedIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    private readonly PaymentSettings _settings;
    private readonly ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> _logger;

    public OrderStatusChangedToStockConfirmedIntegrationEventHandler(
        IEventBus eventBus,
        IOptions<PaymentSettings> settings,
        ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _settings = settings.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogTrace("PaymentSettings: {@PaymentSettings}", _settings);
    }

    public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
    {
        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId}  - ({@IntegrationEvent})",
            @event.Id,
            @event);

        IntegrationEvent orderPaymentIntegrationEvent;

        //Business feature comment:
        // When OrderStatusChangedToStockConfirmed Integration Event is handled.
        // Here we're simulating that we'd be performing the payment against any payment gateway
        // Instead of a real payment we just take the env. var to simulate the payment
        // The payment can be successful or it can fail

        if (_settings.PaymentSucceeded)
        {
            orderPaymentIntegrationEvent = new OrderPaymentSucceededIntegrationEvent(@event.OrderId);
        }
        else
        {
            orderPaymentIntegrationEvent = new OrderPaymentFailedIntegrationEvent(@event.OrderId);
        }

        _logger.LogInformation(
            "----- Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent}) - OrderId:{@event.OrderId}",
            orderPaymentIntegrationEvent.Id,
            orderPaymentIntegrationEvent,
            @event.OrderId);

        _eventBus.Publish(orderPaymentIntegrationEvent);

        await Task.CompletedTask;
    }
}
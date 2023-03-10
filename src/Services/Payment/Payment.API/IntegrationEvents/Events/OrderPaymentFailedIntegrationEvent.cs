using EventBus.Event;

namespace Payment.API.IntegrationEvents.Events;

public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId) => OrderId = orderId;
}

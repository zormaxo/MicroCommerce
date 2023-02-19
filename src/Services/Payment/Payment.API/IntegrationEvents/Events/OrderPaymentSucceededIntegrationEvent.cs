using EventBus.Event;

namespace Payment.API.IntegrationEvents.Events;

public class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSucceededIntegrationEvent(int orderId) => OrderId = orderId;
}

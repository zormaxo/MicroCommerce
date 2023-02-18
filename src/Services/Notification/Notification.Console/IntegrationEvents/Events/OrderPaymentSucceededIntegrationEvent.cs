using EventBus.Event;

namespace Notification.Console.IntegrationEvents.Events;

public class OrderPaymentSucceededIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSucceededIntegrationEvent(int orderId) => OrderId = orderId;
}

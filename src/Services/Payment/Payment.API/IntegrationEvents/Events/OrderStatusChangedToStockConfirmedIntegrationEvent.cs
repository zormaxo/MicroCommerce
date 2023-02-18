using EventBus.Event;

namespace Payment.IntegrationEvents.Events;

public class OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderStatusChangedToStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
}

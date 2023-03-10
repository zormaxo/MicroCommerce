using EventBus.Event;

namespace Payment.API.IntegrationEvents.Events;

public class OrderStatusChangedToStockConfirmedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderStatusChangedToStockConfirmedIntegrationEvent(int orderId) => OrderId = orderId;
}

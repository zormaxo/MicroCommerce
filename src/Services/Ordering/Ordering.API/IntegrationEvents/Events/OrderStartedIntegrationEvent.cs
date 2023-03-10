using EventBus.Event;

namespace Ordering.API.IntegrationEvents.Events;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public OrderStartedIntegrationEvent(string userId, int orderId)
    {
        UserId = userId;
        OrderId = orderId;
    }

    public string UserId { get; private set; }

    public int OrderId { get; private set; }
}

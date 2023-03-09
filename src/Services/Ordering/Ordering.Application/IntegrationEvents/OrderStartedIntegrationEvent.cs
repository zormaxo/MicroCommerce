using EventBus.Event;

namespace Ordering.Application.IntegrationEvents;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public string UserName { get; set; }

    public Guid OrderId { get; set; }

    public OrderStartedIntegrationEvent(string userName, Guid orderId)
    {
        UserName = userName;
        OrderId = orderId;
    }
}

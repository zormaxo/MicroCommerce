using EventBus.Event;

namespace Application.FunctionalTests.Events.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public int Id { get; set; }

        public OrderCreatedIntegrationEvent(int id) { Id = id; }
    }
}

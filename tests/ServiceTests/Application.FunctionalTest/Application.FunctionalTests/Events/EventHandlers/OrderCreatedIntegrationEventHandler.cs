using Application.FunctionalTests.Events.Events;
using EventBus.Abstraction;

namespace Application.FunctionalTests.Events.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event) { return Task.CompletedTask; }
}

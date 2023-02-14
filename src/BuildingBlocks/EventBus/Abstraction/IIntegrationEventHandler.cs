using EventBus.Event;

namespace EventBus.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}

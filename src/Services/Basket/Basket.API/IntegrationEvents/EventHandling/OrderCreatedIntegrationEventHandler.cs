using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using EventBus.Abstraction;

namespace Basket.API.IntegrationEvents.EventHandling;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IBasketRepository _repository;
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(IBasketRepository repository, ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})",
            @event.Id,

            @event);

        await _repository.DeleteBasketAsync(@event.UserId);
    }
}



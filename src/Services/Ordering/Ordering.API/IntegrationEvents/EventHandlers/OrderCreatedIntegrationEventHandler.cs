﻿using EventBus.Abstraction;
using MediatR;
using Ordering.API.IntegrationEvents.Events;
using Ordering.Application.Features.Commands.CreateOrder;

namespace Ordering.API.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator mediator;
    private readonly ILogger<OrderCreatedIntegrationEventHandler> logger;

    public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        try
        {
            logger.LogInformation(
                "Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                @event.Id,
                typeof(Program).Namespace,
                @event);

            var createOrderCommand = new CreateOrderCommand(
                @event.Basket.Items,
                @event.UserId,
                @event.UserName,
                @event.City,
                @event.Street,
                @event.State,
                @event.Country,
                @event.ZipCode,
                @event.CardNumber,
                @event.CardHolderName,
                @event.CardExpiration,
                @event.CardSecurityNumber,
                @event.CardTypeId);

            await mediator.Send(createOrderCommand);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.ToString());
        }
    }
}

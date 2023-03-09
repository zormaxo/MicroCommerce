﻿using MediatR;
using Ordering.Application.Interfaces.Repositories;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Events;

namespace Ordering.Application.DomainEventHandlers;

class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly IBuyerRepository buyerRepository;

    public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository) { this.buyerRepository = buyerRepository; }

    public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken)
    {
        var cardTypeId = orderStartedEvent.CardTypeId != 0 ? orderStartedEvent.CardTypeId : 1;

        var buyer = await buyerRepository.GetSingleAsync(i => i.Name == orderStartedEvent.UserName, i => i.PaymentMethods);

        bool buyerOriginallyExisted = buyer != null;

        if (!buyerOriginallyExisted)
        {
            buyer = new Buyer(orderStartedEvent.UserName);
        }

        buyer.VerifyOrAddPaymentMethod(
            cardTypeId,
            $"Payment Method on {DateTime.UtcNow}",
            orderStartedEvent.CardNumber,
            orderStartedEvent.CardSecurityNumber,
            orderStartedEvent.CardHolderName,
            orderStartedEvent.CardExpiration,
            orderStartedEvent.Order.Id);

        var buyerUpdated = buyerOriginallyExisted ? buyerRepository.Update(buyer) : await buyerRepository.AddAsync(buyer);

        await buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // order status changed event may be fired here
    }
}

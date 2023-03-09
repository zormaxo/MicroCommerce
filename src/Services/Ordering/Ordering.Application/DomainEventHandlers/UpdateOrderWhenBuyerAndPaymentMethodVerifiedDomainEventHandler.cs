using MediatR;
using Ordering.Application.Interfaces.Repositories;
using Ordering.Domain.Events;

namespace Ordering.Application.DomainEventHandlers;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository orderRepository;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(IOrderRepository orderRepository)
    { this.orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository)); }

    public async Task Handle(
        BuyerAndPaymentMethodVerifiedDomainEvent buyerPaymentMethodVerifiedEvent,
        CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetByIdAsync(buyerPaymentMethodVerifiedEvent.OrderId);
        orderToUpdate.SetBuyerId(buyerPaymentMethodVerifiedEvent.Buyer.Id);
        orderToUpdate.SetPaymentMethodId(buyerPaymentMethodVerifiedEvent.Payment.Id);

        // set methods so validate
    }
}

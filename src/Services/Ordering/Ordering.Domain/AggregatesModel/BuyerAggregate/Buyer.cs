using Ordering.Domain.Events;
using Ordering.Domain.SeedWork;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate;

public class Buyer : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    private readonly List<PaymentMethod> _paymentMethods;

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    protected Buyer() { _paymentMethods = new List<PaymentMethod>(); }

    public Buyer(string name) : this() { Name = name ?? throw new ArgumentNullException(nameof(name)); }


    public PaymentMethod VerifyOrAddPaymentMethod(
        int cardTypeId,
        string alias,
        string cardNumber,
        string securityNumber,
        string cardHolderName,
        DateTime expiration,
        Guid orderId)
    {
        var existingPayment = _paymentMethods
            .SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

        if (existingPayment != null)
        {
            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

            return existingPayment;
        }

        var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

        _paymentMethods.Add(payment);

        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

        return payment;
    }
}

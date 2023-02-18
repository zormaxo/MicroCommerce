﻿using EventBus.Event;

namespace Notification.Console.IntegrationEvents.Events;

public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string ErrorMessage { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
    {
        OrderId = orderId;
        ErrorMessage = errorMessage;
    }
}

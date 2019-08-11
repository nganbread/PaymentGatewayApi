using System;

namespace Checkout.PaymentGateway.Service.Eventing
{
    public interface IEvent<in T>
    {
        Guid Id { get; }
        DateTime TimeStamp { get; }
        void Process(T payment);
    }
}
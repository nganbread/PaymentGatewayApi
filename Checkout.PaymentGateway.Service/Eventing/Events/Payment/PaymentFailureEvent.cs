using System;

namespace Checkout.PaymentGateway.Service.Eventing.Events.Payment
{
    public class PaymentFailureEvent : IEvent<Domain.Payment>
    {
        public PaymentFailureEvent(Guid id, DateTime timeStamp, string reason)
        {
            Id = id;
            TimeStamp = timeStamp;
            Reason = reason;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        public string Reason { get; }

        public void Process(Domain.Payment payment) => payment.SetFailed(this);
    }
}
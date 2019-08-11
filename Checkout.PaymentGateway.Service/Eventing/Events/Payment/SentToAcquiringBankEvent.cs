using System;

namespace Checkout.PaymentGateway.Service.Eventing.Events.Payment
{
    public class SentToAcquiringBankEvent : IEvent<Domain.Payment>
    {
        public SentToAcquiringBankEvent(Guid id, DateTime timeStamp)
        {
            Id = id;
            TimeStamp = timeStamp;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }

        public void Process(Domain.Payment payment) => payment.SentToAcquiringBank(this);
    }
}
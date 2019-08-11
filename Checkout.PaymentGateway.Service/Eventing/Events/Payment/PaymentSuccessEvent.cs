using System;

namespace Checkout.PaymentGateway.Service.Eventing.Events.Payment
{
    public class PaymentSuccessEvent : IEvent<Domain.Payment>
    {
        public PaymentSuccessEvent(Guid id, DateTime timeStamp, Guid acquiringBankPaymentId)
        {
            Id = id;
            TimeStamp = timeStamp;
            AcquiringBankPaymentId = acquiringBankPaymentId;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        public Guid AcquiringBankPaymentId { get; }

        public void Process(Domain.Payment payment) => payment.SetSucceeded(this);
    }
}
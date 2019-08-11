using System;
using Checkout.PaymentGateway.Service.Eventing.Events.Payment;

namespace Checkout.PaymentGateway.Service.Domain
{
    /// <summary>
    /// TODO: investigate FSMs to manage moving between valid states
    /// </summary>
    public class Payment
    {
        internal Payment() { }

        internal void Create(CreatePaymentEvent e)
        {
            if(Status != PaymentStatus.Unknown) throw new Exception("Payment has already been created");

            Status = PaymentStatus.Created;
            CardNumber = e.CardNumber;
            Amount = e.Amount;
            Currency = e.Currency;
            Created = e.TimeStamp;
            Id = e.Id;
        }

        internal void SentToAcquiringBank(SentToAcquiringBankEvent e)
        {
            if (Status != PaymentStatus.Created) throw new Exception("Payment not yet been created");

            Status = PaymentStatus.WithAcquiringBank;
        }

        internal void SetSucceeded(PaymentSuccessEvent e)
        {
            Status = PaymentStatus.Success;
            AcquiringBankPaymentId = e.AcquiringBankPaymentId;
        }

        internal void SetFailed(PaymentFailureEvent e)
        {
            Status = PaymentStatus.Failure;
            FailureReason = e.Reason;
        }

        public string FailureReason { get; private set; }

        public Guid AcquiringBankPaymentId { get; private set; }

        public Guid Id { get; private set; }

        public PaymentStatus Status { get; private set; }

        public DateTime Created { get; private set; }

        public Currency Currency { get; private set; }

        public decimal Amount { get; private set; }

        public string CardNumber { get; private set; }

        public string GetFormattedMaskedCardNumber()
        {
            return $"{CardNumber.Substring(0, 4)} **** **** {CardNumber.Substring(12)}";
        }
    }
}
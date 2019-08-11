using System;
using Checkout.PaymentGateway.Service.Domain;

namespace Checkout.PaymentGateway.Service.Eventing.Events.Payment
{
    public class CreatePaymentEvent : IEvent<Domain.Payment>
    {
        public CreatePaymentEvent(Guid id, DateTime timeStamp, decimal amount, Currency currency, string cardNumber)
        {
            Id = id;
            TimeStamp = timeStamp;
            Amount = amount;
            Currency = currency;
            CardNumber = cardNumber;
        }

        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        public decimal Amount { get; }
        public Currency Currency { get; }
        public string CardNumber { get; }

        public void Process(Domain.Payment payment) => payment.Create(this);
    }
}
using System;
using Checkout.PaymentGateway.Service.Domain;

namespace Checkout.PaymentGateway.Service.RequestResponse
{
    public class CreateRequest : Request
    {
        public CreateRequest(Guid id, decimal amount, Currency currency, string cardNumber, int expiryMonth, int expiryYear, string securityCode)
        {
            Id = id;
            Amount = amount;
            Currency = currency;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            SecurityCode = securityCode;
        }

        public Guid Id { get; }
        public decimal Amount { get; }
        public Currency Currency { get; }
        public string CardNumber { get; }
        public int ExpiryMonth { get; }
        public int ExpiryYear { get; }
        public string SecurityCode { get; }
    }
}
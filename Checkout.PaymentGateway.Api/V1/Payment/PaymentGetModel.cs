using System;
using Checkout.PaymentGateway.Service.Domain;

namespace Checkout.PaymentGateway.Api.V1.Payment
{
    public class PaymentGetModel
    {
        public Guid Id { get; set; }

        public string FailureReason { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime Created { get; set; }

        public Currency Currency { get; set; }

        public decimal Amount { get; set; }

        public string CardNumber { get; set; }

        public Guid AcquiringBankPaymentId { get; set; }
    }
}
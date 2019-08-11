using System.ComponentModel.DataAnnotations;
using Checkout.PaymentGateway.Api.Mvc.Validation;
using Checkout.PaymentGateway.Service.Domain;

namespace Checkout.PaymentGateway.Api.V1.Payment
{
    public class PaymentPostModel
    {
        [Required, NotDefault, Range(0, 1_000_000)]
        public decimal Amount { get; set; }

        [Required, NotDefault, EnumIsDefined]
        public Currency Currency { get; set; }

        [Required, CreditCard]
        public string CardNumber { get; set; }

        [Required, Range(1, 12)]
        public int ExpiryMonth { get; set; }

        [Required, Range(2019, 2050)]
        public int ExpiryYear { get; set; }

        [Required, MinLength(3), MaxLength(4)]
        public string SecurityCode { get; set; }
    }
}

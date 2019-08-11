using System;
using System.Collections.Generic;
using Checkout.PaymentGateway.Api.V1.Payment;
using Checkout.PaymentGateway.Service.Domain;

namespace Checkout.PaymentGateway.Api.Tests.Integration.V1.Builders
{
    public class PaymentPostModelBuilder
    {
        private const string SuccessCardNumber = "4929395704290281";
        private const string FailCardNumber = "4716521177828292";
        private string _cardNumber = SuccessCardNumber;
        private IList<Action<PaymentPostModel>> _modifications = new List<Action<PaymentPostModel>>();

        public PaymentPostModelBuilder WithFailingCardNumber()
        {
            _modifications.Add(x => x.CardNumber = FailCardNumber);
            return this;
        }

        public PaymentPostModelBuilder With(Action<PaymentPostModel> modify)
        {
            _modifications.Add(modify);
            return this;
        }

        public PaymentPostModel Build()
        {
            var paymentPostModel = new PaymentPostModel
            {
                Amount = 1,
                CardNumber = _cardNumber,
                Currency = Currency.NewZealandDollar,
                ExpiryYear = 2050,
                ExpiryMonth = 1,
                SecurityCode = "123"
            };

            foreach (var modification in _modifications)
            {
                modification(paymentPostModel);
            }

            return paymentPostModel;
        }
    }
}
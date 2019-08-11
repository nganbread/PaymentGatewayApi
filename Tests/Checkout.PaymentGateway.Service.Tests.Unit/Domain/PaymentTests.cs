using System;
using Checkout.PaymentGateway.Service.Domain;
using Checkout.PaymentGateway.Service.Eventing.Events.Payment;
using FluentAssertions;
using Xunit;

namespace Checkout.PaymentGateway.Service.Tests.Unit.Domain
{
    public class PaymentTests
    {
        private readonly Payment _payment = new Payment();

        [Fact]
        public void WhenAPaymentIsCreated_ThenStatusIsSetToCreated()
        {
            _payment.Create(new CreatePaymentEvent(default, default, default, default, default));

            _payment.Status.Should().Be(PaymentStatus.Created);
        }
        
        [Fact]
        public void GivenAPaymentIsCreated_WhenItIsCreatedAgain_ThenAnExceptionIsThrown()
        {
            _payment.Create(new CreatePaymentEvent(default, default, default, default, default));

            Assert.ThrowsAny<Exception>(() => _payment.Create(new CreatePaymentEvent(default, default, default, default, default)));
        }

        [Fact]
        public void GivenAPaymentIsCreated_WhenItIsSentToAcquiringBank_ThenStatusIsSetToWithAcquiringBank()
        {
            _payment.Create(new CreatePaymentEvent(default, default, default, default, default));
            _payment.SentToAcquiringBank(new SentToAcquiringBankEvent(default, default));

            _payment.Status.Should().Be(PaymentStatus.WithAcquiringBank);
        }

        [Fact]
        public void GivenAPaymentIsSuccessful_WhenItIsSentToAcquiringBank_ThenAnExceptionIsThrown()
        {
            _payment.Create(new CreatePaymentEvent(default, default, default, default, default));
            _payment.SetSucceeded(new PaymentSuccessEvent(default, default, default));

            Assert.ThrowsAny<Exception>(() => _payment.SentToAcquiringBank(new SentToAcquiringBankEvent(default, default)));
        }

        [Fact]
        public void GivenAPaymentIsNotSuccessful_WhenItIsSentToAcquiringBank_ThenAnExceptionIsThrown()
        {
            _payment.Create(new CreatePaymentEvent(default, default, default, default, default));
            _payment.SetFailed(new PaymentFailureEvent(default, default, default));

            Assert.ThrowsAny<Exception>(() => _payment.SentToAcquiringBank(new SentToAcquiringBankEvent(default, default)));
        }
    }
}

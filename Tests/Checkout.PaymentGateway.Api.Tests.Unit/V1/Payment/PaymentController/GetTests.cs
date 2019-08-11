using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.V1.Payment;
using Checkout.PaymentGateway.Service.Eventing.Events.Payment;
using Checkout.PaymentGateway.Service.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Api.Tests.Unit.V1.Payment
{
    public class GetTests
    {
        private readonly PaymentController _controller;
        private readonly Mock<IPaymentService> _paymentService;
        private readonly Service.Domain.Payment _payment;

        public GetTests()
        {
            _paymentService = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentService.Object);

            _payment = new Service.Domain.Payment();
            _payment.Create(new CreatePaymentEvent(default, default, default, default, "1234123412341234"));

            _paymentService
                .Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync(_payment);
        }

        [Fact]
        public async Task WhenPaymentExists_ThenA200IsReturnedAndTheCardNumberIsMasked()
        {
            var response = await _controller.Get(Guid.Empty);

            response.As<ObjectResult>()?.StatusCode.Should().Be(200);
            response.Value.CardNumber.Trim().Replace(" ", "").Should().NotBe(_payment.CardNumber);
        }

        [Fact]
        public async Task WhenPaymentDoesNotExist_ThenA404IsReturned()
        {
            _paymentService
                .Setup(x => x.Get(It.IsAny<Guid>()))
                .ReturnsAsync((Service.Domain.Payment)null);

            var response = await _controller.Get(Guid.Empty);

            response.As<ObjectResult>()?.StatusCode.Should().Be(404);
        }
    }
}
using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.V1.Payment;
using Checkout.PaymentGateway.Service.RequestResponse;
using Checkout.PaymentGateway.Service.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Api.Tests.Unit.V1.Payment
{
    public class PostTests
    {
        private readonly PaymentController _controller;
        private readonly Mock<IPaymentService> _paymentService;

        public PostTests()
        {
            _paymentService = new Mock<IPaymentService>();
            _controller = new PaymentController(_paymentService.Object);

            _paymentService
                .Setup(x => x.Create(It.IsAny<CreateRequest>()))
                .ReturnsAsync(Response.Success);
        }

        [Fact]
        public async Task WhenPaymentIsSuccessful_ThenA201IsReturned()
        {
            var response = await _controller.Post(Guid.Empty, new PaymentPostModel());

            response.As<ObjectResult>()?.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task WhenPaymentIsNotSuccessful_ThenA400IsReturned()
        {
            _paymentService
                .Setup(x => x.Create(It.IsAny<CreateRequest>()))
                .ReturnsAsync(new Response(error: "Error"));

            var response = await _controller.Post(Guid.Empty, new PaymentPostModel());

            response.As<ObjectResult>()?.StatusCode.Should().Be(400);
        }
    }
}
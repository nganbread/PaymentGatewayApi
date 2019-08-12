using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Mvc.Validation;
using Checkout.PaymentGateway.Service.RequestResponse;
using Checkout.PaymentGateway.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Checkout.PaymentGateway.Api.V1.Payment
{
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PaymentController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Route("{id:guid}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromRoute, NotDefault, Description("The id of the payment to be created. Used for idempotency.")] Guid id, [FromBody, Required] PaymentPostModel model)
        {
            var request = new CreateRequest(id, model.Amount, model.Currency, model.CardNumber, model.ExpiryMonth, model.ExpiryYear, model.SecurityCode);
            var response = await _paymentService.Create(request);

            if (!response.IsSuccessful) return new BadRequestObjectResult(new ValidationProblemDetails(response.Errors)
            {
                Title = "Payment failed to process successfully"
            });

            return new CreatedResult($"/v1/payment/{id}", null);
        }

        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentGetModel>> Get([Required, Description("The id of the payment")] Guid id)
        {
            var payment = await _paymentService.Get(id);

            if(payment == null) return new NotFoundResult();

            return new PaymentGetModel
            {
                Id = payment.Id,
                Status = payment.Status,
                Created = payment.Created,
                Currency = payment.Currency,
                Amount = payment.Amount,
                FailureReason = payment.FailureReason,
                CardNumber = payment.GetFormattedMaskedCardNumber(),
                AcquiringBankPaymentId = payment.AcquiringBankPaymentId
            };
        }
    }
}

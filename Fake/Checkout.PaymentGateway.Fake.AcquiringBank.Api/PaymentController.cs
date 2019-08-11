using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Fake.AcquiringBank.Api
{
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Model model)
        {
            _logger.LogInformation($"Received Request at {HttpContext.Request.GetDisplayUrl()} with CardNumber '{model?.CardNumber}'");

            if (int.TryParse(model?.CardNumber?.Last().ToString(), out var lastDigit))
            {
                switch (lastDigit)
                {
                    case 1: return Created("", Guid.NewGuid());
                    case 2: return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return BadRequest();
        }
    }

    public class Model
    {
        public string CardNumber { get; set; }
    }
}

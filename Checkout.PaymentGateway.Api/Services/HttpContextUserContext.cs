using Checkout.PaymentGateway.Service.Services;
using Microsoft.AspNetCore.Http;

namespace Checkout.PaymentGateway.Api.Services
{
    internal class HttpContextUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserName => _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
    }
}

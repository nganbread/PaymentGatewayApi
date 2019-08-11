using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Checkout.PaymentGateway.Api.Mvc
{
    internal class DummyAuthenticationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next(context);
        }
    }
}
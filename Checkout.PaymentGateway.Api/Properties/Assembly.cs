using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]
[assembly: InternalsVisibleTo("Checkout.PaymentGateway.Api.Tests.Integration")]
[assembly: InternalsVisibleTo("Checkout.PaymentGateway.Api.Tests.Unit")]
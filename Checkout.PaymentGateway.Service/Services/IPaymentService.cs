using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Service.Domain;
using Checkout.PaymentGateway.Service.RequestResponse;

namespace Checkout.PaymentGateway.Service.Services
{
    public interface IPaymentService
    {
        Task<Response> Create(CreateRequest request);
        Task<Payment> Get(Guid id);
    }
}
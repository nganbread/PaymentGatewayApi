namespace Checkout.PaymentGateway.Service.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object o);
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Checkout.PaymentGateway.Service.Serialization
{
    internal class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new JsonConverter[]
            {
                new StringEnumConverter()
            }
        };

        public string Serialize(object o) => JsonConvert.SerializeObject(o, _settings);
    }
}

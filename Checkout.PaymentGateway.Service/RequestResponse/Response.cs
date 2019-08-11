using System.Collections.Generic;
using System.Linq;

namespace Checkout.PaymentGateway.Service.RequestResponse
{
    public class Response<T> : Response
    {
        public T Value { get; }

        public Response(T value)
        {
            Value = value;
        }

        public Response(string error) : base(error)
        {

        }

        public static implicit operator Response<T>(T t) => new Response<T>(t);
        public static implicit operator T(Response<T> response) => response.Value;
    }

    public class Response
    {
        private readonly Dictionary<string, IList<string>> _errors = new Dictionary<string, IList<string>>();

        public Response(string error)
        {
            _errors[string.Empty] = new List<string>{ error };
        }

        public Response()
        {
            
        }

        public static Response Success => new Response();

        public string this[string property]  
        {
            set
            {
                if (_errors.ContainsKey(property))
                {
                    _errors[property].Add(value);
                }
                else
                {
                    _errors.Add(property, new List<string>{ value });
                }
            }  
        }

        public IDictionary<string, string[]> Errors => _errors.ToDictionary(x => x.Key, x => x.Value.ToArray());

        public bool IsSuccessful => !_errors.Any(x => x.Value.Any());

        public override string ToString()
        {
            var errors = _errors.Select(e =>
            {
                var error = string.Join(", ", e);
                return $"{e.Key}: {error}";
            });

            return string.Join(". ", errors);
        }
    }
}   
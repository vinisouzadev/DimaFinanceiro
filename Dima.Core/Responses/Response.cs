using Dima.Core.Common;
using System.Text.Json.Serialization;

namespace Dima.Core.Responses
{
    public class Response<TData>
    {   
        private const int _defaultStatusCode = Configuration.DefaultStatusCode;

        private readonly int _code;

        [JsonConstructor]
        public Response()
        {
            _code = _defaultStatusCode;
        }

        public Response(TData? data, int code = _defaultStatusCode, string? message = null)
        {
            Data = data;
            Message = message;
            _code = code;
        }

        public TData? Data { get; set; }

        public string? Message { get; set; } = string.Empty; 

        [JsonIgnore]
        public bool IsSuccess => _code is >= 200 and <= 299; 

    }
}

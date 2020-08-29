using System.Net;

namespace Fortnite.External.Api
{
    public class ExternalResponse
    {
        public HttpStatusCode? StatusCode { get; set; } = HttpStatusCode.ExpectationFailed;
        public string Data { get; set; } = null;
        public string ErrorMessage { get; set; } = null;
    }
}
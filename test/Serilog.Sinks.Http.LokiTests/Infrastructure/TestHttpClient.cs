using System.Net.Http;
using System.Threading.Tasks;
using Serilog.Sinks.Http.Loki.HttpClients;

namespace Serilog.Sinks.Http.Loki.Tests.Infrastructure
{
    public class TestHttpClient : DefaultLokiHttpClient
    {
        public override async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            Content = await content.ReadAsStringAsync();
            RequestUri = requestUri;

            return new HttpResponseMessage();
        }

        public HttpClient Client => HttpClient;

        public string Content;

        public string RequestUri;
    }
}

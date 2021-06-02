using Serilog.Sinks.Http.Loki.Tests.Infrastructure;
using Xunit;

namespace Serilog.Sinks.Http.Loki.Tests.Labels
{
    public class LocalLabelsTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly HttpClientTestFixture _httpClientTestFixture;
        private readonly TestHttpClient _client;

        public LocalLabelsTests(HttpClientTestFixture httpClientTestFixture)
        {
            _httpClientTestFixture = httpClientTestFixture;
            _client = new TestHttpClient();
        }
    }
}

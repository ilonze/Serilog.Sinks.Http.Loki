using Serilog.Sinks.Http.Loki.Labels;
using Serilog.Sinks.Http.Loki.Tests.Infrastructure;
using Xunit;
using System.Linq;
using Shouldly;
#if SYSTEMTEXTJSON
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki.Tests.Labels
{
    public class LocalLabelsTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly HttpClientTestFixture _httpClientTestFixture;
        private readonly TestHttpClient _client;
        private readonly BasicAuthCredentials _credentials;
        private readonly BasicAuthCredentials _deprecatedCredentials;

        public LocalLabelsTests(HttpClientTestFixture httpClientTestFixture)
        {
            _httpClientTestFixture = httpClientTestFixture;
            _client = new TestHttpClient();
            _credentials = new BasicAuthCredentials("http://test:80", "Walter", "White");
            _deprecatedCredentials = new BasicAuthCredentials("http://test:80", "Walter", "White", LokiCredentials.DeprecatedPushDataPath);
        }

        [Fact]
        public void Deprecated_LocalLabelsCanBeSet()
        {
            // Arrange
            var provider = new DefaultLogLabelProvider();
            provider.AddPropertiesAsLabels("local_label");
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_deprecatedCredentials, logLabelProvider: provider, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong {local_label}", "ThisIsALocalLabel");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"error\",local_label=\"ThisIsALocalLabel\"}");
        }

        [Fact]
        public void LocalLabelsCanBeSet()
        {
            // Arrange
            var provider = new DefaultLogLabelProvider();
            provider.AddPropertiesAsLabels("local_label");
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, logLabelProvider: provider, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong {local_label}", "ThisIsALocalLabel");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            ("{" + string.Join(",", response.Streams.First().Stream.OrderBy(r => r.Key).Select(r => $"{r.Key}=\"{r.Value}\"")) + "}").ShouldBe("{level=\"error\",local_label=\"ThisIsALocalLabel\"}");
        }
    }
}

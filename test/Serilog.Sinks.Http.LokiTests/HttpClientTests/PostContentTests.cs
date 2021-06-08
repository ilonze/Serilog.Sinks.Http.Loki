using System.Text.RegularExpressions;
using Serilog.Sinks.Http.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Http.Loki.Tests.HttpClientTests
{
    public class PostContentTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly TestHttpClient _client;

        public PostContentTests()
        {
            _client = new TestHttpClient();
        }

        [Fact]
        public void Deprecated_ContentMatchesApproved()
        {
            // Arrange
            var credentials = new NoAuthCredentials("http://test:80", LokiCredentials.DeprecatedPushDataPath);
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong");
            log.Dispose();

            // Assert
            _client.Content.ShouldMatch(@"\d{1,2}\d{1,2}\d{2,4}-\d{1,2}-\d{1,2}T\d{1,2}:\d{1,2}:\d{1,2}.\d{1,7}\+\d{2}:\d{2}");
        }

        [Fact]
        public void ContentMatchesApproved()
        {
            // Arrange
            var credentials = new NoAuthCredentials("http://test:80");
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Something's wrong");
            log.Dispose();

            // Assert
            _client.Content.ShouldMatch(@"\d{17,}00");
        }
    }
}

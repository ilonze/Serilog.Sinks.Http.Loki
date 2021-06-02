using System.Linq;
#if SYSTEMTEXTJSON
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif
using Serilog.Sinks.Http.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Http.Loki.Tests.Content
{
    public class ContentTests : IClassFixture<HttpClientTestFixture>
    {
        private readonly HttpClientTestFixture _httpClientTestFixture;
        private readonly TestHttpClient _client;
        private readonly BasicAuthCredentials _credentials;

        public ContentTests(HttpClientTestFixture httpClientTestFixture)
        {
            _httpClientTestFixture = httpClientTestFixture;
            _client = new TestHttpClient();
            _credentials = new BasicAuthCredentials("http://test:80", "Walter", "White");
        }

        [Fact]
        public void QuotedContentStringsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "Text \"with\" quotes.");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            response.Streams.First().Entries.First().Line.ShouldStartWith("Data with quotes: Text \"with\" quotes.\n");
        }

        [Fact]
        public void UnquotedWhitespaceFieldsAreNotSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Data without quotes: {data}", "Text without quotes.");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"Text without quotes.\"");
        }

        [Fact]
        public void QuotedWhitespaceFieldsAreNotSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "Text \"with\" quotes.");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"Text \\\"with\\\" quotes.\"");
        }

        [Fact]
        public void UnquotedNonWhitespaceFieldsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Data without quotes: {data}", "TextWithoutQuotes");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            response.Streams.First().Entries.First().Line.ShouldContain("data=TextWithoutQuotes");
        }

        [Fact]
        public void QuotedNonWhitespaceFieldsAreSimplified()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();

            // Act
            log.Error("Data with quotes: {data}", "\"TextWithQuotes\"");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestResponse>(_client.Content);
#endif
            response.Streams.First().Entries.First().Line.ShouldContain("data=\"TextWithQuotes\"");
        }
    }
}

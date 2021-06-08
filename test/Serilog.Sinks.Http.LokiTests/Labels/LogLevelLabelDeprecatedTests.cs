using System.Linq;
#if SYSTEMTEXTJSON
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif
using Serilog.Sinks.Http.Loki;
using Serilog.Sinks.Http.Loki.Labels;
using Serilog.Sinks.Http.Loki.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace Serilog.Sinks.Loki.Tests.Labels
{
    public class LogLevelLabelDeprecatedTests
    {
        private readonly TestHttpClient _client;
        private readonly BasicAuthCredentials _credentials;

        public LogLevelLabelDeprecatedTests()
        {
            _client = new TestHttpClient();
            _credentials = new BasicAuthCredentials("http://test:80", "Walter", "White", LokiCredentials.DeprecatedPushDataPath);
        }
        
        [Fact]
        public void NoLabelIsSet()
        {
            // Arrange
            var provider = new DefaultLogLabelProvider(new LokiLabel[0], new string[0]); // Explicitly NOT include level
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.HttpLoki(_credentials, logLabelProvider: provider, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Fatal("Fatal Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{}");
        }
        
        [Fact]
        public void VerboseLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Verbose("Verbose Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"trace\"}");
        }
        
        [Fact]
        public void DebugLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Debug("Debug Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"debug\"}");
        }
        
        [Fact]
        public void InformationLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Information("Information Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"info\"}");
        }

        [Fact]
        public void ErrorLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Error("Error Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"error\"}");
        }

        [Fact]
        public void FatalLabelIsSet()
        {
            // Arrange
            var log = new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .WriteTo.HttpLoki(_credentials, httpClient: _client)
                .CreateLogger();
            
            // Act
            log.Fatal("Fatal Level");
            log.Dispose();

            // Assert
#if SYSTEMTEXTJSON
            var response = JsonSerializer.Deserialize<TestDeprecatedResponse>(_client.Content);
#elif NEWTONSOFTJSON
            var response = JsonConvert.DeserializeObject<TestDeprecatedResponse>(_client.Content);
#endif
            response.Streams.First().Labels.ShouldBe("{level=\"critical\"}");
        }
    }
}

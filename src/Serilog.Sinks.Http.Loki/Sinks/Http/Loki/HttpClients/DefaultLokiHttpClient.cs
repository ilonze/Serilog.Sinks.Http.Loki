using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki.HttpClients
{
    /// <summary>
    /// Default Loki HttpClient
    /// </summary>
    public class DefaultLokiHttpClient : ILokiHttpClient
    {
        /// <summary>
        /// HttpClient
        /// </summary>
        protected HttpClient HttpClient { get; }
        /// <summary>
        /// 
        /// </summary>
        public DefaultLokiHttpClient()
        {
            HttpClient = new HttpClient();
        }

        /// <inheritdoc/>
        public virtual void Configure(IConfiguration configuration)
        {

        }

        /// <inheritdoc/>
        public virtual void Dispose()
            => HttpClient.Dispose();

        /// <inheritdoc/>
        public virtual async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return await HttpClient.PostAsync(requestUri, content);
        }

        /// <inheritdoc/>
        public virtual void SetAuthCredentials(LokiCredentials credentials)
        {
            if (credentials is not BasicAuthCredentials c)
                return;

            var headers = HttpClient.DefaultRequestHeaders;
            if (headers.Any(x => x.Key == "Authorization"))
                return;

            var token = Base64Encode($"{c.Username}:{c.Password}");
            headers.Add("Authorization", $"Basic {token}");
        }
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}

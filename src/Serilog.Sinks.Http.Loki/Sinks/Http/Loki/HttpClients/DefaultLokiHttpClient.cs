using Microsoft.Extensions.Configuration;
using Serilog.Debugging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
        /// Callback when logs were's discarded
        /// </summary>
        protected Action<HttpResponseMessage>? CallbackOnDiscarded { get; private set; }

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

            var result = await HttpClient.PostAsync(requestUri, content);
            if (!result.IsSuccessStatusCode)
            {
                var contentAsString = await content.ReadAsStringAsync();
                if ((int)result.StatusCode == 429)
                {
                    SelfLog.WriteLine("The Loki server is soo busy, these logs are discarded.\n{0}", contentAsString);
                }
                else
                {
                    SelfLog.WriteLine("Exception occurred during pushing logs, these logs are discarded. HttpStatusCode：{0}.\n{1}", result.StatusCode, contentAsString);
                }
                CallbackOnDiscarded?.Invoke(result);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            return result;
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

        /// <inheritdoc/>
        public void OnDiscarded(Action<HttpResponseMessage> action)
        {
            CallbackOnDiscarded = action;
        }
    }
}

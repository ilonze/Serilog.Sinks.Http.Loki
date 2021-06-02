using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Credentials for Loki
    /// </summary>
    public abstract class LokiCredentials
    {
        /// <summary>
        /// The path for push log of Loki
        /// </summary>
        public const string PostDataUri = "/api/prom/push";
        /// <summary>
        /// Url of Credentials
        /// </summary>
        public string Url { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        protected LokiCredentials(string url)
        {
            Url = $"{url.TrimEnd('/')}{PostDataUri}";
        }
    }
    /// <summary>
    /// Non auth credentials for Loki
    /// </summary>
    public class NoAuthCredentials : LokiCredentials
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public NoAuthCredentials(string url): base(url)
        {
        }
    }
    /// <summary>
    /// Basic auth credentials for Loki
    /// </summary>
    public class BasicAuthCredentials : LokiCredentials
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public BasicAuthCredentials(string url, string username, string password): base(url)
        {
            Username = username;
            Password = password;
        }
        /// <summary>
        /// Username for Loki
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// Password for Loki
        /// </summary>
        public string Password { get; }
    }
}

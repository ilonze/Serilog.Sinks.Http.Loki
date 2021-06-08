using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Sinks.Http.Loki.BatchFormatters;

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Credentials for Loki
    /// </summary>
    public abstract class LokiCredentials
    {
        /// <summary>
        /// 
        /// </summary>
        public const string PushDataPath = "/loki/api/v1/push";
        /// <summary>
        /// 
        /// </summary>
        public const string DeprecatedPushDataPath = "/api/prom/push";
        /// <summary>
        /// Url of Credentials
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pushPath"></param>
        protected LokiCredentials(string url, string pushPath = PushDataPath)
        {
            Url = $"{url.TrimEnd('/')}{pushPath ?? PushDataPath}";
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
        /// <param name="pushPath"></param>
        public NoAuthCredentials(string url, string pushPath = PushDataPath) : base(url, pushPath)
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
        /// <param name="pushPath"></param>
        public BasicAuthCredentials(string url, string username, string password, string pushPath = PushDataPath) : base(url, pushPath)
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

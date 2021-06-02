using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Interface responsible for posting HTTP requests to Loki.
    /// </summary>
    public interface ILokiHttpClient: IHttpClient
    {
        /// <summary>
        /// Set auth credentials for Loki
        /// </summary>
        /// <param name="credentials"></param>
        void SetAuthCredentials(LokiCredentials credentials);
    }
}

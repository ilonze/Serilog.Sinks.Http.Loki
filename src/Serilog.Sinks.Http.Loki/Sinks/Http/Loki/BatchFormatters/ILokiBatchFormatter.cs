using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki.BatchFormatters
{
    /// <summary>
    /// Formats batches of log events into payloads that can be sent over the network to loki.
    /// </summary>
    public interface ILokiBatchFormatter: IBatchFormatter
    {
        /// <summary>
        /// Check this formatter can format?
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool CanFormat(string path);
    }
}

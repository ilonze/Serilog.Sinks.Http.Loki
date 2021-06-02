using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki.Labels
{
    /// <summary>
    /// Log label provider for Loki
    /// </summary>
    public interface ILogLabelProvider
    {
        /// <summary>
        /// Gets labels
        /// </summary>
        /// <returns></returns>
        IList<LokiLabel> GetLabels();

        /// <summary>
        /// Properties as labels
        /// </summary>
        IList<string> PropertiesAsLabels { get; }
        /// <summary>
        /// Properties to append Log content
        /// </summary>
        IList<string> PropertiesToAppend { get; }
        /// <summary>
        /// Formatter strategy
        /// </summary>
        LokiFormatterStrategy FormatterStrategy { get; }
    }
}

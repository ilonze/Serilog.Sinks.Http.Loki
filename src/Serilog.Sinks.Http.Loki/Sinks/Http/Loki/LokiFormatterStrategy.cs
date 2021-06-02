using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Loki formatter strategy
    /// </summary>
    public enum LokiFormatterStrategy
    {
        /// <summary>
        /// All Serilog Event properties will be sent as labels
        /// </summary>
        AllPropertiesAsLabels,

        /// <summary>
        /// Specific Serilog Event properties will be sent as labels.
        /// The rest of properties will be discarder.
        /// </summary>
        SpecificPropertiesAsLabelsAndRestDiscarded,

        /// <summary>
        /// Specific Serilog Event properties will be sent as labels.
        /// The rest of properties will be appended to the log message.
        /// </summary>
        SpecificPropertiesAsLabelsAndRestAppended,

        /// <summary>
        /// Specific Serilog Event properties will be sent as labels.
        /// Other specific properties will be appended to the log message.
        /// The rest of properties will be discarded
        /// </summary>
        SpecificPropertiesAsLabelsOrAppended
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki.Labels
{
    /// <summary>
    /// Default log label provider for Loki
    /// </summary>
    public class DefaultLogLabelProvider : ILogLabelProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public DefaultLogLabelProvider() : this(new List<LokiLabel>())
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="propertiesAsLabels"></param>
        /// <param name="propertiesToAppend"></param>
        /// <param name="formatterStrategy"></param>
        public DefaultLogLabelProvider(IEnumerable<LokiLabel> labels,
            IEnumerable<string>? propertiesAsLabels = null,
            IEnumerable<string>? propertiesToAppend = null,
            LokiFormatterStrategy formatterStrategy = LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended)
        {
            Labels = labels?.ToList() ?? new List<LokiLabel>();
            PropertiesAsLabels = propertiesAsLabels?.ToList() ?? new List<string> { "level" };
            PropertiesToAppend = propertiesToAppend?.ToList() ?? new List<string>();
            FormatterStrategy = formatterStrategy;
        }
        /// <inheritdoc/>
        public IList<LokiLabel> GetLabels()
            => Labels;

        private IList<LokiLabel> Labels { get; }
        /// <inheritdoc/>
        public IList<string> PropertiesAsLabels { get; }
        /// <inheritdoc/>
        public IList<string> PropertiesToAppend { get; }
        /// <inheritdoc/>
        public LokiFormatterStrategy FormatterStrategy { get; }
    }
}

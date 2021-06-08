using System.Collections.Generic;
using System.Linq;

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
        public List<LokiLabel> Labels { get; }

        /// <inheritdoc/>
        public List<string> PropertiesAsLabels { get; }

        /// <inheritdoc/>
        public List<string> PropertiesToAppend { get; }

        /// <inheritdoc/>
        public LokiFormatterStrategy FormatterStrategy { get; }

        /// <inheritdoc/>
        public DefaultLogLabelProvider AddLabel(string key, string value)
        {
            Labels.Add(new LokiLabel(key, value));
            return this;
        }

        /// <inheritdoc/>
        public DefaultLogLabelProvider AddLabels(params LokiLabel[] labels)
        {
            Labels.AddRange(labels);
            return this;
        }

        /// <inheritdoc/>
        public DefaultLogLabelProvider AddLabels(Dictionary<string, string> labels)
        {
            Labels.AddRange(labels.Select(r => new LokiLabel(r.Key, r.Value)));
            return this;
        }

        /// <inheritdoc/>
        public DefaultLogLabelProvider AddPropertiesAsLabels(params string[] properties)
        {
            PropertiesAsLabels.AddRange(properties);
            return this;
        }

        /// <inheritdoc/>
        public DefaultLogLabelProvider AddPropertiesToAppend(params string[] properties)
        {
            PropertiesToAppend.AddRange(properties);
            return this;
        }
    }
}

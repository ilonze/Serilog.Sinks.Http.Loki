using System.Collections.Generic;

namespace Serilog.Sinks.Http.Loki.Labels
{
    /// <summary>
    /// Log label provider for Loki
    /// </summary>
    public interface ILogLabelProvider
    {
        /// <summary>
        /// Adds Labels
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DefaultLogLabelProvider AddLabel(string key, string value);

        /// <summary>
        /// Adds Labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        DefaultLogLabelProvider AddLabels(params LokiLabel[] labels);

        /// <summary>
        /// Adds Labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        DefaultLogLabelProvider AddLabels(Dictionary<string, string> labels);

        /// <summary>
        /// Add properties as labels
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        DefaultLogLabelProvider AddPropertiesAsLabels(params string[] properties);

        /// <summary>
        /// Add properties append to log message content
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        DefaultLogLabelProvider AddPropertiesToAppend(params string[] properties);

        /// <summary>
        /// Gets properties as labels
        /// </summary>
        List<string> PropertiesAsLabels { get; }

        /// <summary>
        /// Gets properties to append log content
        /// </summary>
        List<string> PropertiesToAppend { get; }

        /// <summary>
        /// Formatter strategy
        /// </summary>
        LokiFormatterStrategy FormatterStrategy { get; }

        /// <summary>
        /// Gets labels
        /// </summary>
        List<LokiLabel> Labels { get; }
    }
}

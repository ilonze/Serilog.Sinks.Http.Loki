using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.Loki.Labels;
using System.Text;
using Serilog.Sinks.Http.Loki.PushDatas;

namespace Serilog.Sinks.Http.Loki.BatchFormatters
{
    /// <summary>
    /// Formats batches of log events into payloads that can be sent over the network to Loki.
    /// </summary>
    public class LokiBatchFormatter : ILokiBatchFormatter
    {
        private static readonly Regex ValueWithoutSpaces = new Regex("^\"(\\S+)\"$", RegexOptions.Compiled);
        /// <summary>
        /// Log label provider
        /// </summary>
        public ILogLabelProvider LogLabelProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLabelProvider"></param>
        public LokiBatchFormatter(ILogLabelProvider logLabelProvider)
            => LogLabelProvider = logLabelProvider;

        /// <inheritdoc/>
        public void Format(IEnumerable<LogEvent> logEvents, ITextFormatter formatter, TextWriter output)
        {
            if (logEvents == null)
                throw new ArgumentNullException(nameof(logEvents));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var logs = logEvents.ToList();
            if (!logs.Any())
                return;

            var streamsDictionary = new Dictionary<string, LokiStream>();
            foreach (var logEvent in logs)
            {
                var labels = new List<LokiLabel>();

                foreach (var globalLabel in LogLabelProvider.Labels)
                    labels.Add(new LokiLabel(globalLabel.Key, globalLabel.Value));

                var time = ((logEvent.Timestamp.ToUniversalTime().Ticks - 621355968000000000) * 100).ToString();
                var sb = new StringBuilder();
                using (var tw = new StringWriter(sb))
                {
                    formatter.Format(logEvent, tw);
                }

                HandleProperty("level", GetLevel(logEvent.Level), labels, sb);
                foreach (var property in logEvent.Properties)
                {
                    HandleProperty(property.Key, property.Value.ToString(), labels, sb);
                }

                // Order the labels so they always get the same chunk in loki
                labels = labels.OrderBy(l => l.Key).ToList();
                var key = string.Join(",", labels.Select(l => $"{l.Key}={l.Value}"));
                if (!streamsDictionary.TryGetValue(key, out var stream))
                {
                    streamsDictionary.Add(key, stream = new LokiStream());
                    stream.AddLabels(labels);
                }

                // Loki doesn't like \r\n for new line, and we can't guarantee the message doesn't have any
                // in it, so we replace \r\n with \n on the final message
                stream.Values.Add(new string[] { time, sb.ToString().Replace("\r\n", "\n") });
                sb.Clear();
            }

            if (streamsDictionary.Count > 0)
            {
                var content = new LokiPushData();
                content.Streams.AddRange(streamsDictionary.Values);
                output.Write(content.Serialize());
            }
        }

        private void HandleProperty(string name, string value, ICollection<LokiLabel> labels, StringBuilder sb)
        {
            // Some enrichers pass strings with quotes surrounding the values inside the string,
            // which results in redundant quotes after serialization and a "bad request" response.
            // To avoid this, remove all quotes from the value.
            // We also remove any \r\n newlines and replace with \n new lines to prevent "bad request" responses
            // We also remove backslashes and replace with forward slashes, Loki doesn't like those either
            value = value.Replace("\r\n", "\n");

            switch (DetermineHandleActionForProperty(name))
            {
                case HandleAction.Discard: return;
                case HandleAction.SendAsLabel:
                    value = value.Replace("\"", "").Replace("\\", "/");
                    labels.Add(new LokiLabel(name, value));
                    break;
                case HandleAction.AppendToMessage:
                    value = SimplifyValue(value);
                    sb.Append($" {name}={value}");
                    break;
            }
        }

        /// <inheritdoc/>
        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {

        }

        private static string SimplifyValue(string value)
        {
            var match = ValueWithoutSpaces.Match(value);
            return match.Success
                ? Regex.Unescape(match.Groups[1].Value)
                : value;
        }

        private static string GetLevel(LogEventLevel level) => level switch
        {
            LogEventLevel.Verbose => "trace",
            LogEventLevel.Information => "info",
            LogEventLevel.Fatal => "critical",
            _ => level.ToString().ToLower(),
        };

        private HandleAction DetermineHandleActionForProperty(string propertyName) => LogLabelProvider.FormatterStrategy switch
        {
            LokiFormatterStrategy.AllPropertiesAsLabels
                => HandleAction.SendAsLabel,
            LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestDiscarded
                => LogLabelProvider.PropertiesAsLabels.Contains(propertyName)
                    ? HandleAction.SendAsLabel
                    : HandleAction.Discard,
            LokiFormatterStrategy.SpecificPropertiesAsLabelsAndRestAppended
                => LogLabelProvider.PropertiesAsLabels.Contains(propertyName)
                    ? HandleAction.SendAsLabel
                    : HandleAction.AppendToMessage,
            _
                => LogLabelProvider.PropertiesAsLabels.Contains(propertyName)
                    ? HandleAction.SendAsLabel
                    : LogLabelProvider.PropertiesToAppend.Contains(propertyName)
                        ? HandleAction.AppendToMessage
                        : HandleAction.Discard,
        };

        /// <inheritdoc/>
        public bool CanFormat(string path) => path == LokiCredentials.PushDataPath;

        private enum HandleAction
        {
            Discard,
            SendAsLabel,
            AppendToMessage
        }
    }
}

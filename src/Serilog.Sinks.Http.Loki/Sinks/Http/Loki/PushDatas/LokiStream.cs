using System;
using System.Collections.Generic;
#if SYSTEMTEXTJSON
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif
using Serilog.Sinks.Http.Loki.Labels;

namespace Serilog.Sinks.Http.Loki.PushDatas
{
    /// <summary>
    /// Loki stream
    /// </summary>
    public class LokiStream
    {
        /// <summary>
        /// Loki stream labels
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("stream")]
#elif NEWTONSOFTJSON
        [JsonProperty("stream")]
#endif
        public Dictionary<string, string> Stream { get; } = new Dictionary<string, string>();
        /// <summary>
        /// Loki log contents
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("values")]
#elif NEWTONSOFTJSON
        [JsonProperty("values")]
#endif
        public List<string[]> Values { get; } = new List<string[]>();

        /// <summary>
        /// Add labels to stream
        /// </summary>
        /// <param name="labels"></param>
        public void AddLabels(IEnumerable<LokiLabel> labels)
        {
            foreach (var label in labels)
            {
                Stream.Add(label.Key, label.Value);
            }
        }
    }
}

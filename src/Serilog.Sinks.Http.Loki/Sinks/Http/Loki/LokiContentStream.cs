using Serilog.Sinks.Http.Loki.Labels;
using System.Collections.Generic;
using System.Text;
#if SYSTEMTEXTJSON
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Loki content stream
    /// </summary>
    public class LokiContentStream
    {
        /// <summary>
        /// Labels for Loki
        /// </summary>
        [JsonIgnore]
        public List<LokiLabel> Labels { get; } = new List<LokiLabel>();

        /// <summary>
        /// Labels for Loki
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("labels")]
#elif NEWTONSOFTJSON
        [JsonProperty("labels")]
#endif
        public string LabelsString
        {
            get
            {
                StringBuilder sb = new StringBuilder("{");
                bool firstLabel = true;
                foreach (LokiLabel label in Labels)
                {
                    if (firstLabel)
                        firstLabel = false;
                    else
                        sb.Append(",");

                    sb.Append(label.Key);
                    sb.Append("=\"");
                    sb.Append(label.Value);
                    sb.Append("\"");
                }

                sb.Append("}");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Entries for Loki
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("entries")]
#elif NEWTONSOFTJSON
        [JsonProperty("entries")]
#endif
        public List<LokiEntry> Entries { get; set; } = new List<LokiEntry>();
    }
}

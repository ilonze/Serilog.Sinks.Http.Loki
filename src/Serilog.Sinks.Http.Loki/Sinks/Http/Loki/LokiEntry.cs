#if SYSTEMTEXTJSON
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Entry for Loki
    /// </summary>
    public class LokiEntry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="line"></param>
        public LokiEntry(string ts, string line)
        {
            Ts = ts;
            Line = line;
        }

        /// <summary>
        /// Time stamp
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("ts")]
#elif NEWTONSOFTJSON
        [JsonProperty("ts")]
#endif
        public string Ts { get; set; }

        /// <summary>
        /// log line
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("line")]
#elif NEWTONSOFTJSON
        [JsonProperty("line")]
#endif
        public string Line { get; set; }
    }
}

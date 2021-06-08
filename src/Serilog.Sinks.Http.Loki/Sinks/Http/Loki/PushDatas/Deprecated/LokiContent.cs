using System;
using System.Collections.Generic;
#if SYSTEMTEXTJSON
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki
{
    /// <summary>
    /// Loki content
    /// </summary>
    public class LokiContent
    {
        /// <summary>
        /// Loki content streams
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("streams")]
#elif NEWTONSOFTJSON
        [JsonProperty("streams")]
#endif
        public List<LokiContentStream> Streams { get; set; } = new List<LokiContentStream>();

        /// <summary>
        /// Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
#if SYSTEMTEXTJSON
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
#elif NEWTONSOFTJSON
            return JsonConvert.SerializeObject(this);
#endif
        }
    }
}

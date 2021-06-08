using System;
using System.Collections.Generic;
#if SYSTEMTEXTJSON
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki.PushDatas
{
    /// <summary>
    /// Loki push data
    /// </summary>
    public class LokiPushData
    {
        /// <summary>
        /// streams
        /// </summary>
#if SYSTEMTEXTJSON
        [JsonPropertyName("streams")]
#elif NEWTONSOFTJSON
        [JsonProperty("streams")]
#endif
        public List<LokiStream> Streams { get; } = new List<LokiStream>();

        /// <summary>
        /// Serialize push data
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

        /// <inheritdoc/>
        public override string ToString() => Serialize();
    }
}

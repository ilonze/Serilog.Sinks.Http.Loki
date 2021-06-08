using System;
using System.Collections.Generic;
using System.Text;
#if SYSTEMTEXTJSON
using System.Text.Json;
using System.Text.Json.Serialization;
#elif NEWTONSOFTJSON
using Newtonsoft.Json;
#endif

namespace Serilog.Sinks.Http.Loki.Tests.Infrastructure
{
    public class TestResponse
    {
        public TestResponse()
        {
            Streams = new List<TestResponseStream>();
        }

#if SYSTEMTEXTJSON
        [JsonPropertyName("streams")]
#elif NEWTONSOFTJSON
        [JsonProperty("streams")]
#endif
        public IList<TestResponseStream> Streams { get; set; }
    }

    public class TestResponseStream
    {
#if SYSTEMTEXTJSON
        [JsonPropertyName("stream")]
#elif NEWTONSOFTJSON
        [JsonProperty("stream")]
#endif
        public Dictionary<string, string> Stream { get; set; }
#if SYSTEMTEXTJSON
        [JsonPropertyName("values")]
#elif NEWTONSOFTJSON
        [JsonProperty("values")]
#endif
        public List<string[]> Values { get; set; }
    }
}

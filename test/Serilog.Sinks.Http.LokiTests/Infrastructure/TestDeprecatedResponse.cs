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
    public class TestDeprecatedResponse
    {
        public TestDeprecatedResponse()
        {
            Streams = new List<Stream>();
        }

#if SYSTEMTEXTJSON
        [JsonPropertyName("streams")]
#elif NEWTONSOFTJSON
        [JsonProperty("streams")]
#endif
        public IList<Stream> Streams { get; set; }
    }

    public class Stream
    {
#if SYSTEMTEXTJSON
        [JsonPropertyName("labels")]
#elif NEWTONSOFTJSON
        [JsonProperty("labels")]
#endif
        public string Labels { get; set; }
#if SYSTEMTEXTJSON
        [JsonPropertyName("entries")]
#elif NEWTONSOFTJSON
        [JsonProperty("entries")]
#endif
        public List<Entry> Entries { get; set; }
    }

    public class Entry
    {
#if SYSTEMTEXTJSON
        [JsonPropertyName("line")]
#elif NEWTONSOFTJSON
        [JsonProperty("line")]
#endif
        public string Line { get; set; }
    }
}

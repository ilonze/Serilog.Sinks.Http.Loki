using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Http.Loki.Labels
{
    /// <summary>
    /// Loki label
    /// </summary>
    public class LokiLabel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public LokiLabel(string key, string value)
        {
            Key = key;
            Value = value;
        }
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; }
    }
}

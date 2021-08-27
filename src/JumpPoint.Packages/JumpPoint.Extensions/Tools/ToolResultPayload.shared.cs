using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Extensions.Tools
{
    public class ToolResultPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ToolResult Result { get; set; }

        public string Path { get; set; }

        public string Message { get; set; }

    }
}

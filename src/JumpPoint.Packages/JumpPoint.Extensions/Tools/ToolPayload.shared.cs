using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Extensions.Tools
{
    public class ToolPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ToolPayloadType ItemType { get; set; }

        public string Path { get; set; }

        public string Token { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JumpPoint.Platform.Extensions
{
    public class ToolPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public JumpPointItemType ItemType { get; set; }

        public string Path { get; set; }

        public string Token { get; set; }

    }

}

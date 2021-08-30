using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JumpPoint.Platform
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataType
    {
        String = 0,
        Int16 = 1,
        Int32 = 2,
        Int64 = 3,
        UInt8 = 4,
        UInt16 = 5,
        UInt32 = 6,
        UInt64 = 7,
        Single = 8,
        Double = 9,
        Boolean = 10,
        Char = 11,
        DateTime = 12,
        TimeSpan = 13,
        Guid = 14,
        ByteArray = 15,
        Point = 16,
        Size = 17,
        Rect = 18,
    }
}

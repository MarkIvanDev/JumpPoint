using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NittyGritty;

namespace JumpPoint.Platform.Models
{
    public class ValueInfo : ObservableObject
    {

        private string _key;

        public string Key
        {
            get { return _key; }
            set { Set(ref _key, value); }
        }

        private DataType _dataType;

        public DataType DataType
        {
            get { return _dataType; }
            set { Set(ref _dataType, value); }
        }

        private object _value;

        [JsonIgnore]
        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

    }

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

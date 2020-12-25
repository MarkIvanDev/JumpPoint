using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NittyGritty;

namespace JumpPoint.Platform.Models
{
    public class Seed : ObservableObject
    {

        private JumpPointItemType _type;

        [JsonConverter(typeof(StringEnumConverter))]
        public JumpPointItemType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

    }
}

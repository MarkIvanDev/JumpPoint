using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public class CloudAccountProperty : ObservableObject
    {
        public CloudAccountProperty(string name, bool isRequired, bool isSensitive)
        {
            Name = name;
            IsRequired = isRequired;
            IsSensitive = isSensitive;
        }

        public string Name { get; }

        public bool IsRequired { get; }

        public bool IsSensitive { get; }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

    }

}

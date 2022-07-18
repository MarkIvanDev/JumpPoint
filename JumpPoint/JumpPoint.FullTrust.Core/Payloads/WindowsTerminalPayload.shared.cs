using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;

namespace JumpPoint.FullTrust.Core.Payloads
{
    public class WindowsTerminalPayload
    {
        public string Paths { get; set; }

        public Collection<string> PathCollection
        {
            get { return JsonConvert.DeserializeObject<Collection<string>>(Paths); }
        }
    }
}

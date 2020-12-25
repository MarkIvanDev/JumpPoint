using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace JumpPoint.FullTrust.Core.Payloads
{
    public class CmdPayload
    {
        public string Paths { get; set; }

        public Collection<string> PathCollection
        {
            get { return JsonConvert.DeserializeObject<Collection<string>>(Paths); }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace JumpPoint.FullTrust.Core.Payloads
{
    public class PastePayload
    {
        public PasteOperation Operation { get; set; }

        public string Destination { get; set; }

        public string Paths { get; set; }

        public Collection<string> PathCollection
        {
            get { return JsonConvert.DeserializeObject<Collection<string>>(Paths); }
        }

        public PasteCollisionOption Option { get; set; }
    }

    public enum PasteOperation : uint
    {
        None = 0,
        Copy = 1,
        Move = 2,
        Link = 4
    }
}

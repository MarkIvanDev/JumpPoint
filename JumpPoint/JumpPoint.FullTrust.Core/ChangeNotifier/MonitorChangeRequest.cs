using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.FullTrust.Core.ChangeNotifier
{
    public class MonitorChangeRequest
    {
        public string Path { get; set; }

        public bool Release { get; set; }
    }
}

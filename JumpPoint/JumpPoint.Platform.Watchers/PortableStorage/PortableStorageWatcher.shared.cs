using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Watchers.PortableStorage;

namespace JumpPoint.Platform.Watchers
{
    public static partial class PortableStorageWatcher
    {
        public static event EventHandler<PortableDevicesChangedEventArgs> PortableDevicesChanged;

        public static void Start()
            => PlatformStart();

        public static void Stop()
            => PlatformStop();
    }
}

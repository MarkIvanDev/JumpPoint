﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Watchers.PortableStorage
{
    public class PortableDevicesChangedEventArgs
    {
        public PortableDevicesChangedEventArgs(CollectionChangedAction action, string deviceId)
        {
            Action = action;
            DeviceId = deviceId;
        }

        public CollectionChangedAction Action { get; }

        public string DeviceId { get; }
    }

}

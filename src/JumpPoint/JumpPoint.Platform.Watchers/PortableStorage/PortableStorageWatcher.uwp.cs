using System;
using System.Text;
using JumpPoint.Platform.Watchers.PortableStorage;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;

namespace JumpPoint.Platform.Watchers
{
    public static partial class PortableStorageWatcher
    {
        private static DeviceWatcher deviceWatcher = null;

        static void PlatformStart()
        {
            deviceWatcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());
            deviceWatcher.Added += OnDeviceAdded;
            deviceWatcher.Removed += OnDeviceRemoved;
            deviceWatcher.Updated += OnDeviceUpdated;
            deviceWatcher.EnumerationCompleted += OnEnumerationCompleted;
            deviceWatcher.Start();
        }

        private static void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            PortableDevicesChanged?.Invoke(null,
                new PortableDevicesChangedEventArgs(CollectionChangedAction.Add, args.Id));
        }

        private static void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            PortableDevicesChanged?.Invoke(null,
                new PortableDevicesChangedEventArgs(CollectionChangedAction.Remove, args.Id));
        }

        private static void OnDeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            PortableDevicesChanged?.Invoke(null,
                new PortableDevicesChangedEventArgs(CollectionChangedAction.Update, args.Id));
        }

        private static void OnEnumerationCompleted(DeviceWatcher sender, object args)
        {
            PortableDevicesChanged?.Invoke(null,
                new PortableDevicesChangedEventArgs(CollectionChangedAction.Reset, null));
        }

        static void PlatformStop()
        {
            if (deviceWatcher != null)
            {
                deviceWatcher.Added -= OnDeviceAdded;
                deviceWatcher.Removed -= OnDeviceRemoved;
                deviceWatcher.Updated -= OnDeviceUpdated;
                deviceWatcher.EnumerationCompleted -= OnEnumerationCompleted;
                if (deviceWatcher.Status == DeviceWatcherStatus.Started || deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted)
                {
                    deviceWatcher.Stop();
                }
            }
        }
    }
}

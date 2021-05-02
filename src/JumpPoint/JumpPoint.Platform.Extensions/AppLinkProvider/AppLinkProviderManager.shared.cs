﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Models;
using NittyGritty.Models;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {

        public static event ExtensionInstalledEventHandler ExtensionInstalled;
        public static event ExtensionUpdatedEventHandler ExtensionUpdated;
        public static event ExtensionUpdatingEventHandler ExtensionUpdating;
        public static event ExtensionUninstallingEventHandler ExtensionUninstalling;
        public static event ExtensionStatusChangedEventHandler ExtensionStatusChanged;

        public static void Start()
            => PlatformStart();

        public static void Stop()
            => PlatformStop();

        public static async Task<IList<AppLinkProvider>> GetProviders()
            => await PlatformGetProviders();

        public static async Task<AppLinkInfo> Pick(AppLinkProvider provider)
            => await PlatformPick(provider);

        public static async Task<IList<AppLinkPayload>> GetLocalAppLinks()
            => await PlatformGetLocalAppLinks();

    }

    public class ExtensionInstalledEventArgs
    {
        public ExtensionInstalledEventArgs(IList<AppLinkProvider> extensions)
        {
            Extensions = new ReadOnlyCollection<AppLinkProvider>(extensions);
        }

        public IReadOnlyList<AppLinkProvider> Extensions { get; }
    }

    public delegate void ExtensionInstalledEventHandler(object sender, ExtensionInstalledEventArgs args);

    public class ExtensionUpdatedEventArgs
    {
        public ExtensionUpdatedEventArgs(IList<AppLinkProvider> extensions)
        {
            Extensions = new ReadOnlyCollection<AppLinkProvider>(extensions);
        }

        public IReadOnlyList<AppLinkProvider> Extensions { get; }
    }

    public delegate void ExtensionUpdatedEventHandler(object sender, ExtensionUpdatedEventArgs args);

    public class ExtensionUpdatingEventArgs
    {
        public ExtensionUpdatingEventArgs(string packageId)
        {
            PackageId = packageId;
        }

        public string PackageId { get; }
    }

    public delegate void ExtensionUpdatingEventHandler(object sender, ExtensionUpdatingEventArgs args);


    public class ExtensionUninstallingEventArgs
    {
        public ExtensionUninstallingEventArgs(string packageId)
        {
            PackageId = packageId;
        }

        public string PackageId { get; }
    }

    public delegate void ExtensionUninstallingEventHandler(object sender, ExtensionUninstallingEventArgs args);


    public class ExtensionStatusChangedEventArgs
    {
        public ExtensionStatusChangedEventArgs(string packageId, bool? isAvailable)
        {
            PackageId = packageId;
            IsAvailable = isAvailable;
        }

        public string PackageId { get; }
        public bool? IsAvailable { get; }
    }

    public delegate void ExtensionStatusChangedEventHandler(object sender, ExtensionStatusChangedEventArgs args);

}

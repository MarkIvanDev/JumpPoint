using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.FullTrust.Core;
using JumpPoint.FullTrust.Core.Payloads;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class DesktopService
    {
        private static readonly ApplicationDataContainer LocalSettings;

        static DesktopService()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        static async Task PlatformOpen(IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(OpenPayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(paths))
                };
                LocalSettings.Values[nameof(OpenPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.Open);
            }
        }

        static async Task PlatformPaste(string destination)
        {
            if (PlatformIsSupported())
            {
                var content = Clipboard.GetContent();
                if (content.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await content.GetStorageItemsAsync();
                    var data = new ApplicationDataCompositeValue
                    {
                        [nameof(PastePayload.Operation)] = (uint)(PasteOperation)content.RequestedOperation,
                        [nameof(PastePayload.Destination)] = destination,
                        [nameof(PastePayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(items.Select(i => i.Path).ToList())),
                        [nameof(PastePayload.Option)] = (int)PasteCollisionOption.LetMeDecide
                    };
                    LocalSettings.Values[nameof(PastePayload)] = data;
                    await PlatformLaunchFullTrust(DesktopParameter.Paste);
                }
            }
        }

        static async Task PlatformCopyTo(string destination, IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(PastePayload.Operation)] = (uint)PasteOperation.Copy,
                    [nameof(PastePayload.Destination)] = destination,
                    [nameof(PastePayload.Paths)] = JsonConvert.SerializeObject(paths),
                    [nameof(PastePayload.Option)] = (int)PasteCollisionOption.LetMeDecide
                };
                LocalSettings.Values[nameof(PastePayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.Paste);
            }
        }

        static async Task PlatformMoveTo(string destination, IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(PastePayload.Operation)] = (uint)PasteOperation.Move,
                    [nameof(PastePayload.Destination)] = destination,
                    [nameof(PastePayload.Paths)] = JsonConvert.SerializeObject(paths),
                    [nameof(PastePayload.Option)] = (int)PasteCollisionOption.LetMeDecide
                };
                LocalSettings.Values[nameof(PastePayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.Paste);
            }
        }

        static async Task PlatformDelete(IList<string> paths, bool deletePermanently)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(DeletePayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(paths)),
                    [nameof(DeletePayload.IsPermanent)] = deletePermanently
                };
                LocalSettings.Values[nameof(DeletePayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.Delete);
            }
        }

        static async Task PlatformOpenCleanManager(char? driveLetter = null)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(CleanMgrPayload.DriveLetter)] = driveLetter?.ToString()
                };
                LocalSettings.Values[nameof(CleanMgrPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.CleanManager);
            }
        }

        static async Task PlatformOpenInCommandPrompt(IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(CmdPayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(paths))
                };
                LocalSettings.Values[nameof(CmdPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.CommandPrompt);
            }
        }

        static async Task PlatformOpenInPowershell(IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(PowershellPayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(paths))
                };
                LocalSettings.Values[nameof(PowershellPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.Powershell);
            }
        }

        static async Task PlatformOpenInWindowsTerminal(IList<string> paths)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(WindowsTerminalPayload.Paths)] = JsonConvert.SerializeObject(new Collection<string>(paths))
                };
                LocalSettings.Values[nameof(WindowsTerminalPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.WindowsTerminal);
            }
        }

        static async Task PlatformOpenSystemApp(string app, string arguments = null)
        {
            if (PlatformIsSupported())
            {
                var data = new ApplicationDataCompositeValue
                {
                    [nameof(SysAppPayload.App)] = app,
                    [nameof(SysAppPayload.Arguments)] = arguments
                };
                LocalSettings.Values[nameof(SysAppPayload)] = data;
                await PlatformLaunchFullTrust(DesktopParameter.SystemApps);
            }
        }

        static bool PlatformIsSupported()
        {
            return ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0);
        }

        static async Task PlatformLaunchFullTrust(string parameter = null)
        {
            if (parameter != null)
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync(parameter);
            }
            else
            {
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }
    }
}

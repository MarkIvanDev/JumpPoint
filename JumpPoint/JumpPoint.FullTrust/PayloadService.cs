using System;
using JumpPoint.FullTrust.Core;
using JumpPoint.FullTrust.Core.Payloads;
using Windows.Storage;

namespace JumpPoint.FullTrust
{
    public static class PayloadService
    {
        private static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static OpenPayload? GetOpenPayload()
        {
            if (LocalSettings.Values[nameof(OpenPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(OpenPayload));
                return new OpenPayload
                {
                    Paths = (string)payload[nameof(OpenPayload.Paths)]
                };
            }
            return null;
        }

        public static PastePayload? GetPastePayload()
        {
            if (LocalSettings.Values[nameof(PastePayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(PastePayload));
                return new PastePayload()
                {
                    Operation = (PasteOperation)payload[nameof(PastePayload.Operation)],
                    Destination = (string)payload[nameof(PastePayload.Destination)],
                    Paths = (string)payload[nameof(PastePayload.Paths)],
                    Option = (PasteCollisionOption)payload[nameof(PastePayload.Option)]
                };
            }
            return null;
        }

        public static DeletePayload? GetDeletePayload()
        {
            if (LocalSettings.Values[nameof(DeletePayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(DeletePayload));
                return new DeletePayload()
                {
                    Paths = (string)payload[nameof(DeletePayload.Paths)],
                    IsPermanent = (bool)payload[nameof(DeletePayload.IsPermanent)]
                };
            }
            return null;
        }

        public static CmdPayload? GetCmdPayload()
        {
            if (LocalSettings.Values[nameof(CmdPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(CmdPayload));
                return new CmdPayload()
                {
                    Paths = (string)payload[nameof(CmdPayload.Paths)]
                };
            }
            return null;
        }

        public static PowershellPayload? GetPowershellPayload()
        {
            if (LocalSettings.Values[nameof(PowershellPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(PowershellPayload));
                return new PowershellPayload()
                {
                    Paths = (string)payload[nameof(PowershellPayload.Paths)]
                };
            }
            return null;
        }

        public static WindowsTerminalPayload? GetWindowsTerminalPayload()
        {
            if (LocalSettings.Values[nameof(WindowsTerminalPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(WindowsTerminalPayload));
                return new WindowsTerminalPayload()
                {
                    Paths = (string)payload[nameof(WindowsTerminalPayload.Paths)]
                };
            }
            return null;
        }

        public static CleanMgrPayload? GetCleanMgrPayload()
        {
            if (LocalSettings.Values[nameof(CleanMgrPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(CleanMgrPayload));
                return new CleanMgrPayload()
                {
                    DriveLetter = payload[nameof(CleanMgrPayload.DriveLetter)] as string
                };
            }
            return null;
        }

        public static SysAppPayload? GetSysAppPayload()
        {
            if (LocalSettings.Values[nameof(SysAppPayload)] is ApplicationDataCompositeValue payload)
            {
                LocalSettings.Values.Remove(nameof(SysAppPayload));
                return new SysAppPayload()
                {
                    App = (string)payload[nameof(SysAppPayload.App)],
                    Arguments = payload[nameof(SysAppPayload.Arguments)] as string
                };
            }
            return null;
        }
    }
}

using System;
using System.Diagnostics;
using JumpPoint.FullTrust.Core;
using JumpPoint.FullTrust.Core.Payloads;
using JumpPoint.FullTrust.IO;
using JumpPoint.FullTrust.IO.Interop;

namespace JumpPoint.FullTrust
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                // launch process based on parameter
                switch (args[2])
                {
                    case "/open":
                        Open();
                        break;

                    case "/paste":
                        Paste();
                        break;

                    case "/delete":
                        Delete();
                        break;

                    case "/cmd":
                        LaunchCommandPrompt();
                        break;

                    case "/powershell":
                        LaunchPowershell();
                        break;

                    case "/cleanmgr":
                        LaunchCleanManager();
                        break;

                    case "/sysapp":
                        LaunchSystemApp();
                        break;
                }
            }
        }

        static void Open()
        {
            var payload = PayloadService.GetOpenPayload();
            if (payload != null)
            {
                foreach (var path in payload.PathCollection)
                {
                    var processInfo = new ProcessStartInfo(path)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    Process.Start(processInfo);
                }
            }
        }

        static void Paste()
        {
            var payload = PayloadService.GetPastePayload();
            if (payload != null)
            {
                FileOperationFlags flags = FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOFX_ADDUNDORECORD;
                switch (payload.Option)
                {
                    case PasteCollisionOption.GenerateUniqueName:
                        flags |= FileOperationFlags.FOF_RENAMEONCOLLISION | FileOperationFlags.FOFX_PRESERVEFILEEXTENSIONS;
                        break;
                    case PasteCollisionOption.ReplaceExisiting:
                        flags |= FileOperationFlags.FOF_NOCONFIRMATION;
                        break;
                    case PasteCollisionOption.DoNothing:
                        flags |= FileOperationFlags.FOF_NOERRORUI;
                        break;
                    case PasteCollisionOption.KeepNewer:
                        flags |= FileOperationFlags.FOFX_KEEPNEWERFILE;
                        break;
                    case PasteCollisionOption.LetMeDecide:
                        break;
                }
                using (var fileOperation = new FileOperation(flags))
                {
                    foreach (var path in payload.PathCollection)
                    {
                        if (payload.Operation == PasteOperation.Copy)
                        {
                            fileOperation.CopyItem(path, payload.Destination, null);
                        }
                        else if (payload.Operation == PasteOperation.Move)
                        {
                            fileOperation.MoveItem(path, payload.Destination, null);
                        }
                    }

                    fileOperation.PerformOperations();
                }
            }
        }

        static void Delete()
        {
            var payload = PayloadService.GetDeletePayload();
            if (payload != null)
            {
                var flags = payload.IsPermanent ?
                    FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOFX_ADDUNDORECORD : //  | FileOperationFlags.FOF_WANTNUKEWARNING
                    FileOperationFlags.FOF_NOCONFIRMMKDIR | FileOperationFlags.FOFX_ADDUNDORECORD | FileOperationFlags.FOFX_RECYCLEONDELETE;

                using (var fileOperation = new FileOperation(flags))
                {
                    foreach (var path in payload.PathCollection)
                    {
                        fileOperation.DeleteItem(path);
                    }
                    fileOperation.PerformOperations();
                }
            }
        }

        static void LaunchCommandPrompt()
        {
            var payload = PayloadService.GetCmdPayload();
            if (payload != null)
            {
                foreach (var path in payload.PathCollection)
                {
                    Process.Start(SystemApp.CommandPrompt, $"/k \"pushd {path}\"");
                }
            }
        }

        static void LaunchPowershell()
        {
            var payload = PayloadService.GetPowershellPayload();
            if (payload != null)
            {
                foreach (var path in payload.PathCollection)
                {
                    Process.Start(SystemApp.Powershell, $"-noexit -command \"cd \'{path}\'\"");
                }
            }
        }

        static void LaunchCleanManager()
        {
            var payload = PayloadService.GetCleanMgrPayload();
            if (payload != null)
            {
                if (payload.DriveLetter != null)
                {
                    Process.Start(SystemApp.CleanManager, $"/D {payload.DriveLetter}");
                }
                else
                {
                    Process.Start(SystemApp.CleanManager);
                }
            }
        }

        static void LaunchSystemApp()
        {
            var payload = PayloadService.GetSysAppPayload();
            if (payload != null)
            {
                if (payload.Arguments == null)
                {
                    Process.Start(new ProcessStartInfo(payload.App)
                    {
                        Verb = "runas"
                    });
                }
                else
                {
                    Process.Start(payload.App, payload.Arguments);
                }
            }
        }
    }
}

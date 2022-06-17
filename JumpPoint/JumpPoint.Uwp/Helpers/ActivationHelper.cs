using JumpPoint.Platform;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Utilities;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace JumpPoint.Uwp.Helpers
{
    public static class ActivationHelper
    {
        public static AppPath GetAppPath(CommandLineActivatedEventArgs args)
        {
            var appPath = AppPath.Unknown;
            GetRootCommand().Invoke(args.Operation.Arguments);
            return appPath;

            RootCommand GetRootCommand()
            {
                var rootCommand = new RootCommand()
                {
                    new Command(".")
                    {
                        Handler = CommandHandler.Create(() =>
                        {
                            appPath = AppPath.Folder;
                        })
                    },
                    GetCommand(CommandLinePath.Dashboard),
                    GetCommand(CommandLinePath.Settings),
                    GetCommand(CommandLinePath.Favorites),
                    GetCommand(CommandLinePath.Drives),
                    GetCommand(CommandLinePath.CloudDrives),
                    GetCommand(CommandLinePath.Workspaces),
                    GetCommand(CommandLinePath.AppLinks),

                    GetCommand(CommandLinePath.Open),
                    GetCommand(CommandLinePath.Drive),
                    GetCommand(CommandLinePath.Folder),
                    GetCommand(CommandLinePath.Workspace),
                    GetCommand(CommandLinePath.Cloud),

                    GetCommand(CommandLinePath.Properties),
                };
                rootCommand.Handler = CommandHandler.Create(() =>
                {
                    appPath = AppPath.Dashboard;
                });
                rootCommand.Name = Prefix.MAIN_SCHEME;
                return rootCommand;
            }

            Command GetCommand(CommandLinePath clPath)
            {
                switch (clPath)
                {
                    case CommandLinePath.Dashboard:
                    case CommandLinePath.Settings:
                    case CommandLinePath.Favorites:
                    case CommandLinePath.Drives:
                    case CommandLinePath.CloudDrives:
                    case CommandLinePath.Workspaces:
                    case CommandLinePath.AppLinks:
                        return new Command(clPath.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create(() =>
                            {
                                appPath = clPath.ToAppPath();
                            })
                        };

                    case CommandLinePath.Open:
                        var openCommand = new Command(clPath.ToString().ToLower())
                        {
                            new Argument<string>("path", () => "Dashboard")
                            {
                                Arity = ArgumentArity.ExactlyOne
                            }
                        };
                        openCommand.Handler = CommandHandler.Create<string>((path) =>
                        {
                            var pathKind = path.GetPathKind();
                            var lastCrumb = path.GetBreadcrumbs().LastOrDefault();
                            switch (pathKind)
                            {
                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Drive:
                                    appPath = AppPath.Drive;
                                    break;

                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Folder:
                                    appPath = AppPath.Folder;
                                    break;

                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Cloud:
                                    if (EnumHelper<CloudStorageProvider>.ParseOrDefault(lastCrumb?.DisplayName, ignoreCase: true) != CloudStorageProvider.Unknown)
                                    {
                                        appPath = AppPath.Cloud;
                                    }
                                    else
                                    {
                                        appPath = AppPath.CloudDrives;
                                    }
                                    break;

                                case PathKind.Workspace:
                                    if (lastCrumb?.AppPath == AppPath.Workspace)
                                    {
                                        appPath = AppPath.Workspace;
                                    }
                                    else
                                    {
                                        appPath = AppPath.Workspaces;
                                    }
                                    break;

                                case PathKind.AppLink:
                                    appPath = AppPath.AppLinks;
                                    break;

                                case PathKind.Local:
                                    appPath = lastCrumb?.AppPath ?? AppPath.Dashboard;
                                    break;

                                case PathKind.Unknown:
                                default:
                                    appPath = AppPath.Dashboard;
                                    break;
                            }
                        });
                        return openCommand;

                    case CommandLinePath.Drive:
                    case CommandLinePath.Folder:
                    case CommandLinePath.Workspace:
                        var itemCommand = new Command(clPath.ToString().ToLower())
                        {
                            new Argument<string>("path")
                            {
                                Arity = ArgumentArity.ExactlyOne
                            }
                        };
                        itemCommand.Handler = CommandHandler.Create<string>((path) =>
                        {
                            appPath = clPath.ToAppPath();
                        });
                        return itemCommand;

                    case CommandLinePath.Cloud:
                        var cloudCommand = new Command(clPath.ToString().ToLower())
                        {
                            new Argument<string>("provider")
                            {
                                Arity = ArgumentArity.ExactlyOne
                            }
                        };
                        cloudCommand.Handler = CommandHandler.Create<string>((provider) =>
                        {
                            if (EnumHelper<CloudStorageProvider>.ParseOrDefault(provider, ignoreCase: true) != CloudStorageProvider.Unknown)
                            {
                                appPath = AppPath.Cloud;
                            }
                            else
                            {
                                appPath = AppPath.CloudDrives;
                            }
                        });
                        return cloudCommand;

                    case CommandLinePath.Properties:
                        {
                            var command = new Command("properties")
                            {
                                new Option<string>(new string[] { "--type", "-t" })
                                {
                                    Argument = new Argument<string>("type")
                                    {
                                        Arity = ArgumentArity.ExactlyOne
                                    }
                                },
                                new Option<string>(new string[] { "--path", "-p" })
                                {
                                    Argument = new Argument<string>("path")
                                    {
                                        Arity = ArgumentArity.ExactlyOne
                                    }
                                },
                                new Option<string>(new string[] { "--seeds", "-s" })
                                {
                                    Argument = new Argument<string>("seeds")
                                    {
                                        Arity = ArgumentArity.ExactlyOne
                                    }
                                }
                            };
                            command.Handler = CommandHandler.Create<string, string, string>((type, path, seeds) =>
                            {
                                appPath = AppPath.Properties;
                            });
                            return command;
                        }

                    case CommandLinePath.Unknown:
                    default:
                        return null;
                }
            }
        }
    }
}

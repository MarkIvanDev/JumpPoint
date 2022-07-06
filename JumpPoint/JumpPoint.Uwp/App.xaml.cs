using System;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Glif.Pickers;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.Uwp.Helpers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NittyGritty;
using NittyGritty.Models;
using NittyGritty.Platform.Contacts;
using NittyGritty.Platform.Payloads;
using NittyGritty.Uwp.Services;
using NittyGritty.Utilities;
using NittyGritty.Uwp;
using NittyGritty.Uwp.Extensions.Activation;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Networking.NetworkOperators;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JumpListService = JumpPoint.Platform.Services.JumpListService;
using GalaSoft.MvvmLight.Ioc;

namespace JumpPoint.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : NGApp
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            UnhandledException += OnUnhandledException;

            SQLitePCL.Batteries_V2.Init();

            SetupAppCenter();
        }

        private void SetupAppCenter()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appcenter.jps.json", optional: true)
                .Build();
#if BETA
            AppCenter.Start(config["betaKey"], typeof(Analytics), typeof(Crashes));
#else
            AppCenter.Start(config["mainKey"], typeof(Analytics), typeof(Crashes));
#endif
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            if(e.Exception is UnauthorizedAccessException uae)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(e.Exception, e.Exception.Message), MessengerTokens.ExceptionManagement);
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(e.Exception, e.Exception.Message), MessengerTokens.ExceptionManagement);
            }
        }

        #region Activation
        protected override async Task HandleActivation(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                await SuspensionManager.RestoreAsync();
            }

            var source = args.GetLaunchSource();
            switch (source)
            {
                case LaunchSource.Primary:
                    NavigateShell(new QueryString()
                    {
                        { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                        { nameof(PathInfo.Path), nameof(AppPath.Dashboard) }
                    }.ToString());
                    break;
                case LaunchSource.Secondary:
                case LaunchSource.Jumplist:
                    NavigateShell(args.Arguments);
                    break;
                case LaunchSource.Chaseable:
                    break;
                default:
                    break;
            }

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(new Size(400, 400));
        }

        protected override async Task HandleActivation(ProtocolActivatedEventArgs args)
        {
            var rootFrame = GetNavigationContext();

            var host = EnumHelper<ProtocolPath>.ParseOrDefault(args.Uri.Host, ignoreCase: true);
            var appPath = host.ToAppPath();
            var query = QueryString.Parse(args.Uri.Query.TrimStart('?'));
            switch (host)
            {
                case ProtocolPath.Dashboard:
                case ProtocolPath.Settings:
                case ProtocolPath.Favorites:
                case ProtocolPath.Drives:
                case ProtocolPath.CloudDrives:
                case ProtocolPath.Workspaces:
                case ProtocolPath.AppLinks:
                    NavigateShell(new QueryString()
                    {
                        { nameof(PathInfo.Type), appPath.ToString() },
                        { nameof(PathInfo.Path), appPath.ToString() }
                    }.ToString());
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    break;

                case ProtocolPath.Open:
                    {
                        if (query.TryGetValue("path", out var path))
                        {
                            var pathKind = path.GetPathKind();
                            var lastCrumb = path.GetBreadcrumbs().LastOrDefault();
                            switch (pathKind)
                            {
                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Drive:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                                        { nameof(PathInfo.Path), path }
                                    }.ToString());
                                    break;

                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Folder:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Folder.ToString() },
                                        { nameof(PathInfo.Path), path }
                                    }.ToString());
                                    break;

                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Cloud:
                                    if (Enum.TryParse<CloudStorageProvider>(lastCrumb?.DisplayName, true, out _))
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.Cloud.ToString() },
                                            { nameof(PathInfo.Path), path }
                                        }.ToString());
                                    }
                                    else
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.CloudDrives.ToString() },
                                            { nameof(PathInfo.Path), AppPath.CloudDrives.ToString() }
                                        }.ToString());
                                    }
                                    break;

                                case PathKind.Workspace:
                                    if (lastCrumb?.AppPath == AppPath.Workspace)
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.Workspace.ToString() },
                                            { nameof(PathInfo.Path), path }
                                        }.ToString());
                                    }
                                    else
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.Workspaces.ToString() },
                                            { nameof(PathInfo.Path), AppPath.Workspaces.ToString() }
                                        }.ToString());
                                    }
                                    break;

                                case PathKind.AppLink:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.AppLinks.ToString() },
                                        { nameof(PathInfo.Path), AppPath.AppLinks.ToString() }
                                    }.ToString());
                                    break;

                                case PathKind.Local:
                                    var localPath = lastCrumb?.AppPath ?? AppPath.Dashboard;
                                    if (localPath == AppPath.Chat)
                                    {
                                        Navigate<Hosted.ChatbotPage>();
                                    }
                                    else
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), localPath.ToString() },
                                            { nameof(PathInfo.Path), localPath.ToString() }
                                        }.ToString());
                                    }
                                    break;

                                case PathKind.Unknown:
                                default:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                                        { nameof(PathInfo.Path), AppPath.Dashboard.ToString() }
                                    }.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                                { nameof(PathInfo.Path), AppPath.Dashboard.ToString() }
                            }.ToString());
                        }
                        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    }
                    break;

                case ProtocolPath.Drive:
                case ProtocolPath.Folder:
                    {
                        if (query.TryGetValue("path", out var path))
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), host.ToAppPath().ToString() },
                                { nameof(PathInfo.Path), path }
                            }.ToString());
                        }
                        else
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), AppPath.Drives.ToString() },
                                { nameof(PathInfo.Path), AppPath.Drives.ToString() }
                            }.ToString());
                        }
                        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    }
                    break;

                case ProtocolPath.Workspace:
                    {
                        if (query.TryGetValue("path", out var path))
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), host.ToAppPath().ToString() },
                                { nameof(PathInfo.Path), path }
                            }.ToString());
                        }
                        else
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), AppPath.Workspaces.ToString() },
                                { nameof(PathInfo.Path), AppPath.Workspaces.ToString() }
                            }.ToString());
                        }
                        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    }
                    break;

                case ProtocolPath.Cloud:
                    {
                        if (query.TryGetValue("provider", out var prov) && Enum.TryParse<CloudStorageProvider>(prov, true, out var provider))
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), AppPath.Cloud.ToString() },
                                { nameof(PathInfo.Path), $@"cloud:\{provider}" }
                            }.ToString());
                        }
                        else
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), AppPath.CloudDrives.ToString() },
                                { nameof(PathInfo.Path), AppPath.CloudDrives.ToString() }
                            }.ToString());
                        }
                        ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    }
                    break;

                case ProtocolPath.Properties:
                    {
                        if (query.TryGetValue("type", out var type) &&
                            Enum.TryParse<JumpPointItemType>(type, true, out var itemType) &&
                            query.TryGetValue("path", out var path))
                        {
                            rootFrame.Navigate(typeof(Standalone.PropertiesPage),
                                JsonConvert.SerializeObject(new Collection<Seed>()
                                {
                                    new Seed() { Type = itemType, Path = path }
                                }));
                        }
                        else if (query.TryGetValue("seedsToken", out var seedsToken))
                        {
                            try
                            {
                                var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(seedsToken);
                                var json = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                                await file.DeleteAsync();
                                rootFrame.Navigate(typeof(Standalone.PropertiesPage), json);
                            }
                            catch
                            {
                                rootFrame.Navigate(typeof(Standalone.PropertiesPage), "[]");
                            }
                        }
                        else if (query.TryGetValue("fileToken", out var fileToken))
                        {
                            try
                            {
                                var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(fileToken);
                                rootFrame.Navigate(typeof(Standalone.PropertiesPage),
                                    JsonConvert.SerializeObject(new Collection<Seed>()
                                    {
                                    new Seed() { Type = JumpPointItemType.File, Path = file.Path }
                                    }));
                            }
                            catch
                            {
                                rootFrame.Navigate(typeof(Standalone.PropertiesPage), "[]");
                            }
                        }
                    }
                    break;

                case ProtocolPath.Chat:
                    Navigate<Hosted.ChatbotPage>();
                    break;

                case ProtocolPath.Clipboard:
                    Navigate<Standalone.ClipboardManagerPage>();
                    break;
                
                case ProtocolPath.Unknown:
                default:
                    NavigateShell(new QueryString()
                    {
                        { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                        { nameof(PathInfo.Path), AppPath.Dashboard.ToString() }
                    }.ToString());
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                    break;
            }

            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(new Size(400, 400));
        }

        protected override async Task HandleActivation(CommandLineActivatedEventArgs args)
        {
            var deferral = args.Operation.GetDeferral();
            var rootFrame = GetNavigationContext();
            await GetRootCommand().InvokeAsync(args.Operation.Arguments);
            deferral.Complete();

            RootCommand GetRootCommand()
            {
                var rootCommand = new RootCommand()
                {
                    new Command(".")
                    {
                        Handler = CommandHandler.Create(async () =>
                        {
                            try
                            {
                                var currentDirectory = await StorageService.GetDirectory(args.Operation.CurrentDirectoryPath);
                                NavigateShell(new QueryString()
                                {
                                    { nameof(PathInfo.Type), currentDirectory.Type == JumpPointItemType.Drive ? AppPath.Drive.ToString() : AppPath.Folder.ToString() },
                                    { nameof(PathInfo.Path), currentDirectory.Path }
                                }.ToString());
                                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                            }
                            catch (Exception)
                            {
                                NavigateShell(new QueryString()
                                {
                                    { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                                    { nameof(PathInfo.Path), nameof(AppPath.Dashboard) }
                                }.ToString());
                                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                            }
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

                    GetCommand(CommandLinePath.Properties)
                };
                rootCommand.Handler = CommandHandler.Create(() =>
                {
                    NavigateShell(new QueryString()
                    {
                        { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                        { nameof(PathInfo.Path), AppPath.Dashboard.ToString() }
                    }.ToString());
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
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
                                NavigateShell(new QueryString()
                                {
                                    { nameof(PathInfo.Type), clPath.ToString() },
                                    { nameof(PathInfo.Path), clPath.ToString() }
                                }.ToString());
                                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                            })
                        };

                    case CommandLinePath.Open:
                    {
                        var command = new Command(clPath.ToString().ToLower())
                        {
                            new Argument<string>("path", () => "Dashboard")
                            {
                                Arity = ArgumentArity.ExactlyOne
                            }
                        };
                        command.Handler = CommandHandler.Create<string>((path) =>
                        {
                            var pathKind = path.GetPathKind();
                            var lastCrumb = path.GetBreadcrumbs().LastOrDefault();
                            switch (pathKind)
                            {
                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Drive:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Drive:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                                        { nameof(PathInfo.Path), path }
                                    }.ToString());
                                    break;

                                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Network when lastCrumb?.AppPath == AppPath.Folder:
                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Folder:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Folder.ToString() },
                                        { nameof(PathInfo.Path), path }
                                    }.ToString());
                                    break;

                                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Cloud:
                                    if (EnumHelper<CloudStorageProvider>.ParseOrDefault(lastCrumb?.DisplayName, ignoreCase: true) != CloudStorageProvider.Unknown)
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.Cloud.ToString() },
                                            { nameof(PathInfo.Path), path }
                                        }.ToString());
                                    }
                                    else
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), AppPath.CloudDrives.ToString() },
                                            { nameof(PathInfo.Path), AppPath.CloudDrives.ToString() }
                                        }.ToString());
                                    }
                                    break;

                                case PathKind.Workspace when lastCrumb?.AppPath == AppPath.Workspace:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Workspace.ToString() },
                                        { nameof(PathInfo.Path), path }
                                    }.ToString());
                                    break;

                                case PathKind.Workspace:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Workspaces.ToString() },
                                        { nameof(PathInfo.Path), AppPath.Workspaces.ToString() }
                                    }.ToString());
                                    break;

                                case PathKind.AppLink:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.AppLinks.ToString() },
                                        { nameof(PathInfo.Path), AppPath.AppLinks.ToString() }
                                    }.ToString());
                                    break;

                                case PathKind.Local:
                                    var localPath = lastCrumb?.AppPath ?? AppPath.Dashboard;
                                    if (localPath == AppPath.Chat)
                                    {
                                        Navigate<Hosted.ChatbotPage>();
                                    }
                                    else if (localPath == AppPath.ClipboardManager)
                                    {
                                        Navigate<Standalone.ClipboardManagerPage>();
                                    }
                                    else
                                    {
                                        NavigateShell(new QueryString()
                                        {
                                            { nameof(PathInfo.Type), localPath.ToString() },
                                            { nameof(PathInfo.Path), localPath.ToString() }
                                        }.ToString());
                                    }
                                    break;

                                case PathKind.Unknown:
                                default:
                                    NavigateShell(new QueryString()
                                    {
                                        { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                                        { nameof(PathInfo.Path), AppPath.Dashboard.ToString() }
                                    }.ToString());
                                    break;
                            }
                            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                        });
                        return command;
                    }

                    case CommandLinePath.Drive:
                    case CommandLinePath.Folder:
                    case CommandLinePath.Workspace:
                    {
                        var command = new Command(clPath.ToString().ToLower())
                        {
                            new Option<string>(new string[] { "--path", "-p" })
                            {
                                IsRequired = true,
                                Argument = new Argument<string>("path")
                                {
                                    Arity = ArgumentArity.ExactlyOne
                                }
                            }
                        };
                        command.Handler = CommandHandler.Create<string>((path) =>
                        {
                            NavigateShell(new QueryString()
                            {
                                { nameof(PathInfo.Type), clPath.ToString() },
                                { nameof(PathInfo.Path), path }
                            }.ToString());
                            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                        });
                        return command;
                    }

                    case CommandLinePath.Cloud:
                    {
                        var command = new Command(clPath.ToString().ToLower())
                        {
                            new Option<string>(new string[] { "--provider" })
                            {
                                IsRequired = true,
                                Argument = new Argument<string>("provider")
                                {
                                    Arity = ArgumentArity.ExactlyOne
                                }
                            }
                        };
                        command.Handler = CommandHandler.Create<string>((provider) =>
                        {
                            if (Enum.TryParse<CloudStorageProvider>(provider, true, out var prov) && prov != CloudStorageProvider.Unknown)
                            {
                                NavigateShell(new QueryString
                                {
                                    { nameof(PathInfo.Type), AppPath.Cloud.ToString() },
                                    { nameof(PathInfo.Path), $@"cloud:\{prov}" }
                                }.ToString());
                            }
                            else
                            {
                                NavigateShell(new QueryString()
                                {
                                    { nameof(PathInfo.Type), AppPath.CloudDrives.ToString() },
                                    { nameof(PathInfo.Path), AppPath.CloudDrives.ToString() }
                                }.ToString());
                            }
                            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
                        });
                        return command;
                    }

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
                            command.Handler = CommandHandler.Create<string, string, string>(async (type, path, seeds) =>
                            {
                                if (type != null && Enum.TryParse<JumpPointItemType>(type, true, out var itemType) && path != null)
                                {
                                    rootFrame.Navigate(typeof(Standalone.PropertiesPage),
                                        JsonConvert.SerializeObject(new Collection<Seed>()
                                        {
                                            new Seed() { Type = itemType, Path = path }
                                        }));
                                }
                                else if (seeds != null)
                                {
                                    try
                                    {
                                        var file = await StorageFile.GetFileFromPathAsync(seeds);
                                        var json = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                                        rootFrame.Navigate(typeof(Standalone.PropertiesPage), json);
                                    }
                                    catch
                                    {
                                        rootFrame.Navigate(typeof(Standalone.PropertiesPage), "[]");
                                    }
                                }
                            });
                            return command;
                        }

                    case CommandLinePath.Unknown:
                    default:
                        return null;
                }
            }
        }

        protected override Task HandleActivation(ShareTargetActivatedEventArgs args)
        {
            var rootFrame = GetNavigationContext();
            var payload = new ShareTargetPayload(args.ShareOperation);
            if (args.ShareOperation.Data.Contains(StandardDataFormats.ApplicationLink))
            {
                rootFrame.Navigate(typeof(Hosted.ShareAppLinkPage), payload);
            }
            return Task.CompletedTask;
        }

        protected override Task HandleActivation(ProtocolForResultsActivatedEventArgs args)
        {
            var rootFrame = GetNavigationContext();
            var payload = new ProtocolForResultsPayload(args.Uri, args.Data, args.ProtocolForResultsOperation);

            switch (args.Uri.Scheme)
            {
                case Prefix.PICKER_SCHEME:
                    {
                        var host = Enum.TryParse<PickerPath>(args.Uri.Host, true, out var pickerPath) ? pickerPath : PickerPath.Unknown;
                        switch (host)
                        {
                            case PickerPath.ManualAppLink:
                                rootFrame.Navigate(typeof(Hosted.ManualAppLinkPickerPage), payload);
                                break;

                            case PickerPath.AppLinkProvider:
                                rootFrame.Navigate(typeof(Hosted.AppLinkProviderPage), payload);
                                break;

                            case PickerPath.Unknown:
                            default:
                                break;
                        }
                    }
                    break;

                case Prefix.TOOL_SCHEME:
                    {
                        var host = Enum.TryParse<ToolPath>(args.Uri.Host, true, out var toolPath) ? toolPath : ToolPath.Unknown;
                        switch (host)
                        {
                            case ToolPath.Hash:
                                rootFrame.Navigate(typeof(Hosted.HashToolPage), payload);
                                break;

                            case ToolPath.Unknown:
                            default:
                                break;
                        }
                    }
                    break;

                default:
                    break;
            }

            
            return Task.CompletedTask;
        }

        protected override Task HandleActivation(ContactPanelActivatedEventArgs args)
        {
            var rootFrame = GetNavigationContext();
            args.ContactPanel.LaunchFullAppRequested += ContactPanel_LaunchFullAppRequested;

            rootFrame.Navigate(typeof(Hosted.ChatbotPage), args.Contact.ToNGContact());
            return Task.CompletedTask;
        }

        private async void ContactPanel_LaunchFullAppRequested(ContactPanel sender, ContactPanelLaunchFullAppRequestedEventArgs args)
        {
            args.Handled = true;
            var dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (dispatcher != null)
            {
                await dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        await JumpPointService.OpenNewWindow(AppPath.Dashboard, null);
                        sender.ClosePanel();
                    });
            }
        }

        protected override Task HandleDesktopActivation(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.StartupTask)
            {
                NavigateShell(new QueryString()
                {
                    { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                    { nameof(PathInfo.Path), nameof(AppPath.Dashboard) }
                }.ToString());
            }
            return Task.CompletedTask;
        }

        #endregion

        public override Frame GetNavigationContext()
        {
            return (Frame)Window.Current.Content;
        }

        private void NavigateShell(string parameter)
        {
            var rootFrame = GetNavigationContext();
            if (rootFrame.Content is TabbedShell shell)
            {
                shell.ProcessParameter(parameter);
            }
            else
            {
                rootFrame.Navigate(typeof(TabbedShell), parameter);
            }
        }

        private void Navigate<T>(object parameter = null)
        {
            var rootFrame = GetNavigationContext();
            if (rootFrame.Content is T)
            {
                // TODO: Pass parameter to current content
            }
            else
            {
                rootFrame.Navigate(typeof(T), parameter);
            }
        }

        public override async Task Initialization(IActivatedEventArgs args)
        {
            await JumpPointService.Initialize();
        }

        public override async Task Startup(IActivatedEventArgs args)
        {
            Singleton<ThemeService>.Instance.SetTheme(SimpleIoc.Default.GetInstance<AppSettings>().Theme);
            await JumpListService.Initialize();
            FontPicker.ClearFontCache();
        }

    }
}

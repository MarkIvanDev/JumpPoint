using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Services
{
    public static class ChatService
    {
        public static async Task<ChatMessage> GetResponse(string message)
        {
            var response = default(ChatMessage);
            try
            {
                var rootCommand = GetRootCommand();
                var parseResult = rootCommand.Parse(message);
                if (parseResult.Errors.Count == 0)
                {
                    await rootCommand.InvokeAsync($"{message}");
                }
                else
                {
                    response = new SimpleChatMessage(ChatMessageSource.Bot)
                    {
                        Message = "Sorry, I can't process the command"
                    };
                }
                
                return response;
            }
            catch (Exception ex)
            {
                response = new SimpleChatMessage(ChatMessageSource.Bot)
                {
                    Message = "Sorry, I can't process the command"
                };
                Debug.WriteLine(ex.Message);
                return response;
            }

            RootCommand GetRootCommand()
            {
                var rootCommand = new RootCommand()
                {
                    GetCommand(ChatbotCommand.Help),
                    GetCommand(ChatbotCommand.Open),
                    GetCommand(ChatbotCommand.List),
                };
                return rootCommand;
            }

            Command GetCommand(ChatbotCommand cmd)
            {
                switch (cmd)
                {
                    case ChatbotCommand.Help:
                        {
                            var command = new Command(cmd.ToString().ToLower())
                            {
                                new Command("open")
                                {
                                    Handler = CommandHandler.Create(() =>
                                    {
                                        response = new CommandListChatMessage(ChatMessageSource.Bot)
                                        {
                                            Title = "Open Commands",
                                            Items =
                                            {
                                                CommandInfo.OpenCommands
                                            }
                                        };
                                    })
                                },
                                new Command("list")
                                {
                                    Handler = CommandHandler.Create(() =>
                                    {
                                        response = new CommandListChatMessage(ChatMessageSource.Bot)
                                        {
                                            Title = "List Commands",
                                            Items =
                                            {
                                                CommandInfo.ListCommands
                                            }
                                        };
                                    })
                                }
                            };
                            command.Handler = CommandHandler.Create(() =>
                            {
                                response = new CommandListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = "Available Commands",
                                    Items =
                                    {
                                        CommandInfo.HelpCommands,
                                        CommandInfo.OpenCommands,
                                        CommandInfo.ListCommands
                                    }
                                };
                            });
                            return command;
                        }

                    case ChatbotCommand.Open:
                    {
                        var command = new Command(cmd.ToString().ToLower())
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
                            var uri = GetUri(pathKind, lastCrumb, path);
                            response = new OpenChatMessage(ChatMessageSource.Bot)
                            {
                                Title = lastCrumb?.DisplayName,
                                Uri = uri
                            };
                        });
                        return command;
                    }

                    case ChatbotCommand.List:
                    {
                        var command = new Command(cmd.ToString().ToLower())
                        {
                            GetListCommand(ListSubCommand.Favorites),
                            GetListCommand(ListSubCommand.Workspaces),
                            GetListCommand(ListSubCommand.Drives),
                            GetListCommand(ListSubCommand.CloudDrives),
                            GetListCommand(ListSubCommand.UserFolders),
                            GetListCommand(ListSubCommand.SystemFolders),
                            GetListCommand(ListSubCommand.Tools),
                            GetListCommand(ListSubCommand.AppLinkProviders),
                        };
                        return command;
                    }

                    case ChatbotCommand.Unknown:
                    default:
                        return null;
                }
            }

            Command GetListCommand(ListSubCommand listCmd)
            {
                switch (listCmd)
                {
                    case ListSubCommand.Favorites:
                        return new Command(listCmd.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create(async () =>
                            {
                                var faves = await DashboardService.GetFavorites();
                                response = new ItemListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = $"Favorites ({faves.Count})",
                                    Items = { faves }
                                };
                            })
                        };

                    case ListSubCommand.Workspaces:
                        {
                            return new Command(listCmd.ToString().ToLower())
                            {
                                Handler = CommandHandler.Create(async () =>
                                {
                                    var workspaces = await WorkspaceService.GetWorkspaces();
                                    response = new ItemListChatMessage(ChatMessageSource.Bot)
                                    {
                                        Title = $"Workspaces ({workspaces.Count})",
                                        Items = { workspaces }
                                    };
                                })
                            };
                        }

                    case ListSubCommand.Drives:
                        {
                            return new Command(listCmd.ToString().ToLower())
                            {
                                Handler = CommandHandler.Create(async () =>
                                {
                                    var drives = await StorageService.GetDrives();
                                    response = new ItemListChatMessage(ChatMessageSource.Bot)
                                    {
                                        Title = $"Drives ({drives.Count})",
                                        Items = { drives }
                                    };
                                })
                            };
                        }

                    case ListSubCommand.CloudDrives:
                        {
                            return new Command(listCmd.ToString().ToLower())
                            {
                                Handler = CommandHandler.Create(async () =>
                                {
                                    var drives = await CloudStorageService.GetDrives();
                                    response = new ItemListChatMessage(ChatMessageSource.Bot)
                                    {
                                        Title = $"Cloud Drives ({drives.Count})",
                                        Items = { drives }
                                    };
                                })
                            };
                        }

                    case ListSubCommand.UserFolders:
                        var ufCmd = new Command(listCmd.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create<bool>(async (all) =>
                            {
                                var folders = await DashboardService.GetUserFolders(all);
                                response = new ItemListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = $"User Folders ({folders.Count})",
                                    Items = { folders }
                                };
                            })
                        };
                        ufCmd.AddOption(new Option<bool>(new string[] { "--all", "-a" }));
                        return ufCmd;

                    case ListSubCommand.SystemFolders:
                        var sfCmd = new Command(listCmd.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create<bool>(async (all) =>
                            {
                                var folders = await DashboardService.GetSystemFolders(all);
                                response = new ItemListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = $"System Folders ({folders.Count})",
                                    Items = { folders }
                                };
                            })
                        };
                        sfCmd.AddOption(new Option<bool>(new string[] { "--all", "-a" }));
                        return sfCmd;

                    case ListSubCommand.Tools:
                        var toolCmd = new Command(listCmd.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create<bool>(async (all) =>
                            {
                                var tools = await ToolManager.GetTools();
                                if (!all)
                                {
                                    tools = tools.Where(t => t.IsAvailable && t.IsEnabled).ToList();
                                }
                                response = new ToolListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = $"Tools ({tools.Count})",
                                    Items = { tools }
                                };
                            })
                        };
                        toolCmd.AddOption(new Option<bool>(new string[] { "--all", "-a" }));
                        return toolCmd;

                    case ListSubCommand.AppLinkProviders:
                        var alpCmd = new Command(listCmd.ToString().ToLower())
                        {
                            Handler = CommandHandler.Create<bool>(async (all) =>
                            {
                                var providers = await AppLinkProviderManager.GetProviders();
                                if (!all)
                                {
                                    providers = providers.Where(t => t.IsAvailable && t.IsEnabled).ToList();
                                }
                                response = new AppLinkProviderListChatMessage(ChatMessageSource.Bot)
                                {
                                    Title = $"App Link Providers ({providers.Count})",
                                    Items = { providers }
                                };
                            })
                        };
                        alpCmd.AddOption(new Option<bool>(new string[] { "--all", "-a" }));
                        return alpCmd;

                    case ListSubCommand.Unknown:
                    default:
                        return null;
                }
            }
        }

        private static Uri GetUri(PathKind pathKind, Breadcrumb lastCrumb, string path)
        {
            switch (pathKind)
            {
                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Drive:
                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Drive:
                case PathKind.Network when lastCrumb?.AppPath == AppPath.Drive:
                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Drive:
                    return JumpPointService.GetAppUri(AppPath.Drive, path);

                case PathKind.Mounted when lastCrumb?.AppPath == AppPath.Folder:
                case PathKind.Unmounted when lastCrumb?.AppPath == AppPath.Folder:
                case PathKind.Network when lastCrumb?.AppPath == AppPath.Folder:
                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Folder:
                    return JumpPointService.GetAppUri(AppPath.Folder, path);

                case PathKind.Cloud when lastCrumb?.AppPath == AppPath.Cloud:
                    return Enum.TryParse<CloudStorageProvider>(lastCrumb?.DisplayName, true, out _) ?
                        JumpPointService.GetAppUri(AppPath.Cloud, path) :
                        JumpPointService.GetAppUri(AppPath.CloudDrives, null);

                case PathKind.Workspace when lastCrumb?.AppPath == AppPath.Workspace:
                    return JumpPointService.GetAppUri(AppPath.Workspace, path);

                case PathKind.Workspace:
                    return JumpPointService.GetAppUri(AppPath.Workspaces, null);

                case PathKind.AppLink:
                    return JumpPointService.GetAppUri(AppPath.AppLinks, null);

                case PathKind.Local:
                    return JumpPointService.GetAppUri(lastCrumb?.AppPath ?? AppPath.Dashboard, null);

                case PathKind.Unknown:
                default:
                    return JumpPointService.GetAppUri(AppPath.Dashboard, null);
            }
        }
    
    }

    public enum ChatbotCommand
    {
        Unknown = 0,
        Help = 1,

        Open = 10,
        List = 11,
    }

    public enum ListSubCommand
    {
        Unknown = 0,
        Favorites = 1,
        Workspaces = 2,
        Drives = 3,
        CloudDrives = 4,
        UserFolders = 5,
        SystemFolders = 6,
        Tools = 7,
        AppLinkProviders = 8
    }

    public static class ListExtensions
    {
        public static void Add<T>(this IList<T> list, IEnumerable<T> toAdd)
        {
            foreach (var item in toAdd)
            {
                list.Add(item);
            }
        }
    }
}

using JumpPoint.Platform.Items.CloudStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JumpPoint.Platform.Services
{

    public class CommandInfo
    {
        public CommandInfo(string commandFormat, string description, IEnumerable<string> samples = null)
        {
            CommandFormat = commandFormat;
            Description = description;
            Samples = new List<string>(samples ?? Enumerable.Repeat(CommandFormat, 1));
        }

        public string CommandFormat { get; }

        public string Description { get; }

        public IList<string> Samples { get; }

        public string DefaultCommand => Samples.FirstOrDefault();

        public static IList<CommandInfo> HelpCommands { get; } = new List<CommandInfo>
        {
            new CommandInfo("help", "Display all commands"),
            new CommandInfo("help open", "Display open commands"),
            new CommandInfo("help list", "Display list commands")
        };

        public static IList<CommandInfo> OpenCommands { get; } = new List<CommandInfo>
        {
            new CommandInfo("open <app-path>", "Open the specified Jump Point app path. App path is case-insensitive.",
                new List<string>
                {
                    "open Dashboard",
                    "open Settings",
                    "open Favorites",
                    "open Drives",
                    "open CloudDrives",
                    "open Workspaces",
                    "open AppLinks",
                    "open Chat",
                    "open ClipboardManager"
                }),
            new CommandInfo("open <item-path>", "Open the specified path. Item can be a Drive, Folder, Cloud Provider, or Workspace.",
                new List<string>
                {
                    $"open {Path.GetPathRoot(Environment.SystemDirectory)}",
                    $"open {Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}",
                    $"open {Prefix.CLOUD}{CloudStorageProvider.OneDrive}"
                })
        };

        public static IList<CommandInfo> ListCommands { get; } = new List<CommandInfo>
        {
            new CommandInfo("list favorites", "List all your favorite items"),
            new CommandInfo("list workspaces", "List all your workspaces"),
            new CommandInfo("list drives", "List all your drives"),
            new CommandInfo("list clouddrives", "List all your cloud drives"),
            new CommandInfo("list userfolders [--all|-a]", "List your enabled user folders or all of them",
                new List<string>
                {
                    "list userfolders",
                    "list userfolders --all"
                }),
            new CommandInfo("list systemfolders [--all|-a]", "List your enabled system folders or all of them",
                new List<string>
                {
                    "list systemfolders",
                    "list systemfolders --all"
                }),
            new CommandInfo("list tools [--all|-a]", "List your enabled tools or all of them",
                new List<string>
                {
                    "list tools",
                    "list tools --all"
                }),
            new CommandInfo("list applinkproviders [--all|-a]", "List your enabled app link providers or all of them",
                new List<string>
                {
                    "list applinkproviders",
                    "list applinkproviders --all"
                }),
        };

    }

}

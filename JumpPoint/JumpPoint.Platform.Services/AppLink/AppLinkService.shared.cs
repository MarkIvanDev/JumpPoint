using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using Newtonsoft.Json;
using NittyGritty.Extensions;
using NittyGritty.Models;
using NittyGritty.Platform.Launcher;
using NittyGritty.Platform.Payloads;
using NittyGritty.Utilities;
using SQLite;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services
{
    public static partial class AppLinkService
    {
        private const string APPLINK_DATAFILE = "applinks.db";
        private static readonly SQLiteAsyncConnection connection;

        static AppLinkService()
        {
            DataFilePath = Path.Combine(JumpPointService.DataFolder, APPLINK_DATAFILE);
            connection = new SQLiteAsyncConnection(DataFilePath);
        }

        public static string DataFilePath { get; }

        public static async Task Initialize()
        {
            await connection.RunInTransactionAsync(db =>
            {
                var tableSql = db.QueryScalars<string>($@"SELECT sql FROM sqlite_master WHERE type = ? AND name = ?",
                    "table", nameof(AppLinkInfo)).FirstOrDefault() ?? string.Empty;
                if (tableSql.IndexOf("\"Path\"", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    db.Execute("ALTER TABLE AppLinkInfo RENAME COLUMN Path to Link");
                    db.Execute("DROP INDEX IF EXISTS AppLinkInfo_Path");
                }

                if (tableSql.IndexOf("\"DisplayName\"", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    db.Execute("ALTER TABLE AppLinkInfo RENAME COLUMN DisplayName to Name");
                    db.Execute("DROP INDEX IF EXISTS AppLinkInfo_DisplayName");
                }

                if (tableSql.IndexOf("\"Identifier\"", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    db.Execute("ALTER TABLE AppLinkInfo RENAME COLUMN Identifier to AppId");
                }

                if (tableSql.IndexOf("\"InputKeys\"", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    db.Execute("ALTER TABLE AppLinkInfo RENAME COLUMN InputKeys to InputKeysJson");
                }
                db.CreateTable<AppLinkInfo>();
            });
        }

        public static ReadOnlyCollection<DataType> GetDataTypes()
        {
            var list = new List<DataType>();

            return new ReadOnlyCollection<DataType>(list);
        }

        public static async Task<ReadOnlyCollection<AppLink>> GetAppLinks()
        {
            var items = new List<AppLink>();
            var appLinks = await connection.Table<AppLinkInfo>().ToListAsync();
            foreach (var item in appLinks)
            {
                var appLink = GetAppLink(item);
                if (!(appLink is null))
                {
                    items.Add(appLink);
                }
            }
            return new ReadOnlyCollection<AppLink>(items);
        }

        public static async Task<AppLink> GetAppLink(string path)
        {
            var crumb = path.GetBreadcrumbs().LastOrDefault();
            if (crumb != null && !string.IsNullOrWhiteSpace(crumb.DisplayName))
            {
                var appLink = await connection.FindWithQueryAsync<AppLinkInfo>(
                    $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Name)} = ?",
                    crumb.DisplayName);
                return GetAppLink(appLink);
            }
            return null;
        }

        public static async Task<bool> LinkExists(string link)
        {
            var appLink = await connection.FindWithQueryAsync<AppLinkInfo>(
                $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Link)} = ?",
                link);
            return appLink != null;
        }

        public static async Task<AppLink> GetAppLink(IShareTargetPayload payload)
        {
            return await PlatformGetAppLink(payload);
        }

        public static Task<AppLinkLaunchTypes> GetLaunchTypes(Uri uri, string identifier)
            => PlatformGetLaunchTypes(uri, identifier);

        public static AppLink GetAppLink(AppLinkInfo appLink)
        {
            if (appLink != null)
            {
                return new AppLink(
                    appLink.Name,
                    appLink.Description,
                    appLink.Link,
                    appLink.AppName,
                    appLink.AppId,
                    appLink.Logo.ToMemoryStream(),
                    appLink.Background.ToColor(),
                    appLink.QueryKeys.ToQuery(),
                    appLink.InputKeys,
                    appLink.LaunchTypes);
            }
            return null;
        }

        private static string GetAvailableName(SQLiteConnection db, string desiredName)
        {
            var namePart = desiredName.Trim();
            var name = namePart;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Name)} = ?", name) > 0)
            {
                name = $"{namePart} ({number})";
                number += 1;
            }
            return name;
        }

        public static async Task<AppLink> Create(AppLinkInfo appLink)
        {
            AppLink link = null;
            await connection.RunInTransactionAsync(db =>
            {
                var pathExists = db.FindWithQuery<AppLinkInfo>(
                    $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Link)} = ?",
                    appLink.Link) != null;
                if (!pathExists)
                {
                    appLink.Name = GetAvailableName(db, appLink.Name);
                    db.Insert(appLink);
                    link = GetAppLink(appLink);
                }
            });
            return link;
        }

        public static async Task<string> Rename(AppLink appLink, string name, RenameOption option)
        {
            var newName = string.Empty;
            if (!(appLink is null))
            {
                await connection.RunInTransactionAsync(db =>
                {
                    var item = db.FindWithQuery<AppLinkInfo>($"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Link)} = ?", appLink.Path);
                    if (!(item is null))
                    {
                        newName = GetAvailableName(db, item.Name);
                        item.Name = newName;
                        db.Update(item);
                    }
                });
            }
            return newName;
        }

        public static async Task Delete(AppLink appLink)
        {
            await connection.ExecuteAsync($"DELETE FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Link)} = ?", appLink.Link);
        }

        public static async Task Delete(IList<AppLink> appLinks)
        {
            await Delete(appLinks.Select(i => i.Link).ToList());
        }

        public static async Task Delete(IList<string> appLinks)
        {
            await connection.RunInTransactionAsync(db =>
            {
                foreach (var item in appLinks)
                {
                    db.Execute($"DELETE FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Link)} = ?", item);
                }
            });
        }

        public static async Task OpenUri(AppLink appLink)
        {
            await PlatformOpenUri(appLink);
        }

        public static async Task<Collection<ValueInfo>> OpenUriForResults(AppLink appLink)
        {
            return await PlatformOpenUriForResults(appLink);
        }

        public static async Task Load(AppLink appLink)
        {
            appLink.IsFavorite = await DashboardService.GetStatus(appLink);
        }

        public static Task<IList<IAppInfo>> FindAppHandlers(string link)
            => PlatformFindAppHandlers(link);

    }

}

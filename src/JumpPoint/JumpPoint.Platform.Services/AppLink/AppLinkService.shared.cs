using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using Newtonsoft.Json;
using NittyGritty.Models;
using NittyGritty.Platform.Launcher;
using NittyGritty.Platform.Payloads;
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
            connection = new SQLiteAsyncConnection(Path.Combine(JumpPointService.DataFolder, APPLINK_DATAFILE));
        }

        public static async Task Initialize()
        {
            await connection.RunInTransactionAsync(db =>
            {
                var oldTableExists = db.ExecuteScalar<int>("SELECT COUNT(*) FROM sqlite_master WHERE type = ? AND name = ?", "table", "AppLink") > 0;
                if (oldTableExists)
                {
                    db.Execute("ALTER TABLE AppLink RENAME TO ?", nameof(AppLinkInfo));

                    var duplicateNames = db.Query<Item<string>>(
                        $"SELECT {nameof(AppLinkInfo.DisplayName)} AS Data FROM {nameof(AppLinkInfo)} " +
                        $"GROUP BY {nameof(AppLinkInfo.DisplayName)} HAVING COUNT(*) > 0");
                    foreach (var item in duplicateNames)
                    {
                        var duplicates = db.Query<AppLinkInfo>($"SELECT * FROM {nameof(AppLinkInfo)} " +
                            $"WHERE {nameof(AppLinkInfo.DisplayName)} = ?", item);
                        for (int i = 0; i < duplicates.Count; i++)
                        {
                            duplicates[i].DisplayName += $" ({i + 1})";
                            db.Update(duplicates[i]);
                        }
                    }
                }
            });
            await connection.CreateTableAsync<AppLinkInfo>();
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
            var appLink = await connection.FindWithQueryAsync<AppLinkInfo>(
                $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?",
                path);
            return GetAppLink(appLink);
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
                var logo = appLink.Logo != null ? new MemoryStream() : null;
                logo?.Write(appLink.Logo, 0, appLink.Logo.Length);
                return new AppLink(
                    appLink.DisplayName,
                    appLink.Path,
                    appLink.AppName,
                    appLink.Identifier,
                    logo,
                    ColorConverters.FromHex(appLink.Background),
                    JsonConvert.DeserializeObject<Collection<ValueInfo>>(appLink.InputKeys),
                    appLink.LaunchTypes);
            }
            return null;
        }

        public static async Task<bool> AppLinkExists(string path)
        {
            return await connection.FindWithQueryAsync<AppLinkInfo>(
                $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?",
                path) != null;
        }

        private static string GetAvailableName(SQLiteConnection db, string desiredName)
        {
            var namePart = desiredName.Trim();
            var name = namePart;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.DisplayName)} = ?", name) > 0)
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
                    $"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?",
                    appLink.Path) != null;
                if (!pathExists)
                {
                    appLink.DisplayName = GetAvailableName(db, appLink.DisplayName);
                    db.Insert(appLink);
                    link = GetAppLink(appLink);
                }
            });
            return link;
        }

        public static async Task<string> Rename(AppLink appLink, string name, RenameCollisionOption option)
        {
            var newName = string.Empty;
            if (!(appLink is null))
            {
                await connection.RunInTransactionAsync(db =>
                {
                    var item = db.FindWithQuery<AppLinkInfo>($"SELECT * FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?", appLink.Path);
                    if (!(item is null))
                    {
                        newName = GetAvailableName(db, item.DisplayName);
                        item.DisplayName = newName;
                        db.Update(item);
                    }
                });
            }
            return newName;
        }

        public static async Task Delete(AppLink appLink)
        {
            await connection.ExecuteAsync($"DELETE FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?", appLink.Path);
        }

        public static async Task Delete(IList<AppLink> appLinks)
        {
            await Delete(appLinks.Select(i => i.Path).ToList());
        }

        public static async Task Delete(IList<string> appLinks)
        {
            await connection.RunInTransactionAsync(db =>
            {
                foreach (var item in appLinks)
                {
                    db.Execute($"DELETE FROM {nameof(AppLinkInfo)} WHERE {nameof(AppLinkInfo.Path)} = ?", item);
                }
            });
        }

        public static async Task OpenUri(AppLink appLink, IList<ValueInfo> inputValues)
        {
            await PlatformOpenUri(appLink, inputValues);
        }

        public static async Task<Collection<ValueInfo>> OpenUriForResults(AppLink appLink, IList<ValueInfo> inputValues)
        {
            return await PlatformOpenUriForResults(appLink, inputValues);
        }

        public static async Task Load(AppLink appLink)
        {
            appLink.IsFavorite = await DashboardService.GetStatus(appLink);
        }

        public static Task<ReadOnlyCollection<IAppInfo>> FindAppHandlers(string path)
            => PlatformFindAppHandlers(path);

    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using NittyGritty.Extensions;
using NittyGritty.Utilities;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services.OneDrive
{
    public static class OneDriveServiceExtensions
    {
        private const string PATH_PREFIX = @"/drive/root:";

        private static readonly string clientId;
        private static readonly string authority;
        private static readonly string redirectUri;
        private static readonly string graph;
        private static readonly string[] scopes;

        static OneDriveServiceExtensions()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("onedrive.jps.json", optional: true)
                .Build();
            clientId = config[nameof(clientId)];
            authority = config[nameof(authority)];
            redirectUri = config[nameof(redirectUri)];
            graph = config[nameof(graph)];
            scopes = config
                .GetSection(nameof(scopes))?
                .GetChildren()?
                .Select(c => c.Value)?
                .ToArray();
        }

        public static IPublicClientApplication CreateClientApp()
        {
            return PublicClientApplicationBuilder
                .Create(clientId)
                .WithBroker(true)
                .Build();
        }

        public static async Task<IAccount> GetNewAccount(this IPublicClientApplication clientApp)
        {
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var authResult = await clientApp
                        .AcquireTokenInteractive(scopes)
                        .ExecuteAsync();
                    return authResult.Account;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }

        public static GraphServiceClient GetGraphServiceClient(this IPublicClientApplication clientApp, IAccount account)
        {
            return new GraphServiceClient(graph,
                new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", await clientApp.GetAccessToken(account));
                }));
        }

        public static async Task<string> GetAccessToken(this IPublicClientApplication clientApp, IAccount account)
        {
            AuthenticationResult authResult;
            try
            {
                authResult = await clientApp
                    .AcquireTokenSilent(scopes, account)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            catch (MsalUiRequiredException)
            {
                authResult = await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token
                    if (account is null)
                    {
                        return await clientApp
                            .AcquireTokenInteractive(scopes)
                            .ExecuteAsync();
                    }
                    else
                    {
                        return await clientApp
                            .AcquireTokenInteractive(scopes)
                            .WithAccount(account)
                            .ExecuteAsync();
                    }
                });
            }
            return authResult?.AccessToken;
        }

        public static async Task<string> GetDisplayName(this GraphServiceClient graphClient)
        {
            try
            {
                var user = await graphClient.Me.Request().GetAsync().ConfigureAwait(false);
                return user.DisplayName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Drive> GetDrive(this GraphServiceClient graphClient)
        {
            try
            {
                var drive = await graphClient.Me.Drive.Request().GetAsync().ConfigureAwait(false);
                drive.Root = await graphClient.Me.Drive.Root.Request().GetAsync().ConfigureAwait(false);
                return drive;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    
        public static async Task<IList<DriveItem>> GetDriveItems(this GraphServiceClient graphClient)
        {
            var items = new List<DriveItem>();
            try
            {
                var pagedItems = await graphClient.Me.Drive.Root.Children.Request().Top(999).GetAsync().ConfigureAwait(false);
                var pageIterator = PageIterator<DriveItem>
                    .CreatePageIterator(graphClient, pagedItems, i =>
                    {
                        items.Add(i);
                        return true;
                    });
                await pageIterator.IterateAsync();
                return items;
            }
            catch (Exception)
            {
                return items;
            }
        }

        public static async Task<IList<DriveItem>> GetDriveItems(this GraphServiceClient graphClient, DriveItem driveItem)
        {
            var items = new List<DriveItem>();
            try
            {
                if (driveItem.Folder is null) return items;

                var pagedItems = await graphClient.Me.Drive.Items[driveItem.Id].Children.Request().Top(999).GetAsync().ConfigureAwait(false);
                var pageIterator = PageIterator<DriveItem>
                    .CreatePageIterator(graphClient, pagedItems, i =>
                    {
                        items.Add(i);
                        return true;
                    });
                await pageIterator.IterateAsync();
                return items;
            }
            catch (Exception)
            {
                return items;
            }
        }

        public static async Task<IList<DriveItem>> GetDriveItems(this GraphServiceClient graphClient, string path)
        {
            var items = new List<DriveItem>();
            try
            {
                var pagedItems = await graphClient.Me.Drive.Root.ItemWithPath(path).Children.Request().Top(999).GetAsync().ConfigureAwait(false);
                var pageIterator = PageIterator<DriveItem>
                    .CreatePageIterator(graphClient, pagedItems, i =>
                    {
                        items.Add(i);
                        return true;
                    });
                await pageIterator.IterateAsync();
                return items;
            }
            catch (Exception)
            {
                return items;
            }
        }

        public static async Task<DriveItem> GetDriveItem(this GraphServiceClient graphClient, string path)
        {
            try
            {
                var item = await graphClient.Me.Drive.Root.ItemWithPath(path).Request().GetAsync().ConfigureAwait(false);
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<Stream> GetContent(this GraphServiceClient graphClient, string id)
        {
            try
            {
                var content = await graphClient.Me.Drive.Items[id].Content.Request().GetAsync().ConfigureAwait(false);
                return content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DriveItem> CreateFolder(this GraphServiceClient graphClient, string id, string name, CreateOption option)
        {
            try
            {
                var conflictBehavior = "";
                switch (option)
                {
                    case CreateOption.OpenIfExists:
                        var currentFolder = await CodeHelper.InvokeOrDefault(async () => await graphClient.Me.Drive.Items[id].ItemWithPath(name).Request().GetAsync().ConfigureAwait(false));
                        if (currentFolder != null && currentFolder.Folder != null)
                        {
                            return currentFolder;
                        }
                        else
                        {
                            conflictBehavior = "fail";
                        }
                        break;

                    case CreateOption.ReplaceExisting:
                        conflictBehavior = "replace";
                        break;

                    case CreateOption.DoNothing:
                        conflictBehavior = "fail";
                        break;

                    case CreateOption.GenerateUniqueName:
                    default:
                        conflictBehavior = "rename";
                        break;
                }

                var driveItem = new DriveItem
                {
                    Name = name,
                    Folder = new Folder
                    {
                    },
                    AdditionalData = new Dictionary<string, object>()
                    {
                        { "@microsoft.graph.conflictBehavior", conflictBehavior }
                    }
                };

                var newFolder = await graphClient.Me.Drive.Items[id].Children.Request().AddAsync(driveItem).ConfigureAwait(false);
                return newFolder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<DriveItem> CreateFile(this GraphServiceClient graphClient, string id, string name, CreateOption option, byte[] content)
        {
            try
            {
                var stream = content != null ? new MemoryStream(content) : null;
                var currentFile = await CodeHelper.InvokeOrDefault(async () => await graphClient.Me.Drive.Items[id].ItemWithPath(name).Request().GetAsync().ConfigureAwait(false));
                if (currentFile != null)
                {
                    switch (option)
                    {
                        case CreateOption.ReplaceExisting:
                            return await graphClient.Me.Drive.Items[id].ItemWithPath(name).Content.Request().PutAsync<DriveItem>(stream).ConfigureAwait(false);

                        case CreateOption.DoNothing:
                            return null;

                        case CreateOption.OpenIfExists:
                            return currentFile;

                        case CreateOption.GenerateUniqueName:
                        default:
                            var newName = await GetAvailableName(graphClient, id, name);
                            return await graphClient.Me.Drive.Items[id].ItemWithPath(newName).Content.Request().PutAsync<DriveItem>(stream).ConfigureAwait(false);
                    }
                }
                else
                {
                    var newFile = await graphClient.Me.Drive.Items[id].ItemWithPath(name).Content.Request().PutAsync<DriveItem>(stream).ConfigureAwait(false);
                    return newFile;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

        }


        static async Task<string> GetAvailableName(GraphServiceClient graphClient, string id, string desiredName)
        {
            var namePart = Path.GetFileNameWithoutExtension(desiredName.Trim());
            var ext = Path.GetExtension(desiredName);
            var name = desiredName;
            var number = 2;
            while (await CodeHelper.InvokeOrDefault(async () => await graphClient.Me.Drive.Items[id].ItemWithPath(name).Request().GetAsync().ConfigureAwait(false)) != null)
            {
                name = $"{namePart} ({number}){ext}";
                number += 1;
            }
            return name;
        }

        public static async Task<string> Rename(this GraphServiceClient client, string id, string name, RenameOption option)
        {
            try
            {
                var conflictBehavior = "";
                switch (option)
                {
                    case RenameOption.ReplaceExisting:
                        conflictBehavior = "replace";
                        break;
                    case RenameOption.DoNothing:
                        conflictBehavior = "fail";
                        break;
                    case RenameOption.GenerateUniqueName:
                    default:
                        conflictBehavior = "rename";
                        break;
                }
                var driveItem = new DriveItem
                {
                    Name = name,
                    AdditionalData = new Dictionary<string, object>()
                    {
                        { "@microsoft.graph.conflictBehavior", conflictBehavior }
                    }
                };
                var newInfo = await client.Me.Drive.Items[id].Request().UpdateAsync(driveItem).ConfigureAwait(false);
                Debug.Assert(id == newInfo.Id);
                return newInfo.Name;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task Delete(this GraphServiceClient client, string id)
        {
            try
            {
                await client.Me.Drive.Items[id].Request().DeleteAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }

        public static string GetPath(this DriveItem driveItem)
        {
            if (driveItem.ParentReference != null)
            {
                var path = driveItem.ParentReference.Path.StartsWith(PATH_PREFIX) ?
                    driveItem.ParentReference.Path.Remove(0, PATH_PREFIX.Length) :
                    driveItem.ParentReference.Path;
                return $"{path.WithEnding(@"/")}{driveItem.Name}"
                    .TrimStart('/')
                    .Replace(@"/", @"\");
            }
            return driveItem.Name;
        }

        public static StorageItemBase Convert(this DriveItem driveItem, OneDriveAccount account)
        {
            var path = $@"cloud:\OneDrive\{account.Name}\{driveItem.GetPath()}";
            if (driveItem.Folder != null)
            {
                return new OneDriveFolder(account, driveItem, path);
            }
            else if (driveItem.File != null)
            {
                return new OneDriveFile(account, driveItem, path);
            }
            else
            {
                return null;
            }
        }

    }
}

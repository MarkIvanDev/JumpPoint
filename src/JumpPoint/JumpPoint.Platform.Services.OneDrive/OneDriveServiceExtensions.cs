using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.Storage;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using NittyGritty.Extensions;

namespace JumpPoint.Platform.Services.OneDrive
{
    public static class OneDriveServiceExtensions
    {
        private const string clientId = "";
        private const string authority = "https://login.microsoftonline.com/consumers";
        private const string redirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        private const string graph = "https://graph.microsoft.com/v1.0/";
        private const string pathPrefix = @"/drive/root:";
        private static readonly IReadOnlyCollection<string> scopes = new List<string> { "User.Read", "Files.ReadWrite.All" }.AsReadOnly();

        public static IPublicClientApplication CreateClientApp()
        {
            return PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithRedirectUri(redirectUri)
                .Build();
        }

        public static async Task<IAccount> GetNewAccount(this IPublicClientApplication clientApp)
        {
            try
            {
                var authResult = await clientApp
                    .AcquireTokenInteractive(scopes)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
                return authResult.Account;
            }
            catch (Exception)
            {
                return null;
            }
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
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                if (account is null)
                {
                    authResult = await clientApp
                        .AcquireTokenInteractive(scopes)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    authResult = await clientApp
                        .AcquireTokenInteractive(scopes)
                        .WithAccount(account)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
            }
            return authResult.AccessToken;
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
                return drive;
            }
            catch (Exception)
            {
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

        public static string GetPath(this DriveItem driveItem)
        {
            if (driveItem.ParentReference != null)
            {
                var path = driveItem.ParentReference.Path.StartsWith(pathPrefix) ?
                    driveItem.ParentReference.Path.Remove(0, pathPrefix.Length) :
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

using System;
using GalaSoft.MvvmLight.Ioc;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Data;
using NittyGritty.Platform.Store;
using NittyGritty.Services.Core;
using NittyGritty.Uwp.Services;

namespace JumpPoint.Uwp.Helpers
{
    public class ServiceLocator : ViewModelLocator
    {
        static ServiceLocator()
        {
            SimpleIoc.Default.Register<ViewModelLocator>(() => App.Current.Resources["Locator"] as ServiceLocator);
        }

        public ServiceLocator()
        {
            // Register KnownTypes for CacheManager
            CacheManager.KnownTypes.Add(typeof(StorageType));

            // Register DialogService
            SimpleIoc.Default.TryRegister<IDialogService>(() =>
            {
                var dialogService = new DialogService();
                dialogService.Configure(DialogKeys.RequestPermission, typeof(Dialogs.RequestPermissionDialog));
                dialogService.Configure(DialogKeys.DeleteConfirmation, typeof(Dialogs.DeleteConfirmationDialog));
                dialogService.Configure(DialogKeys.NewFile, typeof(Dialogs.NewFileDialog));
                dialogService.Configure(DialogKeys.NewFolder, typeof(Dialogs.NewFolderDialog));
                dialogService.Configure(DialogKeys.NewWorkspace, typeof(Dialogs.AddWorkspaceDialog));
                dialogService.Configure(DialogKeys.NewItemPicker, typeof(Dialogs.NewItemPicker));
                //dialogService.Configure(DialogKeys.NewAppLink, typeof(Dialogs.NewAppLinkDialog));
                dialogService.Configure(DialogKeys.Rename, typeof(Dialogs.RenameDialog));
                dialogService.Configure(DialogKeys.AddToWorkspace, typeof(Dialogs.AddToWorkspaceDialog));
                dialogService.Configure(DialogKeys.AppLinkLaunch, typeof(Dialogs.AppLinkLaunchDialog));
                dialogService.Configure(DialogKeys.AppLinkLaunchResults, typeof(Dialogs.AppLinkLaunchResultsDialog));
                dialogService.Configure(DialogKeys.WorkspaceTemplatePicker, typeof(Dialogs.WorkspaceTemplatePicker));
                dialogService.Configure(DialogKeys.FolderTemplatePicker, typeof(Dialogs.FolderTemplatePicker));
                dialogService.Configure(DialogKeys.RenameCloudAccount, typeof(Dialogs.RenameCloudAccountDialog));
                dialogService.Configure(DialogKeys.AppLinkProviderPicker, typeof(Dialogs.AppLinkProviderPicker));
                dialogService.Configure(DialogKeys.ToolPicker, typeof(Dialogs.ToolPicker));
                dialogService.Configure(DialogKeys.Copy, typeof(Dialogs.Clipboard.CopyDialog));
                dialogService.Configure(DialogKeys.Move, typeof(Dialogs.Clipboard.MoveDialog));
                return dialogService;
            });

            // Register AddOnService
            SimpleIoc.Default.TryRegister<IAddOnService>(() =>
            {
                var addOnService = new AddOnService();
#if JPBETA
                addOnService.Configure(AddOnKeys.Monthly1, new SubscriptionAddOn("9P2WC0V906ZL") { Price = "$0.99" });
                addOnService.Configure(AddOnKeys.Monthly2, new SubscriptionAddOn("9PLBZKNFTCL0") { Price = "$2.99" });
                addOnService.Configure(AddOnKeys.Monthly3, new SubscriptionAddOn("9PJ1PJ9SHQ2G") { Price = "$4.99" });
                addOnService.Configure(AddOnKeys.Monthly4, new SubscriptionAddOn("9P17MRR2QMM1") { Price = "$9.99" });
                addOnService.Configure(AddOnKeys.Monthly5, new SubscriptionAddOn("9NS7RZG3GS55") { Price = "$19.99" });

                addOnService.Configure(AddOnKeys.Durable1, new DurableAddOn("9MX7P8CB4QF0") { Price = "$0.99" });
                addOnService.Configure(AddOnKeys.Durable2, new DurableAddOn("9N70G0L272B5") { Price = "$2.99" });
                addOnService.Configure(AddOnKeys.Durable3, new DurableAddOn("9N0JKP3QV4GT") { Price = "$4.99" });
                addOnService.Configure(AddOnKeys.Durable4, new DurableAddOn("9PH8KSXPB3GR") { Price = "$9.99" });
                addOnService.Configure(AddOnKeys.Durable5, new DurableAddOn("9NXW4694NSK5") { Price = "$19.99" });
#else
                addOnService.Configure(AddOnKeys.Monthly1, new SubscriptionAddOn("9P8M7BX63QVX") { Price = "$0.99" });
                addOnService.Configure(AddOnKeys.Monthly2, new SubscriptionAddOn("9NC3R3HRVT85") { Price = "$2.99" });
                addOnService.Configure(AddOnKeys.Monthly3, new SubscriptionAddOn("9N5C9LN7LLCF") { Price = "$4.99" });
                addOnService.Configure(AddOnKeys.Monthly4, new SubscriptionAddOn("9PMBNNLX4P2Q") { Price = "$9.99" });
                addOnService.Configure(AddOnKeys.Monthly5, new SubscriptionAddOn("9PDZTR16PXLQ") { Price = "$19.99" });

                addOnService.Configure(AddOnKeys.Durable1, new DurableAddOn("9N1KH0N44V71") { Price = "$0.99" });
                addOnService.Configure(AddOnKeys.Durable2, new DurableAddOn("9PDPL8HVBRQP") { Price = "$2.99" });
                addOnService.Configure(AddOnKeys.Durable3, new DurableAddOn("9N2Q823F2H2T") { Price = "$4.99" });
                addOnService.Configure(AddOnKeys.Durable4, new DurableAddOn("9PPCL2KF3Z66") { Price = "$9.99" });
                addOnService.Configure(AddOnKeys.Durable5, new DurableAddOn("9MXW1MMM4GH8") { Price = "$19.99" });
#endif
                return addOnService;
            });

            // Register Icon Resource
            //var iconResources = new ResourceService<Uri>();

            // Register Helpers
            SimpleIoc.Default.TryRegister<ITileIconHelper, TileIconHelper>();

            // Register NittyGritty Services
            SimpleIoc.Default.TryRegister<IClipboardService, NittyGritty.Uwp.Services.ClipboardService>();
            SimpleIoc.Default.TryRegister<IShareService, ShareService>();
            SimpleIoc.Default.TryRegister<IThemeService, ThemeService>();
            SimpleIoc.Default.TryRegister<IShortcutService, ShortcutService>();
            SimpleIoc.Default.TryRegister<IContactService, ContactService>();
            SimpleIoc.Default.TryRegister<IFileService, FileService>();
        }

        public override INavigationService GetNavigationService(string key)
        {
            if (!SimpleIoc.Default.IsRegistered<INavigationService>(key))
            {
                // Register NavigationService
                var navigationService = new NavigationService();
                navigationService.Configure(ViewModelKeys.Dashboard, typeof(Views.DashboardPage));
                navigationService.Configure(ViewModelKeys.Favorites, typeof(Views.FavoritesPage));
                navigationService.Configure(ViewModelKeys.Workspaces, typeof(Views.WorkspacesPage));
                navigationService.Configure(ViewModelKeys.Workspace, typeof(Views.WorkspacePage));
                navigationService.Configure(ViewModelKeys.Drives, typeof(Views.DrivesPage));
                navigationService.Configure(ViewModelKeys.Drive, typeof(Views.DrivePage));
                navigationService.Configure(ViewModelKeys.Folder, typeof(Views.FolderPage));
                navigationService.Configure(ViewModelKeys.CloudDrives, typeof(Views.CloudDrivesPage));
                navigationService.Configure(ViewModelKeys.Cloud, typeof(Views.CloudPage));
                navigationService.Configure(ViewModelKeys.AppLinks, typeof(Views.AppLinksPage));
                navigationService.Configure(ViewModelKeys.Libraries, typeof(Views.LibrariesPage));
                navigationService.Configure(ViewModelKeys.Library, typeof(Views.LibraryPage));
                navigationService.Configure(ViewModelKeys.Settings, typeof(Views.SettingsPage));
                SimpleIoc.Default.Register<INavigationService>(() => navigationService, key);
            }
            return SimpleIoc.Default.GetInstance<INavigationService>(key);
        }

        public static AppSettings AppSettings => SimpleIoc.Default.GetInstance<AppSettings>();
    }

    public static class SimpleIocExtensions
    {

        public static void TryRegister<TService>(this SimpleIoc ioc, Func<TService> factory) where TService : class
        {
            if (!ioc.IsRegistered<TService>())
            {
                ioc.Register<TService>(factory);
            }
        }

        public static void TryRegister<TService, TImplementation>(this SimpleIoc ioc)
            where TService : class
            where TImplementation : class, TService
        {
            if (!ioc.IsRegistered<TService>())
            {
                ioc.Register<TService, TImplementation>();
            }
        }

    }

}

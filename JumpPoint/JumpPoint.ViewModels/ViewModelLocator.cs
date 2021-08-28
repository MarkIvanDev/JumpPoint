using GalaSoft.MvvmLight.Ioc;
using JumpPoint.Platform;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Dialogs;
using JumpPoint.ViewModels.Helpers;
using JumpPoint.ViewModels.Hosted;
using JumpPoint.ViewModels.Standalone;
using NittyGritty.Services;
using NittyGritty.ViewModels;
using System;

namespace JumpPoint.ViewModels
{
    public abstract class ViewModelLocator
    {
        
        public ViewModelLocator()
        {
            // Register other data stores
            SimpleIoc.Default.Register<ShellItems>();

            // Register Helpers
            SimpleIoc.Default.Register<CommandHelper>();

            // Register Tabbed Shell
            SimpleIoc.Default.Register<TabbedShellViewModel>();

            // Register ViewModels
            SimpleIoc.Default.Register<ShellViewModel>();
            SimpleIoc.Default.Register<DashboardViewModel>();
            SimpleIoc.Default.Register<FavoritesViewModel>();
            SimpleIoc.Default.Register<WorkspacesViewModel>();
            SimpleIoc.Default.Register<WorkspaceViewModel>();
            SimpleIoc.Default.Register<DrivesViewModel>();
            SimpleIoc.Default.Register<DriveViewModel>();
            SimpleIoc.Default.Register<FolderViewModel>();
            SimpleIoc.Default.Register<CloudDrivesViewModel>();
            SimpleIoc.Default.Register<CloudViewModel>();
            SimpleIoc.Default.Register<AppLinksViewModel>();
            SimpleIoc.Default.Register<LibrariesViewModel>();
            SimpleIoc.Default.Register<LibraryViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();

            // Register Standalone ViewModels
            SimpleIoc.Default.Register<PropertiesViewModel>();
            SimpleIoc.Default.Register<ClipboardManagerViewModel>();

            // Register Hosted ViewModels
            SimpleIoc.Default.Register<ShareAppLinkViewModel>();
            SimpleIoc.Default.Register<ManualAppLinkPickerViewModel>();
            SimpleIoc.Default.Register<AppLinkProviderViewModel>();
            SimpleIoc.Default.Register<HashToolViewModel>();
            SimpleIoc.Default.Register<ChatbotViewModel>();
        }

        public static ViewModelLocator Instance => SimpleIoc.Default.GetInstance<ViewModelLocator>();

        // Since Shell Windows are opened in a separate instance (see Multi-Process), we can be assured that only 1 ShellViewModel exists in each Process
        public ShellViewModel Shell => SimpleIoc.Default.GetInstance<ShellViewModel>();

        public TabbedShellViewModel TabbedShell => SimpleIoc.Default.GetInstance<TabbedShellViewModel>();

        // Since Property Windows are opened in a separate instance (see Multi-Process), we can be assured that only 1 PropertiesViewModel exists in each Process
        public PropertiesViewModel Properties => SimpleIoc.Default.GetInstance<PropertiesViewModel>();

        public AppSettings AppSettings => AppSettings.Instance;

        public CommandHelper CommandHelper => SimpleIoc.Default.GetInstance<CommandHelper>();

        public T GetUniqueInstance<T>() where T : class
        {
            var key = Guid.NewGuid().ToString();
            var instance = SimpleIoc.Default.GetInstance<T>(key);
            SimpleIoc.Default.Unregister<T>(key);
            return instance;
        }

        public TabViewModel GetNewTab()
        {
            var key = Guid.NewGuid().ToString();
            var navService = GetNavigationService(key);
            SimpleIoc.Default.Register(() => new TabViewModel(key, navService), key);
            var instance = SimpleIoc.Default.GetInstance<TabViewModel>(key);
            return instance;
        }

        public TabViewModel GetTab(string key)
        {
            var navService = GetNavigationService(key);
            if (!SimpleIoc.Default.IsRegistered<TabViewModel>(key))
            {
                SimpleIoc.Default.Register(() => new TabViewModel(key, navService), key);
            }
            var instance = SimpleIoc.Default.GetInstance<TabViewModel>(key);
            return instance;
        }

        public void DisposeTab(string key)
        {
            SimpleIoc.Default.Unregister<INavigationService>(key);
            SimpleIoc.Default.Unregister<TabViewModel>(key);
            SimpleIoc.Default.Unregister<DashboardViewModel>(key);
            SimpleIoc.Default.Unregister<SettingsViewModel>(key);
            SimpleIoc.Default.Unregister<FavoritesViewModel>(key);
            SimpleIoc.Default.Unregister<DrivesViewModel>(key);
            SimpleIoc.Default.Unregister<CloudDrivesViewModel>(key);
            SimpleIoc.Default.Unregister<WorkspacesViewModel>(key);
            SimpleIoc.Default.Unregister<AppLinksViewModel>(key);
            SimpleIoc.Default.Unregister<DriveViewModel>(key);
            SimpleIoc.Default.Unregister<FolderViewModel>(key);
            SimpleIoc.Default.Unregister<WorkspaceViewModel>(key);
            SimpleIoc.Default.Unregister<CloudViewModel>(key);
            SimpleIoc.Default.Unregister<PropertiesViewModel>(key);
            SimpleIoc.Default.Unregister<ChatbotViewModel>(key);
        }

        public ShellContextViewModelBase GetContext(AppPath appPath, string key)
        {
            switch (appPath)
            {
                case AppPath.Dashboard:
                    return SimpleIoc.Default.GetInstance<DashboardViewModel>(key);

                case AppPath.Settings:
                    return SimpleIoc.Default.GetInstance<SettingsViewModel>(key);

                case AppPath.Favorites:
                    return SimpleIoc.Default.GetInstance<FavoritesViewModel>(key);

                case AppPath.Drives:
                    return SimpleIoc.Default.GetInstance<DrivesViewModel>(key);

                case AppPath.CloudDrives:
                    return SimpleIoc.Default.GetInstance<CloudDrivesViewModel>(key);

                case AppPath.Workspaces:
                    return SimpleIoc.Default.GetInstance<WorkspacesViewModel>(key);

                case AppPath.AppLinks:
                    return SimpleIoc.Default.GetInstance<AppLinksViewModel>(key);

                case AppPath.Drive:
                    return SimpleIoc.Default.GetInstance<DriveViewModel>(key);

                case AppPath.Folder:
                    return SimpleIoc.Default.GetInstance<FolderViewModel>(key);

                case AppPath.Workspace:
                    return SimpleIoc.Default.GetInstance<WorkspaceViewModel>(key);

                case AppPath.Cloud:
                    return SimpleIoc.Default.GetInstance<CloudViewModel>(key);

                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                case AppPath.Unknown:
                default:
                    return null;
            }
        }

        public TabbedNavigationHelper GetNavigationHelper(string key)
        {
            var navService = GetNavigationService(key);
            return new TabbedNavigationHelper(key, navService);
        }

        public abstract INavigationService GetNavigationService(string key);
    
    }

    public static class ViewModelKeys
    {
        public static string Dashboard => nameof(Dashboard);
        public static string Favorites => nameof(Favorites);
        public static string Workspaces => nameof(Workspaces);
        public static string Workspace => nameof(Workspace);
        public static string Drives => nameof(Drives);
        public static string Drive => nameof(Drive);
        public static string Folder => nameof(Folder);
        public static string CloudDrives => nameof(CloudDrives);
        public static string Cloud => nameof(Cloud);
        public static string AppLinks => nameof(AppLinks);
        public static string Libraries => nameof(Libraries);
        public static string Library => nameof(Library);
        public static string Settings => nameof(Settings);

        public static string Properties => nameof(Properties);
        public static string Copy => nameof(Copy);
        public static string Move => nameof(Move);
        public static string Delete => nameof(Delete);
        public static string Chatbot => nameof(Chatbot);
        public static string ClipboardManager => nameof(ClipboardManager);

    }

    public static class DialogKeys
    {
        public static string RequestPermission => nameof(RequestPermission);
        public static string DeleteConfirmation => nameof(DeleteConfirmation);
        public static string NewFolder => nameof(NewFolder);
        public static string NewWorkspace => nameof(NewWorkspace);
        //public static string NewAppLink => nameof(NewAppLink);
        public static string Rename => nameof(Rename);
        public static string AddToWorkspace => nameof(AddToWorkspace);
        public static string AppLinkLaunch => nameof(AppLinkLaunch);
        public static string AppLinkLaunchResults => nameof(AppLinkLaunchResults);
        public static string WorkspaceTemplatePicker => nameof(WorkspaceTemplatePicker);
        public static string FolderTemplatePicker => nameof(FolderTemplatePicker);
        public static string RenameCloudAccount => nameof(RenameCloudAccount);
        public static string AppLinkProviderPicker => nameof(AppLinkProviderPicker);
        public static string ToolPicker => nameof(ToolPicker);

        public static string Copy => nameof(Copy);
        public static string Move => nameof(Move);
    }

    public static class AddOnKeys
    {
        public static string Monthly1 => nameof(Monthly1);
        public static string Monthly2 => nameof(Monthly2);
        public static string Monthly3 => nameof(Monthly3);
        public static string Monthly4 => nameof(Monthly4);
        public static string Monthly5 => nameof(Monthly5);

        public static string Durable1 => nameof(Durable1);
        public static string Durable2 => nameof(Durable2);
        public static string Durable3 => nameof(Durable3);
        public static string Durable4 => nameof(Durable4);
        public static string Durable5 => nameof(Durable5);

    }

}

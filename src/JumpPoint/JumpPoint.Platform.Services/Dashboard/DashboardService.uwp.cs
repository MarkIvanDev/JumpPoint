using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Templates;
using Windows.Storage;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services
{
    public static partial class DashboardService
    {
        private const string USER_FOLDERS = "UserFolders";
        private const string SYSTEM_FOLDERS = "SystemFolders";

        static async Task PlatformInitialize()
        {
            if (ApplicationData.Current.Version < 1)
            {
                await ApplicationData.Current.SetVersionAsync(1, (request) =>
                {
                    var deferral = request.GetDeferral();
                    foreach (var item in FolderTemplateService.GetUserFolderTemplates())
                    {
                        var previousKey = item.Humanize();
                        if (Preferences.ContainsKey(previousKey, USER_FOLDERS))
                        {
                            var value = Preferences.Get(previousKey, PlatformGetDefault(item), USER_FOLDERS);
                            Preferences.Set(item.ToString(), value, USER_FOLDERS);
                            Preferences.Remove(previousKey, USER_FOLDERS);
                        }
                    }

                    foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
                    {
                        var previousKey = item.Humanize();
                        if (Preferences.ContainsKey(previousKey, SYSTEM_FOLDERS))
                        {
                            var value = Preferences.Get(previousKey, PlatformGetDefault(item), SYSTEM_FOLDERS);
                            Preferences.Set(item.ToString(), value, SYSTEM_FOLDERS);
                            Preferences.Remove(previousKey, SYSTEM_FOLDERS);
                        }
                    }

                    if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("SettingItems"))
                    {
                        ApplicationData.Current.LocalSettings.DeleteContainer("SettingItems");
                    }

                    deferral.Complete();
                });
            }
            
        }

        #region User Folders

        static ConcurrentDictionary<UserFolderTemplate, bool> PlatformGetUserFolderDefaults()
        {
            var userFolders = new ConcurrentDictionary<UserFolderTemplate, bool>();
            userFolders.TryAdd(UserFolderTemplate.User, true);
            userFolders.TryAdd(UserFolderTemplate.Objects3D, true);
            userFolders.TryAdd(UserFolderTemplate.CameraRoll, true);
            userFolders.TryAdd(UserFolderTemplate.Desktop, true);
            userFolders.TryAdd(UserFolderTemplate.Documents, true);
            userFolders.TryAdd(UserFolderTemplate.Downloads, true);
            userFolders.TryAdd(UserFolderTemplate.Favorites, true);
            userFolders.TryAdd(UserFolderTemplate.Music, true);
            userFolders.TryAdd(UserFolderTemplate.OneDrive, true);
            userFolders.TryAdd(UserFolderTemplate.Pictures, true);
            userFolders.TryAdd(UserFolderTemplate.Playlists, true);
            userFolders.TryAdd(UserFolderTemplate.SavedPictures, true);
            userFolders.TryAdd(UserFolderTemplate.Screenshots, true);
            userFolders.TryAdd(UserFolderTemplate.Videos, true);

            userFolders.TryAdd(UserFolderTemplate.LocalAppData, false);
            userFolders.TryAdd(UserFolderTemplate.LocalAppDataLow, false);
            userFolders.TryAdd(UserFolderTemplate.RoamingAppData, false);

            userFolders.TryAdd(UserFolderTemplate.Cookies, false);
            userFolders.TryAdd(UserFolderTemplate.History, false);
            userFolders.TryAdd(UserFolderTemplate.InternetCache, false);
            userFolders.TryAdd(UserFolderTemplate.Recent, false);
            userFolders.TryAdd(UserFolderTemplate.Templates, false);

            userFolders.TryAdd(UserFolderTemplate.Contacts, false);
            userFolders.TryAdd(UserFolderTemplate.Links, false);
            userFolders.TryAdd(UserFolderTemplate.SavedGames, false);
            userFolders.TryAdd(UserFolderTemplate.Searches, false);

            return userFolders;
        }

        static async Task<ReadOnlyCollection<FolderBase>> PlatformGetUserFolders()
        {
            var folders = new List<FolderBase>();
            foreach (var item in FolderTemplateService.GetUserFolderTemplates())
            {
                if (PlatformGetStatus(item))
                {
                    var folder = await StorageService.GetUserFolder(item);
                    if (!(folder is null))
                    {
                        folders.Add(folder);
                    }
                }
            }
            return new ReadOnlyCollection<FolderBase>(folders);
        }

        static bool PlatformGetDefault(UserFolderTemplate userFolder)
        {
            return userFolders.Value.TryGetValue(userFolder, out var setting) ? setting : false;
        }

        static bool PlatformGetStatus(UserFolderTemplate userFolder)
        {
            return Preferences.Get(userFolder.ToString(), PlatformGetDefault(userFolder), USER_FOLDERS);
        }

        static void PlatformSetStatus(UserFolderTemplate userFolder, bool status)
        {
            Preferences.Set(userFolder.ToString(), status, USER_FOLDERS);
        }

        static async Task<ReadOnlyCollection<UserFolderSetting>> PlatformGetUserFolderSettings()
        {
            return await Task.Run(() =>
            {
                var items = new List<UserFolderSetting>();
                foreach (var item in FolderTemplateService.GetUserFolderTemplates())
                {
                    items.Add(new UserFolderSetting(item));
                }
                return new ReadOnlyCollection<UserFolderSetting>(items);
            });
        }

        #endregion

        #region System Folders

        static ConcurrentDictionary<SystemFolderTemplate, bool> PlatformGetSystemFolderDefaults()
        {
            var systemFolders = new ConcurrentDictionary<SystemFolderTemplate, bool>();
            systemFolders.TryAdd(SystemFolderTemplate.Users, true);
            systemFolders.TryAdd(SystemFolderTemplate.Public, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDesktop, false);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDocuments, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicDownloads, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicMusic, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicPictures, true);
            systemFolders.TryAdd(SystemFolderTemplate.PublicVideos, true);

            systemFolders.TryAdd(SystemFolderTemplate.Fonts, false);
            systemFolders.TryAdd(SystemFolderTemplate.ProgramData, false);

            var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
            if (!string.IsNullOrEmpty(programFiles)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFiles, false);

            var programFilesX86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
            if (!string.IsNullOrEmpty(programFilesX86)) systemFolders.TryAdd(SystemFolderTemplate.ProgramFilesX86, false);

            systemFolders.TryAdd(SystemFolderTemplate.System, false);
            systemFolders.TryAdd(SystemFolderTemplate.Windows, false);

            systemFolders.TryAdd(SystemFolderTemplate.Burn, false);
            return systemFolders;
        }

        static async Task<ReadOnlyCollection<FolderBase>> PlatformGetSystemFolders()
        {
            var folders = new List<FolderBase>();
            foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
            {
                if (PlatformGetStatus(item))
                {
                    var folder = await StorageService.GetSystemFolder(item);
                    if (!(folder is null))
                    {
                        folders.Add(folder);
                    }
                }
            }
            return new ReadOnlyCollection<FolderBase>(folders);
        }

        static bool PlatformGetDefault(SystemFolderTemplate systemFolder)
        {
            return systemFolders.Value.TryGetValue(systemFolder, out var setting) ? setting : false;
        }

        static bool PlatformGetStatus(SystemFolderTemplate systemFolder)
        {
            return Preferences.Get(systemFolder.ToString(), PlatformGetDefault(systemFolder), SYSTEM_FOLDERS);
        }

        static void PlatformSetStatus(SystemFolderTemplate systemFolder, bool status)
        {
            Preferences.Set(systemFolder.ToString(), status, SYSTEM_FOLDERS);
        }

        static async Task<ReadOnlyCollection<SystemFolderSetting>> PlatformGetSystemFolderSettings()
        {
            return await Task.Run(() =>
            {
                var items = new List<SystemFolderSetting>();
                foreach (var item in FolderTemplateService.GetSystemFolderTemplates())
                {
                    items.Add(new SystemFolderSetting(item));
                }
                return new ReadOnlyCollection<SystemFolderSetting>(items);
            });
        }

        #endregion

        #region Setting Links

        static ConcurrentDictionary<SettingLinkTemplate, bool> PlatformGetSettingLinkDefaults()
        {
            var links = new ConcurrentDictionary<SettingLinkTemplate, bool>();
            links.TryAdd(SettingLinkTemplate.Home, true);

            links.TryAdd(SettingLinkTemplate.Display, true);
            links.TryAdd(SettingLinkTemplate.NightLight, false);
            links.TryAdd(SettingLinkTemplate.Sound, false);
            links.TryAdd(SettingLinkTemplate.NotificationsAndActions, false);
            links.TryAdd(SettingLinkTemplate.FocusAssist, false);
            links.TryAdd(SettingLinkTemplate.PowerAndSleep, false);
            links.TryAdd(SettingLinkTemplate.Battery, false);
            links.TryAdd(SettingLinkTemplate.Storage, false);
            links.TryAdd(SettingLinkTemplate.StorageSense, false);
            links.TryAdd(SettingLinkTemplate.DefaultSaveLocations, false);
            links.TryAdd(SettingLinkTemplate.TabletMode, false);
            links.TryAdd(SettingLinkTemplate.Multitasking, false);
            links.TryAdd(SettingLinkTemplate.ProjectingToThisPC, false);
            links.TryAdd(SettingLinkTemplate.SharedExperiences, false);
            links.TryAdd(SettingLinkTemplate.Clipboard, false);
            links.TryAdd(SettingLinkTemplate.RemoteDesktop, false);
            links.TryAdd(SettingLinkTemplate.DeviceEncryption, false);
            links.TryAdd(SettingLinkTemplate.About, false);

            links.TryAdd(SettingLinkTemplate.BluetoothAndOtherDevices, true);
            links.TryAdd(SettingLinkTemplate.PrintersAndScanners, false);
            links.TryAdd(SettingLinkTemplate.Mouse, false);
            links.TryAdd(SettingLinkTemplate.Touchpad, false);
            links.TryAdd(SettingLinkTemplate.Typing, false);
            links.TryAdd(SettingLinkTemplate.PenAndWindowsInk, false);
            links.TryAdd(SettingLinkTemplate.Autoplay, false);
            links.TryAdd(SettingLinkTemplate.USB, false);
            links.TryAdd(SettingLinkTemplate.Wheel, false);
            links.TryAdd(SettingLinkTemplate.Phone, false);

            links.TryAdd(SettingLinkTemplate.NetworkStatus, true);
            links.TryAdd(SettingLinkTemplate.WiFi, false);
            links.TryAdd(SettingLinkTemplate.ManageKnownNetworks, false);
            links.TryAdd(SettingLinkTemplate.Ethernet, false);
            links.TryAdd(SettingLinkTemplate.DialUp, false);
            links.TryAdd(SettingLinkTemplate.CellularAndSIM, false);
            links.TryAdd(SettingLinkTemplate.VPN, false);
            links.TryAdd(SettingLinkTemplate.MobileHotspot, false);
            links.TryAdd(SettingLinkTemplate.DirectAccess, false);
            links.TryAdd(SettingLinkTemplate.NFC, false);
            links.TryAdd(SettingLinkTemplate.AirplaneMode, false);
            links.TryAdd(SettingLinkTemplate.DataUsage, false);
            links.TryAdd(SettingLinkTemplate.Proxy, false);
            links.TryAdd(SettingLinkTemplate.WiFiCalling, false);

            links.TryAdd(SettingLinkTemplate.Background, true);
            links.TryAdd(SettingLinkTemplate.Colors, false);
            links.TryAdd(SettingLinkTemplate.LockScreen, false);
            links.TryAdd(SettingLinkTemplate.Themes, false);
            links.TryAdd(SettingLinkTemplate.Fonts, false);
            links.TryAdd(SettingLinkTemplate.Start, false);
            links.TryAdd(SettingLinkTemplate.ChooseStartPlaces, false);
            links.TryAdd(SettingLinkTemplate.Taskbar, false);

            links.TryAdd(SettingLinkTemplate.AppsAndFeatures, true);
            links.TryAdd(SettingLinkTemplate.DefaultApps, false);
            links.TryAdd(SettingLinkTemplate.ManageOptionalFeatures, false);
            links.TryAdd(SettingLinkTemplate.OfflineMaps, false);
            links.TryAdd(SettingLinkTemplate.AppsForWebsites, false);
            links.TryAdd(SettingLinkTemplate.VideoPlayback, false);
            links.TryAdd(SettingLinkTemplate.Startup, false);

            links.TryAdd(SettingLinkTemplate.YourInfo, true);
            links.TryAdd(SettingLinkTemplate.EmailAndAppAccounts, false);
            links.TryAdd(SettingLinkTemplate.SignInOptions, false);
            links.TryAdd(SettingLinkTemplate.WindowsHelloSetupFace, false);
            links.TryAdd(SettingLinkTemplate.WindowsHelloSetupFingerprint, false);
            links.TryAdd(SettingLinkTemplate.AccessWorkOrSchool, false);
            links.TryAdd(SettingLinkTemplate.FamilyAndOtherUsers, false);
            links.TryAdd(SettingLinkTemplate.SetupAssignedAccess, false);
            links.TryAdd(SettingLinkTemplate.SyncSettings, false);

            links.TryAdd(SettingLinkTemplate.DateAndTime, true);
            links.TryAdd(SettingLinkTemplate.Region, false);
            links.TryAdd(SettingLinkTemplate.Language, false);
            links.TryAdd(SettingLinkTemplate.Speech, false);

            links.TryAdd(SettingLinkTemplate.GameBar, true);
            links.TryAdd(SettingLinkTemplate.Captures, false);
            links.TryAdd(SettingLinkTemplate.Broadcasting, false);
            links.TryAdd(SettingLinkTemplate.GameMode, false);
            links.TryAdd(SettingLinkTemplate.XboxNetworking, false);

            links.TryAdd(SettingLinkTemplate.EaseOfAccessDisplay, true);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessCursorAndPointerSize, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessMagnifier, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessColorFilters, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessHighContrast, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessNarrator, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessAudio, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessClosedCaptions, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessSpeech, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessKeyboard, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessMouse, false);
            links.TryAdd(SettingLinkTemplate.EaseOfAccessEyeControl, false);

            links.TryAdd(SettingLinkTemplate.Cortana, true);
            links.TryAdd(SettingLinkTemplate.CortanaPermissionsAndHistory, false);
            links.TryAdd(SettingLinkTemplate.CortanaAcrossDevices, false);
            links.TryAdd(SettingLinkTemplate.CortanaMoreDetails, false);
            links.TryAdd(SettingLinkTemplate.SearchingWindows, false);

            links.TryAdd(SettingLinkTemplate.PrivacyGeneral, true);
            links.TryAdd(SettingLinkTemplate.PrivacySpeech, false);
            links.TryAdd(SettingLinkTemplate.PrivacyInking, false);
            links.TryAdd(SettingLinkTemplate.PrivacyDiagnosticsAndFeedback, false);
            links.TryAdd(SettingLinkTemplate.PrivacyActivityHistory, false);
            links.TryAdd(SettingLinkTemplate.PrivacyLocation, false);
            links.TryAdd(SettingLinkTemplate.PrivacyCamera, false);
            links.TryAdd(SettingLinkTemplate.PrivacyMicrophone, false);
            links.TryAdd(SettingLinkTemplate.PrivacyNotifications, false);
            links.TryAdd(SettingLinkTemplate.PrivacyAccountInfo, false);
            links.TryAdd(SettingLinkTemplate.PrivacyContacts, false);
            links.TryAdd(SettingLinkTemplate.PrivacyCalendar, false);
            links.TryAdd(SettingLinkTemplate.PrivacyCallHistory, false);
            links.TryAdd(SettingLinkTemplate.PrivacyEmail, false);
            links.TryAdd(SettingLinkTemplate.PrivacyTasks, false);
            links.TryAdd(SettingLinkTemplate.PrivacyMessaging, false);
            links.TryAdd(SettingLinkTemplate.PrivacyRadios, false);
            links.TryAdd(SettingLinkTemplate.PrivacyOtherDevices, false);
            links.TryAdd(SettingLinkTemplate.PrivacyBackgroundApps, false);
            links.TryAdd(SettingLinkTemplate.PrivacyAppDiagnostics, false);
            links.TryAdd(SettingLinkTemplate.PrivacyAutomaticFileDownloads, false);
            links.TryAdd(SettingLinkTemplate.PrivacyDocuments, false);
            links.TryAdd(SettingLinkTemplate.PrivacyPictures, false);
            links.TryAdd(SettingLinkTemplate.PrivacyVideos, false);
            links.TryAdd(SettingLinkTemplate.PrivacyFileSystem, false);
            links.TryAdd(SettingLinkTemplate.PrivacyMotion, false);
            links.TryAdd(SettingLinkTemplate.PrivacyVoiceActivation, false);
            links.TryAdd(SettingLinkTemplate.PrivacyEyeTracker, false);

            links.TryAdd(SettingLinkTemplate.WindowsUpdate, true);
            links.TryAdd(SettingLinkTemplate.WindowsUpdateAdvancedOptions, false);
            links.TryAdd(SettingLinkTemplate.WindowsUpdateRestartOptions, false);
            links.TryAdd(SettingLinkTemplate.WindowsUpdateHistory, false);
            links.TryAdd(SettingLinkTemplate.DeliveryOptimization, false);
            links.TryAdd(SettingLinkTemplate.WindowsSecurity, false);
            links.TryAdd(SettingLinkTemplate.Backup, false);
            links.TryAdd(SettingLinkTemplate.Troubleshoot, false);
            links.TryAdd(SettingLinkTemplate.Recovery, false);
            links.TryAdd(SettingLinkTemplate.Activation, false);
            links.TryAdd(SettingLinkTemplate.FindMyDevice, false);
            links.TryAdd(SettingLinkTemplate.ForDevelopers, false);
            links.TryAdd(SettingLinkTemplate.WindowsInsiderProgram, false);

            links.TryAdd(SettingLinkTemplate.MixedRealityAudioAndSpeech, true);
            links.TryAdd(SettingLinkTemplate.MixedRealityEnvironment, false);
            links.TryAdd(SettingLinkTemplate.MixedRealityHeadsetDisplay, false);
            links.TryAdd(SettingLinkTemplate.MixedRealityUninstall, false);
            return links;
        }

        #endregion

    }
}

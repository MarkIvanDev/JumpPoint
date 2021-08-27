using Humanizer;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Items;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Extensions
{
    public static class SettingsAppLinkProvider
    {
        private static readonly Lazy<ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo>> settingLinks;

        static SettingsAppLinkProvider()
        {
            settingLinks = new Lazy<ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo>>(GetSettingLinkInfos);
        }

        static ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo> GetSettingLinkInfos()
        {
            var links = new ConcurrentDictionary<SettingLinkTemplate, SettingLinkInfo>();
            links.TryAdd(SettingLinkTemplate.Home, new SettingLinkInfo(SettingLinkGroup.None, "ms-settings:"));

            links.TryAdd(SettingLinkTemplate.Display, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:display"));
            links.TryAdd(SettingLinkTemplate.NightLight, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:nightlight"));
            links.TryAdd(SettingLinkTemplate.Sound, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:sound"));
            links.TryAdd(SettingLinkTemplate.NotificationsAndActions, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:notifications"));
            links.TryAdd(SettingLinkTemplate.FocusAssist, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:quiethours"));
            links.TryAdd(SettingLinkTemplate.PowerAndSleep, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:powersleep"));
            links.TryAdd(SettingLinkTemplate.Battery, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:batterysaver"));
            links.TryAdd(SettingLinkTemplate.Storage, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:storagesense"));
            links.TryAdd(SettingLinkTemplate.StorageSense, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:storagepolicies"));
            links.TryAdd(SettingLinkTemplate.DefaultSaveLocations, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:savelocations"));
            links.TryAdd(SettingLinkTemplate.TabletMode, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:tabletmode"));
            links.TryAdd(SettingLinkTemplate.Multitasking, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:multitasking"));
            links.TryAdd(SettingLinkTemplate.ProjectingToThisPC, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:project"));
            links.TryAdd(SettingLinkTemplate.SharedExperiences, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:crossdevice"));
            links.TryAdd(SettingLinkTemplate.Clipboard, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:clipboard"));
            links.TryAdd(SettingLinkTemplate.RemoteDesktop, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:remotedesktop"));
            links.TryAdd(SettingLinkTemplate.DeviceEncryption, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:deviceencryption"));
            links.TryAdd(SettingLinkTemplate.About, new SettingLinkInfo(SettingLinkGroup.System, "ms-settings:about"));

            links.TryAdd(SettingLinkTemplate.BluetoothAndOtherDevices, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:bluetooth"));
            links.TryAdd(SettingLinkTemplate.PrintersAndScanners, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:printers"));
            links.TryAdd(SettingLinkTemplate.Mouse, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:mousetouchpad"));
            links.TryAdd(SettingLinkTemplate.Touchpad, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:devices-touchpad"));
            links.TryAdd(SettingLinkTemplate.Typing, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:typing"));
            links.TryAdd(SettingLinkTemplate.PenAndWindowsInk, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:pen"));
            links.TryAdd(SettingLinkTemplate.Autoplay, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:autoplay"));
            links.TryAdd(SettingLinkTemplate.USB, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:usb"));
            links.TryAdd(SettingLinkTemplate.Wheel, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:wheel"));
            links.TryAdd(SettingLinkTemplate.Phone, new SettingLinkInfo(SettingLinkGroup.Devices, "ms-settings:mobile-devices"));

            links.TryAdd(SettingLinkTemplate.NetworkStatus, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-status"));
            links.TryAdd(SettingLinkTemplate.WiFi, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-wifi"));
            links.TryAdd(SettingLinkTemplate.ManageKnownNetworks, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-wifisettings"));
            links.TryAdd(SettingLinkTemplate.Ethernet, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-ethernet"));
            links.TryAdd(SettingLinkTemplate.DialUp, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-dialup"));
            links.TryAdd(SettingLinkTemplate.CellularAndSIM, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-cellular"));
            links.TryAdd(SettingLinkTemplate.VPN, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-vpn"));
            links.TryAdd(SettingLinkTemplate.MobileHotspot, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-mobilehotspot"));
            links.TryAdd(SettingLinkTemplate.DirectAccess, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-directaccess"));
            links.TryAdd(SettingLinkTemplate.NFC, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:nfctransactions"));
            links.TryAdd(SettingLinkTemplate.AirplaneMode, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-airplanemode"));
            links.TryAdd(SettingLinkTemplate.DataUsage, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:datausage"));
            links.TryAdd(SettingLinkTemplate.Proxy, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-proxy"));
            links.TryAdd(SettingLinkTemplate.WiFiCalling, new SettingLinkInfo(SettingLinkGroup.NetworkAndInternet, "ms-settings:network-wificalling"));

            links.TryAdd(SettingLinkTemplate.Background, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:personalization-background"));
            links.TryAdd(SettingLinkTemplate.Colors, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:personalization-colors"));
            links.TryAdd(SettingLinkTemplate.LockScreen, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:lockscreen"));
            links.TryAdd(SettingLinkTemplate.Themes, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:themes"));
            links.TryAdd(SettingLinkTemplate.Fonts, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:fonts"));
            links.TryAdd(SettingLinkTemplate.Start, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:personalization-start"));
            links.TryAdd(SettingLinkTemplate.ChooseStartPlaces, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:personalization-start-places"));
            links.TryAdd(SettingLinkTemplate.Taskbar, new SettingLinkInfo(SettingLinkGroup.Personalization, "ms-settings:taskbar"));

            links.TryAdd(SettingLinkTemplate.AppsAndFeatures, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:appsfeatures"));
            links.TryAdd(SettingLinkTemplate.DefaultApps, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:defaultapps"));
            links.TryAdd(SettingLinkTemplate.ManageOptionalFeatures, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:optionalfeatures"));
            links.TryAdd(SettingLinkTemplate.OfflineMaps, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:maps"));
            links.TryAdd(SettingLinkTemplate.AppsForWebsites, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:appsforwebsites"));
            links.TryAdd(SettingLinkTemplate.VideoPlayback, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:videoplayback"));
            links.TryAdd(SettingLinkTemplate.Startup, new SettingLinkInfo(SettingLinkGroup.Apps, "ms-settings:startupapps"));

            links.TryAdd(SettingLinkTemplate.YourInfo, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:yourinfo"));
            links.TryAdd(SettingLinkTemplate.EmailAndAppAccounts, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:emailandaccounts"));
            links.TryAdd(SettingLinkTemplate.SignInOptions, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:signinoptions"));
            links.TryAdd(SettingLinkTemplate.WindowsHelloSetupFace, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:signinoptions-launchfaceenrollment"));
            links.TryAdd(SettingLinkTemplate.WindowsHelloSetupFingerprint, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:signinoptions-launchfingerprintenrollment"));
            links.TryAdd(SettingLinkTemplate.AccessWorkOrSchool, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:workplace"));
            links.TryAdd(SettingLinkTemplate.FamilyAndOtherUsers, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:otherusers"));
            links.TryAdd(SettingLinkTemplate.SetupAssignedAccess, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:assignedaccess"));
            links.TryAdd(SettingLinkTemplate.SyncSettings, new SettingLinkInfo(SettingLinkGroup.Accounts, "ms-settings:sync"));

            links.TryAdd(SettingLinkTemplate.DateAndTime, new SettingLinkInfo(SettingLinkGroup.TimeAndLanguage, "ms-settings:dateandtime"));
            links.TryAdd(SettingLinkTemplate.Region, new SettingLinkInfo(SettingLinkGroup.TimeAndLanguage, "ms-settings:regionformatting"));
            links.TryAdd(SettingLinkTemplate.Language, new SettingLinkInfo(SettingLinkGroup.TimeAndLanguage, "ms-settings:regionlanguage"));
            links.TryAdd(SettingLinkTemplate.Speech, new SettingLinkInfo(SettingLinkGroup.TimeAndLanguage, "ms-settings:speech"));

            links.TryAdd(SettingLinkTemplate.GameBar, new SettingLinkInfo(SettingLinkGroup.Gaming, "ms-settings:gaming-gamebar"));
            links.TryAdd(SettingLinkTemplate.Captures, new SettingLinkInfo(SettingLinkGroup.Gaming, "ms-settings:gaming-gamedvr"));
            links.TryAdd(SettingLinkTemplate.Broadcasting, new SettingLinkInfo(SettingLinkGroup.Gaming, "ms-settings:gaming-broadcasting"));
            links.TryAdd(SettingLinkTemplate.GameMode, new SettingLinkInfo(SettingLinkGroup.Gaming, "ms-settings:gaming-gamemode"));
            links.TryAdd(SettingLinkTemplate.XboxNetworking, new SettingLinkInfo(SettingLinkGroup.Gaming, "ms-settings:gaming-xboxnetworking"));

            links.TryAdd(SettingLinkTemplate.EaseOfAccessDisplay, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-display"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessCursorAndPointerSize, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-cursorandpointersize"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessMagnifier, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-magnifier"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessColorFilters, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-colorfilter"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessHighContrast, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-highcontrast"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessNarrator, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-narrator"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessAudio, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-audio"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessClosedCaptions, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-closedcaptioning"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessSpeech, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-speech"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessKeyboard, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-keyboard"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessMouse, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-mouse"));
            links.TryAdd(SettingLinkTemplate.EaseOfAccessEyeControl, new SettingLinkInfo(SettingLinkGroup.EaseOfAccess, "ms-settings:easeofaccess-eyecontrol"));

            links.TryAdd(SettingLinkTemplate.Cortana, new SettingLinkInfo(SettingLinkGroup.Cortana, "ms-settings:cortana-talktocortana"));
            links.TryAdd(SettingLinkTemplate.CortanaPermissionsAndHistory, new SettingLinkInfo(SettingLinkGroup.Cortana, "ms-settings:cortana-permissions"));
            links.TryAdd(SettingLinkTemplate.CortanaAcrossDevices, new SettingLinkInfo(SettingLinkGroup.Cortana, "ms-settings:cortana-notifications"));
            links.TryAdd(SettingLinkTemplate.CortanaMoreDetails, new SettingLinkInfo(SettingLinkGroup.Cortana, "ms-settings:cortana-moredetails"));
            links.TryAdd(SettingLinkTemplate.SearchingWindows, new SettingLinkInfo(SettingLinkGroup.Cortana, "ms-settings:cortana-windowssearch"));

            links.TryAdd(SettingLinkTemplate.PrivacyGeneral, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-general"));
            links.TryAdd(SettingLinkTemplate.PrivacySpeech, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-speech"));
            links.TryAdd(SettingLinkTemplate.PrivacyInking, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-speechtyping"));
            links.TryAdd(SettingLinkTemplate.PrivacyDiagnosticsAndFeedback, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-feedback"));
            links.TryAdd(SettingLinkTemplate.PrivacyActivityHistory, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-activityhistory"));
            links.TryAdd(SettingLinkTemplate.PrivacyLocation, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-location"));
            links.TryAdd(SettingLinkTemplate.PrivacyCamera, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-webcam"));
            links.TryAdd(SettingLinkTemplate.PrivacyMicrophone, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-microphone"));
            links.TryAdd(SettingLinkTemplate.PrivacyNotifications, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-notifications"));
            links.TryAdd(SettingLinkTemplate.PrivacyAccountInfo, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-accountinfo"));
            links.TryAdd(SettingLinkTemplate.PrivacyContacts, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-contacts"));
            links.TryAdd(SettingLinkTemplate.PrivacyCalendar, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-calendar"));
            links.TryAdd(SettingLinkTemplate.PrivacyCallHistory, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-callhistory"));
            links.TryAdd(SettingLinkTemplate.PrivacyEmail, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-email"));
            links.TryAdd(SettingLinkTemplate.PrivacyTasks, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-tasks"));
            links.TryAdd(SettingLinkTemplate.PrivacyMessaging, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-messaging"));
            links.TryAdd(SettingLinkTemplate.PrivacyRadios, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-radios"));
            links.TryAdd(SettingLinkTemplate.PrivacyOtherDevices, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-customdevices"));
            links.TryAdd(SettingLinkTemplate.PrivacyBackgroundApps, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-backgroundapps"));
            links.TryAdd(SettingLinkTemplate.PrivacyAppDiagnostics, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-appdiagnostics"));
            links.TryAdd(SettingLinkTemplate.PrivacyAutomaticFileDownloads, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-automaticfiledownloads"));
            links.TryAdd(SettingLinkTemplate.PrivacyDocuments, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-documents"));
            links.TryAdd(SettingLinkTemplate.PrivacyPictures, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-pictures"));
            links.TryAdd(SettingLinkTemplate.PrivacyVideos, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-videos"));
            links.TryAdd(SettingLinkTemplate.PrivacyFileSystem, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-broadfilesystemaccess"));
            links.TryAdd(SettingLinkTemplate.PrivacyMotion, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-motion"));
            links.TryAdd(SettingLinkTemplate.PrivacyVoiceActivation, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-voiceactivation"));
            links.TryAdd(SettingLinkTemplate.PrivacyEyeTracker, new SettingLinkInfo(SettingLinkGroup.Privacy, "ms-settings:privacy-eyetracker"));

            links.TryAdd(SettingLinkTemplate.WindowsUpdate, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsupdate"));
            links.TryAdd(SettingLinkTemplate.WindowsUpdateAdvancedOptions, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsupdate-options"));
            links.TryAdd(SettingLinkTemplate.WindowsUpdateRestartOptions, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsupdate-restartoptions"));
            links.TryAdd(SettingLinkTemplate.WindowsUpdateHistory, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsupdate-history"));
            links.TryAdd(SettingLinkTemplate.DeliveryOptimization, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:delivery-optimization"));
            links.TryAdd(SettingLinkTemplate.WindowsSecurity, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsdefender"));
            links.TryAdd(SettingLinkTemplate.Backup, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:backup"));
            links.TryAdd(SettingLinkTemplate.Troubleshoot, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:troubleshoot"));
            links.TryAdd(SettingLinkTemplate.Recovery, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:recovery"));
            links.TryAdd(SettingLinkTemplate.Activation, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:activation"));
            links.TryAdd(SettingLinkTemplate.FindMyDevice, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:findmydevice"));
            links.TryAdd(SettingLinkTemplate.ForDevelopers, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:developers"));
            links.TryAdd(SettingLinkTemplate.WindowsInsiderProgram, new SettingLinkInfo(SettingLinkGroup.UpdateAndSecurity, "ms-settings:windowsinsider"));

            links.TryAdd(SettingLinkTemplate.MixedRealityAudioAndSpeech, new SettingLinkInfo(SettingLinkGroup.MixedReality, "ms-settings:holographic-audio"));
            links.TryAdd(SettingLinkTemplate.MixedRealityEnvironment, new SettingLinkInfo(SettingLinkGroup.MixedReality, "ms-settings:privacy-holographic-environment"));
            links.TryAdd(SettingLinkTemplate.MixedRealityHeadsetDisplay, new SettingLinkInfo(SettingLinkGroup.MixedReality, "ms-settings:holographic-headset"));
            links.TryAdd(SettingLinkTemplate.MixedRealityUninstall, new SettingLinkInfo(SettingLinkGroup.MixedReality, "ms-settings:holographic-management"));
            return links;
        }

        public static async Task<IList<AppLinkPayload>> GetPayloads()
        {
            return await Task.Run(() =>
            {
                var settings = new List<AppLinkPayload>();
                foreach (var item in GetTemplates())
                {
                    var setting = GetPayload(item);
                    if (!(setting is null))
                    {
                        settings.Add(setting);
                    }
                }
                return settings;
            });
        }

        static AppLinkPayload GetPayload(SettingLinkTemplate template)
        {
            if (settingLinks.Value.TryGetValue(template, out var info))
            {
                var name = $"{template.Humanize()} - {info.Group.Humanize()}";
                return new AppLinkPayload
                {
                    Link = info.Path,
                    Name = name,
                    Description = name,
                    AppName = "Windows Settings",
                    LaunchTypes = (int)AppLinkLaunchTypes.Uri,
                    LogoUri = new Uri($@"ms-appx:///Assets/Icons/Settings/{template}.png")
                };
            }
            return null;
        }

        static IEnumerable<SettingLinkTemplate> GetTemplates()
        {
            yield return SettingLinkTemplate.Home;

            yield return SettingLinkTemplate.Display;
            yield return SettingLinkTemplate.NightLight;
            yield return SettingLinkTemplate.Sound;
            yield return SettingLinkTemplate.NotificationsAndActions;
            yield return SettingLinkTemplate.FocusAssist;
            yield return SettingLinkTemplate.PowerAndSleep;
            yield return SettingLinkTemplate.Battery;
            yield return SettingLinkTemplate.Storage;
            yield return SettingLinkTemplate.StorageSense;
            yield return SettingLinkTemplate.DefaultSaveLocations;
            yield return SettingLinkTemplate.TabletMode;
            yield return SettingLinkTemplate.Multitasking;
            yield return SettingLinkTemplate.ProjectingToThisPC;
            yield return SettingLinkTemplate.SharedExperiences;
            yield return SettingLinkTemplate.Clipboard;
            yield return SettingLinkTemplate.RemoteDesktop;
            yield return SettingLinkTemplate.DeviceEncryption;
            yield return SettingLinkTemplate.About;

            yield return SettingLinkTemplate.BluetoothAndOtherDevices;
            yield return SettingLinkTemplate.PrintersAndScanners;
            yield return SettingLinkTemplate.Mouse;
            yield return SettingLinkTemplate.Touchpad;
            yield return SettingLinkTemplate.Typing;
            yield return SettingLinkTemplate.PenAndWindowsInk;
            yield return SettingLinkTemplate.Autoplay;
            yield return SettingLinkTemplate.USB;
            yield return SettingLinkTemplate.Wheel;
            yield return SettingLinkTemplate.Phone;

            yield return SettingLinkTemplate.NetworkStatus;
            yield return SettingLinkTemplate.WiFi;
            yield return SettingLinkTemplate.ManageKnownNetworks;
            yield return SettingLinkTemplate.Ethernet;
            yield return SettingLinkTemplate.DialUp;
            yield return SettingLinkTemplate.CellularAndSIM;
            yield return SettingLinkTemplate.VPN;
            yield return SettingLinkTemplate.MobileHotspot;
            yield return SettingLinkTemplate.DirectAccess;
            yield return SettingLinkTemplate.NFC;
            yield return SettingLinkTemplate.AirplaneMode;
            yield return SettingLinkTemplate.DataUsage;
            yield return SettingLinkTemplate.Proxy;
            yield return SettingLinkTemplate.WiFiCalling;

            yield return SettingLinkTemplate.Background;
            yield return SettingLinkTemplate.Colors;
            yield return SettingLinkTemplate.LockScreen;
            yield return SettingLinkTemplate.Themes;
            yield return SettingLinkTemplate.Fonts;
            yield return SettingLinkTemplate.Start;
            yield return SettingLinkTemplate.ChooseStartPlaces;
            yield return SettingLinkTemplate.Taskbar;

            yield return SettingLinkTemplate.AppsAndFeatures;
            yield return SettingLinkTemplate.DefaultApps;
            yield return SettingLinkTemplate.ManageOptionalFeatures;
            yield return SettingLinkTemplate.OfflineMaps;
            yield return SettingLinkTemplate.AppsForWebsites;
            yield return SettingLinkTemplate.VideoPlayback;
            yield return SettingLinkTemplate.Startup;

            yield return SettingLinkTemplate.YourInfo;
            yield return SettingLinkTemplate.EmailAndAppAccounts;
            yield return SettingLinkTemplate.SignInOptions;
            yield return SettingLinkTemplate.WindowsHelloSetupFace;
            yield return SettingLinkTemplate.WindowsHelloSetupFingerprint;
            yield return SettingLinkTemplate.AccessWorkOrSchool;
            yield return SettingLinkTemplate.FamilyAndOtherUsers;
            yield return SettingLinkTemplate.SetupAssignedAccess;
            yield return SettingLinkTemplate.SyncSettings;

            yield return SettingLinkTemplate.DateAndTime;
            yield return SettingLinkTemplate.Region;
            yield return SettingLinkTemplate.Language;
            yield return SettingLinkTemplate.Speech;

            yield return SettingLinkTemplate.GameBar;
            yield return SettingLinkTemplate.Captures;
            yield return SettingLinkTemplate.Broadcasting;
            yield return SettingLinkTemplate.GameMode;
            yield return SettingLinkTemplate.XboxNetworking;

            yield return SettingLinkTemplate.EaseOfAccessDisplay;
            yield return SettingLinkTemplate.EaseOfAccessCursorAndPointerSize;
            yield return SettingLinkTemplate.EaseOfAccessMagnifier;
            yield return SettingLinkTemplate.EaseOfAccessColorFilters;
            yield return SettingLinkTemplate.EaseOfAccessHighContrast;
            yield return SettingLinkTemplate.EaseOfAccessNarrator;
            yield return SettingLinkTemplate.EaseOfAccessAudio;
            yield return SettingLinkTemplate.EaseOfAccessClosedCaptions;
            yield return SettingLinkTemplate.EaseOfAccessSpeech;
            yield return SettingLinkTemplate.EaseOfAccessKeyboard;
            yield return SettingLinkTemplate.EaseOfAccessMouse;
            yield return SettingLinkTemplate.EaseOfAccessEyeControl;

            yield return SettingLinkTemplate.Cortana;
            yield return SettingLinkTemplate.CortanaPermissionsAndHistory;
            yield return SettingLinkTemplate.CortanaAcrossDevices;
            yield return SettingLinkTemplate.CortanaMoreDetails;
            yield return SettingLinkTemplate.SearchingWindows;

            yield return SettingLinkTemplate.PrivacyGeneral;
            yield return SettingLinkTemplate.PrivacySpeech;
            yield return SettingLinkTemplate.PrivacyInking;
            yield return SettingLinkTemplate.PrivacyDiagnosticsAndFeedback;
            yield return SettingLinkTemplate.PrivacyActivityHistory;
            yield return SettingLinkTemplate.PrivacyLocation;
            yield return SettingLinkTemplate.PrivacyCamera;
            yield return SettingLinkTemplate.PrivacyMicrophone;
            yield return SettingLinkTemplate.PrivacyNotifications;
            yield return SettingLinkTemplate.PrivacyAccountInfo;
            yield return SettingLinkTemplate.PrivacyContacts;
            yield return SettingLinkTemplate.PrivacyCalendar;
            yield return SettingLinkTemplate.PrivacyCallHistory;
            yield return SettingLinkTemplate.PrivacyEmail;
            yield return SettingLinkTemplate.PrivacyTasks;
            yield return SettingLinkTemplate.PrivacyMessaging;
            yield return SettingLinkTemplate.PrivacyRadios;
            yield return SettingLinkTemplate.PrivacyOtherDevices;
            yield return SettingLinkTemplate.PrivacyBackgroundApps;
            yield return SettingLinkTemplate.PrivacyAppDiagnostics;
            yield return SettingLinkTemplate.PrivacyAutomaticFileDownloads;
            yield return SettingLinkTemplate.PrivacyDocuments;
            yield return SettingLinkTemplate.PrivacyPictures;
            yield return SettingLinkTemplate.PrivacyVideos;
            yield return SettingLinkTemplate.PrivacyFileSystem;
            yield return SettingLinkTemplate.PrivacyMotion;
            yield return SettingLinkTemplate.PrivacyVoiceActivation;
            yield return SettingLinkTemplate.PrivacyEyeTracker;


            yield return SettingLinkTemplate.WindowsUpdate;
            yield return SettingLinkTemplate.WindowsUpdateAdvancedOptions;
            yield return SettingLinkTemplate.WindowsUpdateRestartOptions;
            yield return SettingLinkTemplate.WindowsUpdateHistory;
            yield return SettingLinkTemplate.DeliveryOptimization;
            yield return SettingLinkTemplate.WindowsSecurity;
            yield return SettingLinkTemplate.Backup;
            yield return SettingLinkTemplate.Troubleshoot;
            yield return SettingLinkTemplate.Recovery;
            yield return SettingLinkTemplate.Activation;
            yield return SettingLinkTemplate.FindMyDevice;
            yield return SettingLinkTemplate.ForDevelopers;
            yield return SettingLinkTemplate.WindowsInsiderProgram;

            yield return SettingLinkTemplate.MixedRealityAudioAndSpeech;
            yield return SettingLinkTemplate.MixedRealityEnvironment;
            yield return SettingLinkTemplate.MixedRealityHeadsetDisplay;
            yield return SettingLinkTemplate.MixedRealityUninstall;
        }

        private class SettingLinkInfo
        {
            public SettingLinkInfo(SettingLinkGroup group, string path)
            {
                Group = group;
                Path = path;
            }

            public SettingLinkGroup Group { get; }

            public string Path { get; }
        }

    }

    public enum SettingLinkTemplate
    {
        None = 0,
        Home = 1,

        Display = 2,
        NightLight = 3,
        Sound = 4,
        NotificationsAndActions = 5,
        FocusAssist = 6,
        PowerAndSleep = 7,
        Battery = 8,
        Storage = 9,
        StorageSense = 10,
        DefaultSaveLocations = 11,
        TabletMode = 12,
        Multitasking = 13,
        ProjectingToThisPC = 14,
        SharedExperiences = 15,
        Clipboard = 16,
        RemoteDesktop = 17,
        DeviceEncryption = 18,
        About = 19,

        BluetoothAndOtherDevices = 31,
        PrintersAndScanners = 32,
        Mouse = 33,
        Touchpad = 34,
        Typing = 35,
        PenAndWindowsInk = 36,
        Autoplay = 37,
        USB = 38,
        Wheel = 39,
        Phone = 40,

        NetworkStatus = 51,
        [Description("Wi-Fi")]
        WiFi = 52,
        ManageKnownNetworks = 53,
        Ethernet = 54,
        [Description("Dial-up")]
        DialUp = 55,
        CellularAndSIM = 56,
        VPN = 57,
        MobileHotspot = 58,
        DirectAccess = 59,
        NFC = 60,
        AirplaneMode = 61,
        DataUsage = 62,
        Proxy = 63,
        [Description("Wi-Fi Calling")]
        WiFiCalling = 64,

        Background = 81,
        Colors = 82,
        LockScreen = 83,
        Themes = 84,
        Fonts = 85,
        Start = 86,
        ChooseStartPlaces = 87,
        Taskbar = 88,

        AppsAndFeatures = 101,
        DefaultApps = 102,
        ManageOptionalFeatures = 103,
        OfflineMaps = 104,
        AppsForWebsites = 105,
        VideoPlayback = 106,
        Startup = 107,

        YourInfo = 121,
        EmailAndAppAccounts = 122,
        [Description("Sign-in Options")]
        SignInOptions = 123,
        [Description("Windows Hello Setup (Face)")]
        WindowsHelloSetupFace = 124,
        [Description("Windows Hello Setup (Fingerprint)")]
        WindowsHelloSetupFingerprint = 125,
        AccessWorkOrSchool = 126,
        FamilyAndOtherUsers = 127,
        SetupAssignedAccess = 128,
        SyncSettings = 129,

        DateAndTime = 141,
        Region = 142,
        Language = 143,
        Speech = 144,

        GameBar = 161,
        Captures = 162,
        Broadcasting = 163,
        GameMode = 164,
        XboxNetworking = 165,

        [Description("Display")]
        EaseOfAccessDisplay = 181,
        [Description("Cursor and pointer size")]
        EaseOfAccessCursorAndPointerSize = 182,
        [Description("Magnifier")]
        EaseOfAccessMagnifier = 183,
        [Description("Color filters")]
        EaseOfAccessColorFilters = 184,
        [Description("High contrast")]
        EaseOfAccessHighContrast = 185,
        [Description("Narrator")]
        EaseOfAccessNarrator = 186,
        [Description("Audio")]
        EaseOfAccessAudio = 187,
        [Description("Closed captions")]
        EaseOfAccessClosedCaptions = 188,
        [Description("Speech")]
        EaseOfAccessSpeech = 189,
        [Description("Keyboard")]
        EaseOfAccessKeyboard = 190,
        [Description("Mouse")]
        EaseOfAccessMouse = 191,
        [Description("Eye control")]
        EaseOfAccessEyeControl = 192,

        Cortana = 211,
        CortanaPermissionsAndHistory = 212,
        CortanaAcrossDevices = 213,
        CortanaMoreDetails = 214,
        SearchingWindows = 215,

        [Description("General")]
        PrivacyGeneral = 231,
        [Description("Speech")]
        PrivacySpeech = 232,
        [Description("Inking")]
        PrivacyInking = 233,
        [Description("Diagnostics and feedback")]
        PrivacyDiagnosticsAndFeedback = 234,
        [Description("Activity history")]
        PrivacyActivityHistory = 235,
        [Description("Location")]
        PrivacyLocation = 236,
        [Description("Camera")]
        PrivacyCamera = 237,
        [Description("Microphone")]
        PrivacyMicrophone = 238,
        [Description("Notifications")]
        PrivacyNotifications = 239,
        [Description("Account info")]
        PrivacyAccountInfo = 240,
        [Description("Contacts")]
        PrivacyContacts = 241,
        [Description("Calendar")]
        PrivacyCalendar = 242,
        [Description("Call history")]
        PrivacyCallHistory = 243,
        [Description("Email")]
        PrivacyEmail = 244,
        [Description("Tasks")]
        PrivacyTasks = 245,
        [Description("Messaging")]
        PrivacyMessaging = 246,
        [Description("Radios")]
        PrivacyRadios = 247,
        [Description("Other devices")]
        PrivacyOtherDevices = 248,
        [Description("Background apps")]
        PrivacyBackgroundApps = 249,
        [Description("App diagnostics")]
        PrivacyAppDiagnostics = 250,
        [Description("Automatic file downloads")]
        PrivacyAutomaticFileDownloads = 251,
        [Description("Documents")]
        PrivacyDocuments = 252,
        [Description("Pictures")]
        PrivacyPictures = 253,
        [Description("Videos")]
        PrivacyVideos = 254,
        [Description("File system")]
        PrivacyFileSystem = 255,
        [Description("Motion")]
        PrivacyMotion = 256,
        [Description("Voice activation")]
        PrivacyVoiceActivation = 257,
        [Description("Eye tracker")]
        PrivacyEyeTracker = 258,

        WindowsUpdate = 271,
        WindowsUpdateAdvancedOptions = 272,
        WindowsUpdateRestartOptions = 273,
        WindowsUpdateHistory = 274,
        DeliveryOptimization = 275,
        WindowsSecurity = 276,
        Backup = 277,
        Troubleshoot = 278,
        Recovery = 279,
        Activation = 280,
        FindMyDevice = 281,
        ForDevelopers = 282,
        WindowsInsiderProgram = 283,

        [Description("Audio and speech")]
        MixedRealityAudioAndSpeech = 301,
        [Description("Environment")]
        MixedRealityEnvironment = 302,
        [Description("Headset display")]
        MixedRealityHeadsetDisplay = 303,
        [Description("Uninstall")]
        MixedRealityUninstall = 304
    }

    public enum SettingLinkGroup
    {
        [Description("Settings")]
        None = 0,
        System = 1,
        Devices = 2,
        Phone = 3,
        NetworkAndInternet = 4,
        Personalization = 5,
        Apps = 6,
        Accounts = 7,
        TimeAndLanguage = 8,
        Gaming = 9,
        EaseOfAccess = 10,
        Cortana = 11,
        Privacy = 12,
        UpdateAndSecurity = 13,
        MixedReality = 14
    }

}

using System;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Templates
{
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

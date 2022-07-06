using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Glif.Core.Text;
using NittyGritty;
using NittyGritty.Platform.Theme;
using NittyGritty.Services.Core;
using Xamarin.Essentials;
using AppTheme = NittyGritty.Platform.Theme.AppTheme;
using System.Threading;
#if UWP
using Windows.ApplicationModel;
#endif

namespace JumpPoint.Platform.Services
{
    public class AppSettings : ObservableObject
    {
        public const string STARTUP_TASK_ID =
#if BETA
            "JumpPointBeta_StartupTask";
#else
            "JumpPoint_StartupTask";
#endif

        private readonly SemaphoreSlim refreshStartupSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim changeStartupSemaphore = new SemaphoreSlim(1, 1);

        private readonly IThemeService themeService;
        private readonly IDialogService dialogService;

        public AppSettings(IThemeService themeService, IDialogService dialogService)
        {
            this.themeService = themeService;
            this.dialogService = dialogService;
            Themes = new ReadOnlyCollection<AppTheme>(new List<AppTheme>
            {
                AppTheme.Default,
                AppTheme.Light,
                AppTheme.Dark
            });
            CopyPathDelimiters = new ReadOnlyCollection<CopyPathDelimiter>(new List<CopyPathDelimiter>
            {
                CopyPathDelimiter.NewLine,
                CopyPathDelimiter.Tab,
                CopyPathDelimiter.Comma,
                CopyPathDelimiter.Pipe,
                CopyPathDelimiter.Colon,
                CopyPathDelimiter.Semicolon,
                CopyPathDelimiter.Slash,
                CopyPathDelimiter.Backslash,
                CopyPathDelimiter.Plus,
                CopyPathDelimiter.Hyphen
            });
        }

        #region Personalization
        
        #region Theme

        public ReadOnlyCollection<AppTheme> Themes { get; }

        public AppTheme Theme
        {
            get { return (AppTheme)Preferences.Get(nameof(Theme), (int)AppTheme.Default); }
            set
            {
                if (Theme != value)
                {
                    Preferences.Set(nameof(Theme), (int)value);
                    RaisePropertyChanged();
                    themeService.SetTheme(value);
                }
            }
        }

        #endregion

        #region Font

        public string Font
        {
            get { return Preferences.Get(nameof(Font), "Segoe UI"); }
            set
            {
                if (Font != value)
                {
                    Preferences.Set(nameof(Font), value);
                    RaisePropertyChanged();
                }
            }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)Preferences.Get(nameof(FontStyle), (int)FontStyle.Normal); }
            set
            {
                if (FontStyle != value)
                {
                    Preferences.Set(nameof(FontStyle), (int)value);
                    RaisePropertyChanged();
                }
            }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)Preferences.Get(nameof(FontWeight), (ushort)FontWeight.Normal); }
            set
            {
                if (FontWeight != value)
                {
                    Preferences.Set(nameof(FontWeight), (ushort)value);
                    RaisePropertyChanged();
                }
            }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)Preferences.Get(nameof(FontStretch), (int)FontStretch.Normal); }
            set
            {
                if (FontStretch != value)
                {
                    Preferences.Set(nameof(FontStretch), (int)value);
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Copy Path Delimiter

        public ReadOnlyCollection<CopyPathDelimiter> CopyPathDelimiters { get; }

        public CopyPathDelimiter CopyPathDelimiter
        {
            get { return (CopyPathDelimiter)Preferences.Get(nameof(CopyPathDelimiter), (int)CopyPathDelimiter.NewLine); }
            set
            {
                if (CopyPathDelimiter != value)
                {
                    Preferences.Set(nameof(CopyPathDelimiter), (int)value);
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Navigation Bar

        public bool ShowBack
        {
            get { return Preferences.Get(nameof(ShowBack), true); }
            set
            {
                if (ShowBack != value)
                {
                    Preferences.Set(nameof(ShowBack), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowForward
        {
            get { return Preferences.Get(nameof(ShowForward), true); }
            set
            {
                if (ShowForward != value)
                {
                    Preferences.Set(nameof(ShowForward), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowUp
        {
            get { return Preferences.Get(nameof(ShowUp), false); }
            set
            {
                if (ShowUp != value)
                {
                    Preferences.Set(nameof(ShowUp), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowRefresh
        {
            get { return Preferences.Get(nameof(ShowRefresh), true); }
            set
            {
                if (ShowRefresh != value)
                {
                    Preferences.Set(nameof(ShowRefresh), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowDashboard
        {
            get { return Preferences.Get(nameof(ShowDashboard), true); }
            set
            {
                if (ShowDashboard != value)
                {
                    Preferences.Set(nameof(ShowDashboard), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowNewItem
        {
            get { return Preferences.Get(nameof(ShowNewItem), false); }
            set
            {
                if (ShowNewItem != value)
                {
                    Preferences.Set(nameof(ShowNewItem), value);
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region Browser

        public bool ShowCheckbox
        {
            get { return Preferences.Get(nameof(ShowCheckbox), false); }
            set
            {
                if (ShowCheckbox != value)
                {
                    Preferences.Set(nameof(ShowCheckbox), value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool ShowFileExtension
        {
            get { return Preferences.Get(nameof(ShowFileExtension), true); }
            set
            {
                if (ShowFileExtension != value)
                {
                    Preferences.Set(nameof(ShowFileExtension), value);
                    RaisePropertyChanged();
                }
            }
        }
        
        public bool ShowHiddenItems
        {
            get { return Preferences.Get(nameof(ShowHiddenItems), false); }
            set
            {
                if (ShowHiddenItems != value)
                {
                    Preferences.Set(nameof(ShowHiddenItems), value);
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region System

        private bool _runAtStartup;

        public bool RunAtStartup
        {
            get { return _runAtStartup; }
            set { Set(ref _runAtStartup, value); }
        }

        private StartupStatus _startupStatus;

        public StartupStatus StartupStatus
        {
            get { return _startupStatus; }
            set { Set(ref _startupStatus, value); }
        }

        public async Task RefreshRunAtStartup()
        {
#if UWP
            await refreshStartupSemaphore.WaitAsync();
            try
            {
                var startupTask = await StartupTask.GetAsync(STARTUP_TASK_ID);
                StartupStatus = (StartupStatus)startupTask.State;
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                    case StartupTaskState.DisabledByUser:
                    case StartupTaskState.DisabledByPolicy:
                        RunAtStartup = false;
                        break;
                    case StartupTaskState.Enabled:
                    case StartupTaskState.EnabledByPolicy:
                        RunAtStartup = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                refreshStartupSemaphore.Release();
            }
#endif
        }

        public async Task EnableRunAtStartup()
        {
#if UWP
            await changeStartupSemaphore.WaitAsync();
            try
            {
                var startupTask = await StartupTask.GetAsync(STARTUP_TASK_ID);
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        await startupTask.RequestEnableAsync();
                        break;

                    case StartupTaskState.DisabledByUser:
                        await dialogService.ShowMessage("You have disabled this app's startup in Settings. You can enable it in the Apps > Startup page.", "Startup Disabled", "OK");
                        break;

                    case StartupTaskState.DisabledByPolicy:
                        await dialogService.ShowMessage("Startup disabled by group policy, or not supported on this device", "Startup Disabled", "OK");
                        break;

                    case StartupTaskState.Enabled:
                    case StartupTaskState.EnabledByPolicy:
                    default:
                        break;
                }
                await RefreshRunAtStartup();
            }
            catch (Exception)
            {
            }
            finally
            {
                changeStartupSemaphore.Release();
            }
#endif
        }

        public async Task DisableRunAtStartup()
        {
#if UWP
            await changeStartupSemaphore.WaitAsync();
            try
            {
                var startupTask = await StartupTask.GetAsync(STARTUP_TASK_ID);
                if (startupTask.State == StartupTaskState.Enabled)
                {
                    startupTask.Disable();
                }
                await RefreshRunAtStartup();
            }
            catch (Exception)
            {
            }
            finally
            {
                changeStartupSemaphore.Release();
            }
#endif
        }

        public bool CanChangeStartup(StartupStatus status)
        {
            switch (status)
            {
                case StartupStatus.Disabled:
                case StartupStatus.Enabled:
                    return true;

                case StartupStatus.DisabledByUser:
                case StartupStatus.DisabledByPolicy:
                case StartupStatus.EnabledByPolicy:
                default:
                    return false;
            }
        }

        public string StartupDescription(StartupStatus status)
        {
            switch (status)
            {
                case StartupStatus.DisabledByUser:
                    return "You have disabled this app's startup in Settings. You can enable it in the Apps > Startup page.";

                case StartupStatus.DisabledByPolicy:
                case StartupStatus.EnabledByPolicy:
                    return "App startup is controlled by the administrator or group policy.";

                case StartupStatus.Enabled:
                case StartupStatus.Disabled:
                default:
                    return string.Empty;
            }
        }

        #endregion

    }
}

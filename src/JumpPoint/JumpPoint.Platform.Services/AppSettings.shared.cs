using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NittyGritty;
using NittyGritty.Platform.Theme;
using NittyGritty.Services;
using Xamarin.Essentials;
using AppTheme = NittyGritty.Platform.Theme.AppTheme;

namespace JumpPoint.Platform.Services
{
    public class AppSettings : ObservableObject
    {
        private static readonly Lazy<AppSettings> lazyInstance = new Lazy<AppSettings>(() => new AppSettings());
        private readonly ThemeService themeService;

        private AppSettings()
        {
            themeService = new ThemeService();
            Themes = new ReadOnlyCollection<AppTheme>(new List<AppTheme>
            {
                AppTheme.Default,
                AppTheme.Light,
                AppTheme.Dark
            });
            Layouts = new ReadOnlyCollection<string>(new List<string>
            {
                LayoutModes.Grid,
                LayoutModes.Details,
                LayoutModes.Tiles,
                LayoutModes.List
            });
        }

        public static AppSettings Instance => lazyInstance.Value;

        #region Appearance
        
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

        #region Layout

        public ReadOnlyCollection<string> Layouts { get; }

        public string Layout
        {
            get { return Preferences.Get(nameof(Layout), LayoutModes.Grid); }
            set
            {
                if (Layout != value)
                {
                    Preferences.Set(nameof(Layout), value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGridLayout));
                    RaisePropertyChanged(nameof(IsDetailsLayout));
                    RaisePropertyChanged(nameof(IsTilesLayout));
                    RaisePropertyChanged(nameof(IsListLayout));
                }
            }
        }

        public bool IsGridLayout
        {
            get { return Layout == LayoutModes.Grid; }
            set
            {
                if (Layout != LayoutModes.Grid && value)
                {
                    Layout = LayoutModes.Grid;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDetailsLayout
        {
            get { return Layout == LayoutModes.Details; }
            set
            {
                if (Layout != LayoutModes.Details && value)
                {
                    Layout = LayoutModes.Details;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsTilesLayout
        {
            get { return Layout == LayoutModes.Tiles; }
            set
            {
                if (Layout != LayoutModes.Tiles && value)
                {
                    Layout = LayoutModes.Tiles;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsListLayout
        {
            get { return Layout == LayoutModes.List; }
            set
            {
                if (Layout != LayoutModes.List && value)
                {
                    Layout = LayoutModes.List;
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

        public bool ShowLayout
        {
            get { return Preferences.Get(nameof(ShowLayout), true); }
            set
            {
                if (ShowLayout != value)
                {
                    Preferences.Set(nameof(ShowLayout), value);
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
        
        #region SortBy

        public SortBy SortBy
        {
            get { return (SortBy)Preferences.Get(nameof(SortBy), (int)SortBy.Name); }
            set
            {
                if (SortBy != value)
                {
                    Preferences.Set(nameof(SortBy), (int)value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortByName));
                    RaisePropertyChanged(nameof(IsSortByDateModified));
                    RaisePropertyChanged(nameof(IsSortByDisplayType));
                    RaisePropertyChanged(nameof(IsSortBySize));
                }
            }
        }

        public bool IsSortByName
        {
            get { return SortBy == SortBy.Name; }
            set
            {
                if (SortBy != SortBy.Name && value)
                {
                    SortBy = SortBy.Name;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortByDateModified
        {
            get { return SortBy == SortBy.DateModified; }
            set
            {
                if (SortBy != SortBy.DateModified && value)
                {
                    SortBy = SortBy.DateModified;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortByDisplayType
        {
            get { return SortBy == SortBy.DisplayType; }
            set
            {
                if (SortBy != SortBy.DisplayType && value)
                {
                    SortBy = SortBy.DisplayType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortBySize
        {
            get { return SortBy == SortBy.Size; }
            set
            {
                if (SortBy != SortBy.Size && value)
                {
                    SortBy = SortBy.Size;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortAscending
        {
            get { return Preferences.Get(nameof(IsSortAscending), true); }
            set
            {
                if (IsSortAscending != value)
                {
                    Preferences.Set(nameof(IsSortAscending), value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortDescending));
                }
            }
        }

        public bool IsSortDescending
        {
            get { return !IsSortAscending; }
            set
            {
                if (IsSortDescending != value)
                {
                    IsSortAscending = !value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortAscending));
                }
            }
        }

        #endregion

        #region GroupBy

        public GroupBy GroupBy
        {
            get { return (GroupBy)Preferences.Get(nameof(GroupBy), (int)GroupBy.None); }
            set
            {
                if (GroupBy != value)
                {
                    Preferences.Set(nameof(GroupBy), (int)value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupByNone));
                    RaisePropertyChanged(nameof(IsGroupByName));
                    RaisePropertyChanged(nameof(IsGroupByDateModified));
                    RaisePropertyChanged(nameof(IsGroupByDisplayType));
                    RaisePropertyChanged(nameof(IsGroupBySize));
                    RaisePropertyChanged(nameof(IsGroupByItemType));
                }
            }
        }

        public bool IsGroupByNone
        {
            get { return GroupBy == GroupBy.None; }
            set
            {
                if (GroupBy != GroupBy.None && value)
                {
                    GroupBy = GroupBy.None;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByName
        {
            get { return GroupBy == GroupBy.Name; }
            set
            {
                if (GroupBy != GroupBy.Name && value)
                {
                    GroupBy = GroupBy.Name;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByDateModified
        {
            get { return GroupBy == GroupBy.DateModified; }
            set
            {
                if (GroupBy != GroupBy.DateModified && value)
                {
                    GroupBy = GroupBy.DateModified;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByDisplayType
        {
            get { return GroupBy == GroupBy.DisplayType; }
            set
            {
                if (GroupBy != GroupBy.DisplayType && value)
                {
                    GroupBy = GroupBy.DisplayType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupBySize
        {
            get { return GroupBy == GroupBy.Size; }
            set
            {
                if (GroupBy != GroupBy.Size && value)
                {
                    GroupBy = GroupBy.Size;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByItemType
        {
            get { return GroupBy == GroupBy.ItemType; }
            set
            {
                if (GroupBy != GroupBy.ItemType && value)
                {
                    GroupBy = GroupBy.ItemType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupAscending
        {
            get { return Preferences.Get(nameof(IsGroupAscending), true); }
            set
            {
                if (IsGroupAscending != value)
                {
                    Preferences.Set(nameof(IsGroupAscending), value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupDescending));
                }
            }
        }

        public bool IsGroupDescending
        {
            get { return !IsGroupAscending; }
            set
            {
                if (IsGroupDescending != value)
                {
                    IsGroupAscending = !value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupAscending));
                }
            }
        }

        #endregion

        #endregion

    }
}

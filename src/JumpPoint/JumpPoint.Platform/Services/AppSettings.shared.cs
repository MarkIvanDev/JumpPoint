using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Models;
using NittyGritty;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services
{
    public class AppSettings : ObservableObject
    {
        private static readonly Lazy<AppSettings> lazyInstance = new Lazy<AppSettings>(() => new AppSettings());

        private AppSettings()
        {
            Layouts = new ReadOnlyCollection<string>(new List<string>
            {
                LayoutModes.Grid,
                LayoutModes.Details,
                LayoutModes.Tiles
            });
        }

        public static AppSettings Instance => lazyInstance.Value;

        #region Appearance

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

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class DrmProperties : ObservableObject
    {

        private DateTimeOffset? _datePlayStarts;

        public DateTimeOffset? DatePlayStarts
        {
            get { return _datePlayStarts; }
            set { Set(ref _datePlayStarts, value); }
        }

        private DateTimeOffset? _datePlayExpires;

        public DateTimeOffset? DatePlayExpires
        {
            get { return _datePlayExpires; }
            set { Set(ref _datePlayExpires, value); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private bool? _isDisabled;

        public bool? IsDisabled
        {
            get { return _isDisabled; }
            set { Set(ref _isDisabled, value); }
        }

        private bool? _isProtected;

        public bool? IsProtected
        {
            get { return _isProtected; }
            set { Set(ref _isProtected, value); }
        }

        private uint? _playCount;

        public uint? PlayCount
        {
            get { return _playCount; }
            set { Set(ref _playCount, value); }
        }

        public static DrmProperties Extract(IDictionary<string, object> props)
        {
            var drmProperties = new DrmProperties()
            {
                DatePlayStarts = props[Key.DatePlayStarts] as DateTimeOffset?,
                DatePlayExpires = props[Key.DatePlayExpires] as DateTimeOffset?,
                Description = (string)props[Key.Description],
                IsDisabled = props[Key.IsDisabled] as bool?,
                IsProtected = props[Key.IsProtected] as bool?,
                PlayCount = props[Key.PlayCount] as uint?
            };
            return drmProperties;
        }

        public static class Key
        {
            public static string DatePlayStarts => "System.DRM.DatePlayStarts";
            public static string DatePlayExpires => "System.DRM.DatePlayExpires";
            public static string Description => "System.DRM.Description";
            public static string IsDisabled => "System.DRM.IsDisabled";
            public static string IsProtected => "System.DRM.IsProtected";
            public static string PlayCount => "System.DRM.PlayCount";

            public static IEnumerable<string> All()
            {
                yield return DatePlayStarts;
                yield return DatePlayExpires;
                yield return Description;
                yield return IsDisabled;
                yield return IsProtected;
                yield return PlayCount;
            }
        }
    }
}

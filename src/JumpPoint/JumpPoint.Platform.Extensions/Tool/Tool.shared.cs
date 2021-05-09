using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Extensions
{
    public class Tool : ExtensionBase
    {
        public Tool() : base(nameof(Tool))
        {
        }

        private string _link;

        public string Link
        {
            get { return _link; }
            set { Set(ref _link, value); }
        }

        private string _service;

        public string Service
        {
            get { return _service; }
            set { Set(ref _service, value); }
        }

        private string _group;

        public string Group
        {
            get { return _group ?? (_group = string.Empty); }
            set { Set(ref _group, value); }
        }

        private HashSet<string> _fileTypes;

        public HashSet<string> FileTypes
        {
            get { return _fileTypes ?? (_fileTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)); }
            set { Set(ref _fileTypes, value); }
        }

        public bool SupportsAnyFileType
        {
            get { return FileTypes.Contains("."); }
        }

        private bool _includeFileTokens;

        public bool IncludeFileTokens
        {
            get { return _includeFileTokens; }
            set { Set(ref _includeFileTokens, value); }
        }

        public bool IsSupported(IList<JumpPointItem> items)
        {
            var groups = items
                .GroupBy(i =>
                {
                    switch (i.Type)
                    {
                        case JumpPointItemType.File:
                            return JumpPointItemType.File;

                        // For now, we do not support these item types in tools
                        case JumpPointItemType.Drive:
                        case JumpPointItemType.Folder:
                        case JumpPointItemType.Workspace:
                        case JumpPointItemType.SettingLink:
                        case JumpPointItemType.AppLink:
                        case JumpPointItemType.Library:
                        case JumpPointItemType.Unknown:
                        default:
                            return JumpPointItemType.Unknown;
                    }
                })
                .ToDictionary(g => g.Key);

            // Check if there are items that the tool does not support
            if (groups.ContainsKey(JumpPointItemType.Unknown))
            {
                return false;
            }

            // Check tool's file support
            if (FileTypes.Count > 0 && groups.TryGetValue(JumpPointItemType.File, out var filesGroup))
            {
                var files = filesGroup.Cast<FileBase>().ToList();

                // Support for Cloud files is not yet supported
                if (files.Any(i => i.StorageType == StorageType.Cloud))
                {
                    return false;
                }

                if (!SupportsAnyFileType && files.Any(i => !FileTypes.Contains(i.FileType)))
                {
                    return false;
                }
            }

            return true;
        }

    }
}

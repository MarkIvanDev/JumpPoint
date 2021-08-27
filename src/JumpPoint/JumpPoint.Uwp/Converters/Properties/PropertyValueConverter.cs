using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class PropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is IList<JumpPointItem> group && parameter is string name)
            {
                switch (name)
                {
                    case "Name":
                        if(group.Count == 1)
                        {
                            return group[0].DisplayName;
                        }
                        else
                        {
                            return "(Multiple Values)";
                        }
                    case "Type":
                        if (group.Count == 1)
                        {
                            return group[0].DisplayType;
                        }
                        else
                        {
                            return "(Multiple Values)";
                        }
                    case "Path":
                        if (group.Count == 1)
                        {
                            return group[0].Path;
                        }
                        else
                        {
                            return "(Multiple Values)";
                        }
                    case "Date Created":
                        if(group.Any(i => i.Type == JumpPointItemType.Folder || i.Type == JumpPointItemType.File))
                        {
                            var dc = group.OfType<StorageItemBase>().ToList();
                            if (dc.Count == 1)
                            {

                                return dc.First().DateCreated?.LocalDateTime.ToString();
                            }
                            else
                            {
                                return $"{dc.Min(i => i.DateCreated)?.LocalDateTime.ToString()} - {dc.Max(i => i.DateCreated)?.LocalDateTime.ToString()}";
                            }
                        }
                        else if(group.Any(i => i.Type == JumpPointItemType.Workspace))
                        {
                            var dc = group.OfType<Workspace>().ToList();
                            if(dc.Count == 1)
                            {
                                return dc.First().DateCreated.LocalDateTime.ToString();
                            }
                            else
                            {
                                return $"{dc.Min(i => i.DateCreated).LocalDateTime.ToString()} - {dc.Max(i => i.DateCreated).LocalDateTime.ToString()}";
                            }
                        }
                        break;
                    case "Date Modified":
                        var dm = group.OfType<StorageItemBase>().ToList();
                        if (dm.Count == 1)
                        {
                            return dm.First().DateModified?.LocalDateTime.ToString();
                        }
                        else
                        {
                            return $"{dm.Min(i => i.DateCreated)?.LocalDateTime.ToString()} - {dm.Max(i => i.DateCreated)?.LocalDateTime.ToString()}";
                        }
                    case "Date Accessed":
                        var da = group.OfType<StorageItemBase>().ToList();
                        if (da.Count == 1)
                        {
                            return da.First().DateAccessed?.LocalDateTime.ToString();
                        }
                        else
                        {
                            return $"{da.Min(i => i.DateCreated)?.LocalDateTime.ToString()} - {da.Max(i => i.DateCreated)?.LocalDateTime.ToString()}";
                        }
                    case "File Count":
                        if(group.Any(i => i.Type == JumpPointItemType.Folder))
                        {
                            var fc = group.OfType<FolderBase>().ToList();
                            ulong count = 0;
                            fc.ForEach(f => count += f.FileCount);
                            return count.ToString();
                        }
                        else if(group.Any(i => i.Type == JumpPointItemType.Workspace))
                        {
                            var fc = group.OfType<Workspace>().ToList();
                            ulong count = 0;
                            fc.ForEach(f => count += f.FileCount);
                            return count.ToString();
                        }
                        break;
                    case "Folder Count":
                        if (group.Any(i => i.Type == JumpPointItemType.Folder))
                        {
                            var fc = group.OfType<FolderBase>().ToList();
                            ulong count = 0;
                            fc.ForEach(f => count += f.FolderCount);
                            return count.ToString();
                        }
                        else if (group.Any(i => i.Type == JumpPointItemType.Workspace))
                        {
                            var fc = group.OfType<Workspace>().ToList();
                            ulong count = 0;
                            fc.ForEach(f => count += f.FolderCount);
                            return count.ToString();
                        }
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

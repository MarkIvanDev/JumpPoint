using System;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models.Favorite
{
    public class FavoriteSettingLink : ObservableObject
    {
        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private SettingLinkTemplate _template;

        [NotNull, Unique, Collation("NOCASE")]
        public SettingLinkTemplate Template
        {
            get { return _template; }
            set { Set(ref _template, value); }
        }
    }
}

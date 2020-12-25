using System;
using System.Collections.Generic;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class MusicProperties : ObservableObject
    {
        
        private string _albumArtist;

        public string AlbumArtist
        {
            get { return _albumArtist; }
            set { Set(ref _albumArtist, value); }
        }

        private string _album;

        public string Album
        {
            get { return _album; }
            set { Set(ref _album, value); }
        }

        private IList<string> _artist;

        public IList<string> Artists
        {
            get { return _artist; }
            set { Set(ref _artist, value); }
        }

        private IList<string> _composers;

        public IList<string> Composers
        {
            get { return _composers; }
            set { Set(ref _composers, value); }
        }

        private IList<string> _conductors;

        public IList<string> Conductors
        {
            get { return _conductors; }
            set { Set(ref _conductors, value); }
        }

        private uint? _discNumber;

        public uint? DiscNumber
        {
            get { return _discNumber; }
            set { Set(ref _discNumber, value); }
        }

        private uint? _trackNumber;

        public uint? TrackNumber
        {
            get { return _trackNumber; }
            set { Set(ref _trackNumber, value); }
        }

        private string _beatsPerMinute;

        public string BeatsPerMinute
        {
            get { return _beatsPerMinute; }
            set { Set(ref _beatsPerMinute, value); }
        }

        private IList<string> _genre;

        public IList<string> Genre
        {
            get { return _genre; }
            set { Set(ref _genre, value); }
        }

        private string _initialKey;

        public string InitialKey
        {
            get { return _initialKey; }
            set { Set(ref _initialKey, value); }
        }

        private string _mood;

        public string Mood
        {
            get { return _mood; }
            set { Set(ref _mood, value); }
        }

        private string _period;

        public string Period
        {
            get { return _period; }
            set { Set(ref _period, value); }
        }

        public static MusicProperties Extract(IDictionary<string, object> props)
        {
            var musicProperties = new MusicProperties()
            {
                AlbumArtist = (string)props[Key.AlbumArtist],
                Album = (string)props[Key.Album],
                Artists = props[Key.Artists] as IList<string> ?? new List<string>(),
                Composers = props[Key.Composers] as IList<string> ?? new List<string>(),
                Conductors = props[Key.Conductors] as IList<string> ?? new List<string>(),
                DiscNumber = props[Key.DiscNumber] as uint?,
                TrackNumber = props[Key.TrackNumber] as uint?,

                BeatsPerMinute = (string)props[Key.BeatsPerMinute],
                Genre = props[Key.Genre] as IList<string> ?? new List<string>(),
                InitialKey = (string)props[Key.InitialKey],
                Mood = (string)props[Key.Mood],
                Period = (string)props[Key.Period]
            };
            return musicProperties;
        }

        public static class Key
        {
            public static string AlbumArtist => "System.Music.AlbumArtist";
            public static string Album => "System.Music.AlbumTitle";
            public static string Artists => "System.Music.Artist";
            public static string Composers => "System.Music.Composer";
            public static string Conductors => "System.Music.Conductor";
            public static string DiscNumber => "System.Music.DiscNumber";
            public static string TrackNumber => "System.Music.TrackNumber";

            public static string BeatsPerMinute => "System.Music.BeatsPerMinute";
            public static string Genre => "System.Music.Genre";
            public static string InitialKey => "System.Music.InitialKey";
            public static string Mood => "System.Music.Mood";
            public static string Period => "System.Music.Period";

            public static IEnumerable<string> All()
            {
                yield return AlbumArtist;
                yield return Album;
                yield return Artists;
                yield return Composers;
                yield return Conductors;
                yield return DiscNumber;
                yield return TrackNumber;
                
                yield return BeatsPerMinute;
                yield return Genre;
                yield return InitialKey;
                yield return Mood;
                yield return Period;
            }
        }
    }
}
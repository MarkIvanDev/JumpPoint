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

        private string[] _artists;

        public string[] Artists
        {
            get { return _artists; }
            set { Set(ref _artists, value); }
        }

        private string[] _composers;

        public string[] Composers
        {
            get { return _composers; }
            set { Set(ref _composers, value); }
        }

        private string[] _conductors;

        public string[] Conductors
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

        private bool? _isCompilation;

        public bool? IsCompilation
        {
            get { return _isCompilation; }
            set { Set(ref _isCompilation, value); }
        }

        private string _partOfSet;

        public string PartOfSet
        {
            get { return _partOfSet; }
            set { Set(ref _partOfSet, value); }
        }

        private string _beatsPerMinute;

        public string BeatsPerMinute
        {
            get { return _beatsPerMinute; }
            set { Set(ref _beatsPerMinute, value); }
        }

        private string[] _genre;

        public string[] Genre
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

        private string _lyrics;

        public string Lyrics
        {
            get { return _lyrics; }
            set { Set(ref _lyrics, value); }
        }

        private byte[] _synchronizedLyrics;

        public byte[] SynchronizedLyrics
        {
            get { return _synchronizedLyrics; }
            set { Set(ref _synchronizedLyrics, value); }
        }

        public static MusicProperties Extract(IDictionary<string, object> props)
        {
            var musicProperties = new MusicProperties()
            {
                AlbumArtist = (string)props[Key.AlbumArtist],
                Album = (string)props[Key.Album],
                Artists = props[Key.Artists] as string[] ?? new string[0],
                Composers = props[Key.Composers] as string[] ?? new string[0],
                Conductors = props[Key.Conductors] as string[] ?? new string[0],
                DiscNumber = props[Key.DiscNumber] as uint?,
                TrackNumber = props[Key.TrackNumber] as uint?,
                IsCompilation = props[Key.IsCompilation] as bool?,
                PartOfSet = (string)props[Key.PartOfSet],

                BeatsPerMinute = (string)props[Key.BeatsPerMinute],
                Genre = props[Key.Genre] as string[] ?? new string[0],
                InitialKey = (string)props[Key.InitialKey],
                Mood = (string)props[Key.Mood],
                Period = (string)props[Key.Period],

                Lyrics = (string)props[Key.Lyrics],
                SynchronizedLyrics = props[Key.SynchronizedLyrics] as byte[]
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
            public static string IsCompilation => "System.Music.IsCompilation";
            public static string PartOfSet => "System.Music.PartOfSet";

            public static string BeatsPerMinute => "System.Music.BeatsPerMinute";
            public static string Genre => "System.Music.Genre";
            public static string InitialKey => "System.Music.InitialKey";
            public static string Mood => "System.Music.Mood";
            public static string Period => "System.Music.Period";

            public static string Lyrics => "System.Music.Lyrics";
            public static string SynchronizedLyrics => "System.Music.SynchronizedLyrics";

            public static IEnumerable<string> All()
            {
                yield return AlbumArtist;
                yield return Album;
                yield return Artists;
                yield return Composers;
                yield return Conductors;
                yield return DiscNumber;
                yield return TrackNumber;
                yield return IsCompilation;
                yield return PartOfSet;
                
                yield return BeatsPerMinute;
                yield return Genre;
                yield return InitialKey;
                yield return Mood;
                yield return Period;

                yield return Lyrics;
                yield return SynchronizedLyrics;
            }
        }
    }
}
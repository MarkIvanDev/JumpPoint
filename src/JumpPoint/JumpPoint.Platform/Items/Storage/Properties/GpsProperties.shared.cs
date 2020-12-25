using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class GpsProperties : ObservableObject
    {

        private string _areaInformation;

        public string AreaInformation
        {
            get { return _areaInformation; }
            set { Set(ref _areaInformation, value); }
        }

        private DateTimeOffset? _date;

        public DateTimeOffset? Date
        {
            get { return _date; }
            set { Set(ref _date, value); }
        }

        private ushort? _differential;

        public ushort? Differential
        {
            get { return _differential; }
            set { Set(ref _differential, value); }
        }

        private string _versionID;

        public string VersionID
        {
            get { return _versionID; }
            set { Set(ref _versionID, value); }
        }

        private double? _altitude;

        public double? Altitude
        {
            get { return _altitude; }
            set { Set(ref _altitude, value); }
        }

        private byte? _altitudeReference;

        public byte? AltitudeReference
        {
            get { return _altitudeReference; }
            set { Set(ref _altitudeReference, value); }
        }

        private IList<double> _latitude;

        public IList<double> Latitude
        {
            get { return _latitude; }
            set { Set(ref _latitude, value); }
        }

        private double? _latitudeDecimal;

        public double? LatitudeDecimal
        {
            get { return _latitudeDecimal; }
            set { Set(ref _latitudeDecimal, value); }
        }

        private string _latitudeReference;

        public string LatitudeReference
        {
            get { return _latitudeReference; }
            set { Set(ref _latitudeReference, value); }
        }

        private IList<double> _longitude;

        public IList<double> Longitude
        {
            get { return _longitude; }
            set { Set(ref _longitude, value); }
        }

        private double? _longitudeDecimal;

        public double? LongitudeDecimal
        {
            get { return _longitudeDecimal; }
            set { Set(ref _longitudeDecimal, value); }
        }

        private string _longitudeReference;

        public string LongitudeReference
        {
            get { return _longitudeReference; }
            set { Set(ref _longitudeReference, value); }
        }

        private double? _track;

        public double? Track
        {
            get { return _track; }
            set { Set(ref _track, value); }
        }

        private string _trackReference;

        public string TrackReference
        {
            get { return _trackReference; }
            set { Set(ref _trackReference, value); }
        }

        public static GpsProperties Extract(IDictionary<string, object> props)
        {
            var gpsProperties = new GpsProperties()
            {
                AreaInformation = (string)props[Key.AreaInformation],
                Date = props[Key.Date] as DateTimeOffset?,
                Differential = props[Key.Differential] as ushort?,
                VersionID = (string)props[Key.VersionID],

                Altitude = props[Key.Altitude] as double?,
                AltitudeReference = props[Key.AltitudeReference] as byte?,
                Latitude = props[Key.Latitude] as IList<double> ?? new List<double>(),
                LatitudeDecimal = props[Key.LatitudeDecimal] as double?,
                LatitudeReference = (string)props[Key.LatitudeReference],
                Longitude = props[Key.Longitude] as IList<double> ?? new List<double>(),
                LongitudeDecimal = props[Key.LongitudeDecimal] as double?,
                LongitudeReference = (string)props[Key.LongitudeReference],
                Track = props[Key.Track] as double?,
                TrackReference = (string)props[Key.TrackReference]
            };
            return gpsProperties;
        }

        public static class Key
        {
            public static string AreaInformation => "System.GPS.AreaInformation";
            public static string Date => "System.GPS.Date";
            public static string Differential => "System.GPS.Differential";
            public static string VersionID => "System.GPS.VersionID";

            public static string Altitude => "System.GPS.Altitude";
            public static string AltitudeReference => "System.GPS.AltitudeRef";
            public static string Latitude => "System.GPS.Latitude";
            public static string LatitudeDecimal => "System.GPS.LatitudeDecimal";
            public static string LatitudeReference => "System.GPS.LatitudeRef";
            public static string Longitude => "System.GPS.Longitude";
            public static string LongitudeDecimal => "System.GPS.LongitudeDecimal";
            public static string LongitudeReference => "System.GPS.LongitudeRef";
            public static string Track => "System.GPS.Track";
            public static string TrackReference => "System.GPS.TrackRef";
            
            public static IEnumerable<string> All()
            {
                yield return AreaInformation;
                yield return Date;
                yield return Differential;
                yield return VersionID;
                
                yield return Altitude;
                yield return AltitudeReference;
                yield return Latitude;
                yield return LatitudeDecimal;
                yield return LatitudeReference;
                yield return Longitude;
                yield return LongitudeDecimal;
                yield return LongitudeReference;
                yield return Track;
                yield return TrackReference;
            }
        }
    }
}

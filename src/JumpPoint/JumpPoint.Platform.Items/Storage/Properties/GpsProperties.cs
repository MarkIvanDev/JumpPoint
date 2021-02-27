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

        private double? _degreeOfPrecision;

        public double? DegreeOfPrecision
        {
            get { return _degreeOfPrecision; }
            set { Set(ref _degreeOfPrecision, value); }
        }

        private string _measureMode;

        public string MeasureMode
        {
            get { return _measureMode; }
            set { Set(ref _measureMode, value); }
        }

        private string _processingMethod;

        public string ProcessingMethod
        {
            get { return _processingMethod; }
            set { Set(ref _processingMethod, value); }
        }

        private string _satellites;

        public string Satellites
        {
            get { return _satellites; }
            set { Set(ref _satellites, value); }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private string _mapDatum;

        public string MapDatum
        {
            get { return _mapDatum; }
            set { Set(ref _mapDatum, value); }
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

        private double[] _latitude;

        public double[] Latitude
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

        private double[] _longitude;

        public double[] Longitude
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

        private double? _imageDirection;

        public double? ImageDirection
        {
            get { return _imageDirection; }
            set { Set(ref _imageDirection, value); }
        }

        private string _imageDirectionReference;

        public string ImageDirectionReference
        {
            get { return _imageDirectionReference; }
            set { Set(ref _imageDirectionReference, value); }
        }

        private double? _speed;

        public double? Speed
        {
            get { return _speed; }
            set { Set(ref _speed, value); }
        }

        private string _speedReference;

        public string SpeedReference
        {
            get { return _speedReference; }
            set { Set(ref _speedReference, value); }
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

        private double[] _destinationLatitude;

        public double[] DestinationLatitude
        {
            get { return _destinationLatitude; }
            set { Set(ref _destinationLatitude, value); }
        }

        private string _destinationLatitudeReference;

        public string DestinationLatitudeReference
        {
            get { return _destinationLatitudeReference; }
            set { Set(ref _destinationLatitudeReference, value); }
        }

        private double[] _destinationLongitude;

        public double[] DestinationLongitude
        {
            get { return _destinationLongitude; }
            set { Set(ref _destinationLongitude, value); }
        }

        private string _destinationLongitudeReference;

        public string DestinationLongitudeReference
        {
            get { return _destinationLongitudeReference; }
            set { Set(ref _destinationLongitudeReference, value); }
        }

        private double? _destinationBearing;

        public double? DestinationBearing
        {
            get { return _destinationBearing; }
            set { Set(ref _destinationBearing, value); }
        }

        private string _destinationBearingReference;

        public string DestinationBearingReference
        {
            get { return _destinationBearingReference; }
            set { Set(ref _destinationBearingReference, value); }
        }

        private double? _destinationDistance;

        public double? DestinationDistance
        {
            get { return _destinationDistance; }
            set { Set(ref _destinationDistance, value); }
        }

        private string _destinationDistanceReference;

        public string DestinationDistanceReference
        {
            get { return _destinationDistanceReference; }
            set { Set(ref _destinationDistanceReference, value); }
        }

        public static GpsProperties Extract(IDictionary<string, object> props)
        {
            var gpsProperties = new GpsProperties()
            {
                AreaInformation = (string)props[Key.AreaInformation],
                Date = props[Key.Date] as DateTimeOffset?,
                Differential = props[Key.Differential] as ushort?,
                DegreeOfPrecision = props[Key.DegreeOfPrecision] as double?,
                MeasureMode = (string)props[Key.MeasureMode],
                ProcessingMethod = (string)props[Key.ProcessingMethod],
                Satellites = (string)props[Key.Satellites],
                Status = (string)props[Key.Status],
                MapDatum = (string)props[Key.MapDatum],
                VersionID = (string)props[Key.VersionId],

                Altitude = props[Key.Altitude] as double?,
                AltitudeReference = props[Key.AltitudeReference] as byte?,
                Latitude = props[Key.Latitude] as double[],
                LatitudeDecimal = props[Key.LatitudeDecimal] as double?,
                LatitudeReference = (string)props[Key.LatitudeReference],
                Longitude = props[Key.Longitude] as double[],
                LongitudeDecimal = props[Key.LongitudeDecimal] as double?,
                LongitudeReference = (string)props[Key.LongitudeReference],
                ImageDirection = props[Key.ImageDirection] as double?,
                ImageDirectionReference = (string)props[Key.ImageDirectionReference],
                Speed = props[Key.Speed] as double?,
                SpeedReference = (string)props[Key.SpeedReference],
                Track = props[Key.Track] as double?,
                TrackReference = (string)props[Key.TrackReference],
                DestinationLatitude = props[Key.DestinationLatitude] as double[],
                DestinationLatitudeReference = (string)props[Key.DestinationLatitudeReference],
                DestinationLongitude = props[Key.DestinationLongitude] as double[],
                DestinationLongitudeReference = (string)props[Key.DestinationLongitudeReference],
                DestinationBearing = props[Key.DestinationBearing] as double?,
                DestinationBearingReference = (string)props[Key.DestinationBearingReference],
                DestinationDistance = props[Key.DestinationDistance] as double?,
                DestinationDistanceReference = (string)props[Key.DestinationDistanceReference],
            };
            return gpsProperties;
        }

        public static class Key
        {
            public static string AreaInformation => "System.GPS.AreaInformation";
            public static string Date => "System.GPS.Date";
            public static string Differential => "System.GPS.Differential";
            public static string DegreeOfPrecision => "System.GPS.DOP";
            public static string MeasureMode => "System.GPS.MeasureMode";
            public static string ProcessingMethod => "System.GPS.ProcessingMethod";
            public static string Satellites => "System.GPS.Satellites";
            public static string Status => "System.GPS.Status";
            public static string MapDatum => "System.GPS.MapDatum";
            public static string VersionId => "System.GPS.VersionID";

            public static string Altitude => "System.GPS.Altitude";
            public static string AltitudeReference => "System.GPS.AltitudeRef";
            public static string Latitude => "System.GPS.Latitude";
            public static string LatitudeDecimal => "System.GPS.LatitudeDecimal";
            public static string LatitudeReference => "System.GPS.LatitudeRef";
            public static string Longitude => "System.GPS.Longitude";
            public static string LongitudeDecimal => "System.GPS.LongitudeDecimal";
            public static string LongitudeReference => "System.GPS.LongitudeRef";
            public static string ImageDirection => "System.GPS.ImgDirection";
            public static string ImageDirectionReference => "System.GPS.ImgDirectionRef";
            public static string Speed => "System.GPS.Speed";
            public static string SpeedReference => "System.GPS.SpeedRef";
            public static string Track => "System.GPS.Track";
            public static string TrackReference => "System.GPS.TrackRef";
            public static string DestinationLatitude => "System.GPS.DestLatitude";
            public static string DestinationLatitudeReference => "System.GPS.DestLatitudeRef";
            public static string DestinationLongitude => "System.GPS.DestLongitude";
            public static string DestinationLongitudeReference => "System.GPS.DestLongitudeRef";
            public static string DestinationBearing => "System.GPS.DestBearing";
            public static string DestinationBearingReference => "System.GPS.DestBearingRef";
            public static string DestinationDistance => "System.GPS.DestDistance";
            public static string DestinationDistanceReference => "System.GPS.DestDistanceRef";

            
            public static IEnumerable<string> All()
            {
                yield return AreaInformation;
                yield return Date;
                yield return Differential;
                yield return DegreeOfPrecision;
                yield return MeasureMode;
                yield return ProcessingMethod;
                yield return Satellites;
                yield return Status;
                yield return MapDatum;
                yield return VersionId;
                
                yield return Altitude;
                yield return AltitudeReference;
                yield return Latitude;
                yield return LatitudeDecimal;
                yield return LatitudeReference;
                yield return Longitude;
                yield return LongitudeDecimal;
                yield return LongitudeReference;
                yield return ImageDirection;
                yield return ImageDirectionReference;
                yield return Speed;
                yield return SpeedReference;
                yield return Track;
                yield return TrackReference;
                yield return DestinationLatitude;
                yield return DestinationLatitudeReference;
                yield return DestinationLongitude;
                yield return DestinationLongitudeReference;
                yield return DestinationBearing;
                yield return DestinationBearingReference;
                yield return DestinationDistance;
                yield return DestinationDistanceReference;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Storage.Properties.Photo;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class PhotoProperties : ObservableObject
    {

        private DateTimeOffset? _dateTaken;

        public DateTimeOffset? DateTaken
        {
            get { return _dateTaken; }
            set { Set(ref _dateTaken, value); }
        }

        private IList<string> _event;

        public IList<string> Event
        {
            get { return _event; }
            set { Set(ref _event, value); }
        }

        private string _exifVersion;

        public string EXIFVersion
        {
            get { return _exifVersion; }
            set { Set(ref _exifVersion, value); }
        }

        private Orientation? _orientation;

        public Orientation? Orientation
        {
            get { return _orientation; }
            set { Set(ref _orientation, value); }
        }

        private IList<string> _peopleNames;

        public IList<string> PeopleNames
        {
            get { return _peopleNames; }
            set { Set(ref _peopleNames, value); }
        }

        private double? _aperture;

        public double? Aperture
        {
            get { return _aperture; }
            set { Set(ref _aperture, value); }
        }

        private double? _brightness;

        public double? Brightness
        {
            get { return _brightness; }
            set { Set(ref _brightness, value); }
        }

        private Contrast? _contrast;

        public Contrast? Contrast
        {
            get { return _contrast; }
            set { Set(ref _contrast, value); }
        }

        private double? _digitalZoom;

        public double? DigitalZoom
        {
            get { return _digitalZoom; }
            set { Set(ref _digitalZoom, value); }
        }

        private double? _exposureBias;

        public double? ExposureBias
        {
            get { return _exposureBias; }
            set { Set(ref _exposureBias, value); }
        }

        private double? _exposureIndex;

        public double? ExposureIndex
        {
            get { return _exposureIndex; }
            set { Set(ref _exposureIndex, value); }
        }

        private double? _exposureTime;

        public double? ExposureTime
        {
            get { return _exposureTime; }
            set { Set(ref _exposureTime, value); }
        }

        private Flash? _flash;

        public Flash? Flash
        {
            get { return _flash; }
            set { Set(ref _flash, value); }
        }

        private double? _fNumber;

        public double? FNumber
        {
            get { return _fNumber; }
            set { Set(ref _fNumber, value); }
        }

        private double? _focalLength;

        public double? FocalLength
        {
            get { return _focalLength; }
            set { Set(ref _focalLength, value); }
        }

        private ushort? _isoSpeed;

        public ushort? ISOSpeed
        {
            get { return _isoSpeed; }
            set { Set(ref _isoSpeed, value); }
        }

        private double? _maxAperture;

        public double? MaxAperture
        {
            get { return _maxAperture; }
            set { Set(ref _maxAperture, value); }
        }

        private Saturation? _saturation;

        public Saturation? Saturation
        {
            get { return _saturation; }
            set { Set(ref _saturation, value); }
        }

        private Sharpness? _sharpness;

        public Sharpness? Sharpness
        {
            get { return _sharpness; }
            set { Set(ref _sharpness, value); }
        }

        private double? _shutterSpeed;

        public double? ShutterSpeed
        {
            get { return _shutterSpeed; }
            set { Set(ref _shutterSpeed, value); }
        }

        private WhiteBalance? _whiteBalance;

        public WhiteBalance? WhiteBalance
        {
            get { return _whiteBalance; }
            set { Set(ref _whiteBalance, value); }
        }

        private string _cameraManufacturer;

        public string CameraManufacturer
        {
            get { return _cameraManufacturer; }
            set { Set(ref _cameraManufacturer, value); }
        }

        private string _cameraModel;

        public string CameraModel
        {
            get { return _cameraModel; }
            set { Set(ref _cameraModel, value); }
        }

        private string _cameraSerialNumber;

        public string CameraSerialNumber
        {
            get { return _cameraSerialNumber; }
            set { Set(ref _cameraSerialNumber, value); }
        }

        private string _flashManufacturer;

        public string FlashManufacturer
        {
            get { return _flashManufacturer; }
            set { Set(ref _flashManufacturer, value); }
        }

        private string _flashModel;

        public string FlashModel
        {
            get { return _flashModel; }
            set { Set(ref _flashModel, value); }
        }

        private string _lensManufacturer;

        public string LensManufacturer
        {
            get { return _lensManufacturer; }
            set { Set(ref _lensManufacturer, value); }
        }

        private string _lensModel;

        public string LensModel
        {
            get { return _lensModel; }
            set { Set(ref _lensModel, value); }
        }

        public static PhotoProperties Extract(IDictionary<string, object> props)
        {
            var photoProperties = new PhotoProperties()
            {
                DateTaken = props[Key.DateTaken] as DateTimeOffset?,
                Event = props[Key.Event] as IList<string> ?? new List<string>(),
                EXIFVersion = (string)props[Key.EXIFVersion],
                Orientation = props[Key.Orientation] is ushort o ? (Orientation?)o : null,
                PeopleNames = props[Key.PeopleNames] as IList<string> ?? new List<string>(),

                Aperture = props[Key.Aperture] as double?,
                Brightness = props[Key.Brightness] as double?,
                Contrast = props[Key.Contrast] is uint c ? (Contrast?)c : null,
                DigitalZoom = props[Key.DigitalZoom] as double?,
                ExposureBias = props[Key.ExposureBias] as double?,
                ExposureIndex = props[Key.ExposureIndex] as double?,
                ExposureTime = props[Key.ExposureTime] as double?,
                Flash = props[Key.Flash] is byte f ? (Flash?)f : null,
                FNumber = props[Key.FNumber] as double?,
                FocalLength = props[Key.FocalLength] as double?,
                ISOSpeed = props[Key.ISOSpeed] as ushort?,
                MaxAperture = props[Key.MaxAperture] as double?,
                Saturation = props[Key.Saturation] is uint sat ? (Saturation?)sat : null,
                Sharpness = props[Key.Sharpness] is uint sha ? (Sharpness?)sha : null,
                ShutterSpeed = props[Key.ShutterSpeed] as double?,
                WhiteBalance = props[Key.WhiteBalance] is uint wb ? (WhiteBalance?)wb : null,

                CameraManufacturer = (string)props[Key.CameraManufacturer],
                CameraModel = (string)props[Key.CameraModel],
                CameraSerialNumber = (string)props[Key.CameraSerialNumber],
                FlashManufacturer = (string)props[Key.FlashManufacturer],
                FlashModel = (string)props[Key.FlashModel],
                LensManufacturer = (string)props[Key.LensManufacturer],
                LensModel = (string)props[Key.LensModel]
            };
            return photoProperties;
        }

        public static class Key
        {
            public static string DateTaken => "System.Photo.DateTaken";
            public static string Event => "System.Photo.Event";
            public static string EXIFVersion => "System.Photo.EXIFVersion";
            public static string Orientation => "System.Photo.Orientation";
            public static string PeopleNames => "System.Photo.PeopleNames";

            public static string Aperture => "System.Photo.Aperture";
            public static string Brightness => "System.Photo.Brightness";
            public static string Contrast => "System.Photo.Contrast";
            public static string DigitalZoom => "System.Photo.DigitalZoom";
            public static string ExposureBias => "System.Photo.ExposureBias";
            public static string ExposureIndex => "System.Photo.ExposureIndex";
            public static string ExposureTime => "System.Photo.ExposureTime";
            public static string Flash => "System.Photo.Flash";
            public static string FNumber => "System.Photo.FNumber";
            public static string FocalLength => "System.Photo.FocalLength";
            public static string ISOSpeed => "System.Photo.ISOSpeed";
            public static string MaxAperture => "System.Photo.MaxAperture";
            public static string Saturation = "System.Photo.Saturation";
            public static string Sharpness => "System.Photo.Sharpness";
            public static string ShutterSpeed => "System.Photo.ShutterSpeed";
            public static string WhiteBalance => "System.Photo.WhiteBalance";

            public static string CameraManufacturer => "System.Photo.CameraManufacturer";
            public static string CameraModel => "System.Photo.CameraModel";
            public static string CameraSerialNumber => "System.Photo.CameraSerialNumber";
            public static string FlashManufacturer => "System.Photo.FlashManufacturer";
            public static string FlashModel => "System.Photo.FlashModel";
            public static string LensManufacturer => "System.Photo.LensManufacturer";
            public static string LensModel => "System.Photo.LensModel";            

            public static IEnumerable<string> All()
            {
                yield return DateTaken;
                yield return Event;
                yield return EXIFVersion;
                yield return Orientation;
                yield return PeopleNames;
                
                yield return Aperture;
                yield return Brightness;
                yield return Contrast;
                yield return DigitalZoom;
                yield return ExposureBias;
                yield return ExposureIndex;
                yield return ExposureTime;
                yield return Flash;
                yield return FNumber;
                yield return FocalLength;
                yield return ISOSpeed;
                yield return MaxAperture;
                yield return Saturation;
                yield return Sharpness;
                yield return ShutterSpeed;
                yield return WhiteBalance;
                
                yield return CameraManufacturer;
                yield return CameraModel;
                yield return CameraSerialNumber;
                yield return FlashManufacturer;
                yield return FlashModel;
                yield return LensManufacturer;
                yield return LensModel;
            }
        }
    }
}

using System;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    [Flags]
    public enum FileBaseProperties
    {
        None        = 0,
        Basic       = 1 << 0,
        Core        = 1 << 1,
        Document    = 1 << 2,
        Image       = 1 << 3,
        Photo       = 1 << 4,
        Media       = 1 << 5,
        Audio       = 1 << 6,
        Music       = 1 << 7,
        Video       = 1 << 8,
        DRM         = 1 << 9,
        GPS         = 1 << 10,
        
        All         = Basic | Core | Document | Image | Photo | Media | Audio | Music | Video | DRM | GPS
    }
}

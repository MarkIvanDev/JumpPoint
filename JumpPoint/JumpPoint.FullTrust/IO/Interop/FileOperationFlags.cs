using System;

namespace JumpPoint.FullTrust.IO.Interop
{
    [Flags]
    internal enum FileOperationFlags : uint
    {
        FOF_MULTIDESTFILES              = 0x0001,
        FOF_CONFIRMMOUSE                = 0x0002,
        FOF_SILENT                      = 0x0004,       // don't create progress/report
        FOF_RENAMEONCOLLISION           = 0x0008,

        FOF_NOCONFIRMATION              = 0x0010,       // Don't prompt the user.
        FOF_WANTMAPPINGHANDLE           = 0x0020,       // Fill in SHFILEOPSTRUCT.hNameMappings, Must be freed using SHFreeNameMappings
        FOF_ALLOWUNDO                   = 0x0040,
        FOF_FILESONLY                   = 0x0080,       // on *.*, do only files

        FOF_SIMPLEPROGRESS              = 0x0100,       // means don't show names of files
        FOF_NOCONFIRMMKDIR              = 0x0200,       // don't confirm making any needed dirs
        FOF_NOERRORUI                   = 0x0400,       // don't put up error UI
        FOF_NOCOPYSECURITYATTRIBS       = 0x0800,       // dont copy NT file Security Attributes

        FOF_NORECURSION                 = 0x1000,       // don't recurse into directories.
        FOF_NO_CONNECTED_ELEMENTS       = 0x2000,       // don't operate on connected file elements.
        FOF_WANTNUKEWARNING             = 0x4000,       // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
        FOF_NORECURSEREPARSE            = 0x8000,       // treat reparse points as objects, not containers

        FOFX_NOSKIPJUNCTIONS            = 0x0001_0000,  // Don't avoid binding to junctions (like Task folder, Recycle-Bin)
        FOFX_PREFERHARDLINK             = 0x0002_0000,  // Create hard link if possible
        FOFX_SHOWELEVATIONPROMPT        = 0x0004_0000,  // Show elevation prompts when error UI is disabled (use with FOF_NOERRORUI)
        FOFX_RECYCLEONDELETE            = 0x0008_0000,  // When a file is deleted, send it to the Recycle Bin rather than permanently deleting it.

        FOFX_EARLYFAILURE               = 0x0010_0000,  // Fail operation as soon as a single error occurs rather than trying to process other items (applies only when using FOF_NOERRORUI)
        FOFX_PRESERVEFILEEXTENSIONS     = 0x0020_0000,  // Rename collisions preserve file extns (use with FOF_RENAMEONCOLLISION)
        FOFX_KEEPNEWERFILE              = 0x0040_0000,  // Keep newer file on naming conflicts
        FOFX_NOCOPYHOOKS                = 0x0080_0000,  // Don't use copy hooks

        FOFX_NOMINIMIZEBOX              = 0x0100_0000,  // Don't allow minimizing the progress dialog
        FOFX_MOVEACLSACROSSVOLUMES      = 0x0200_0000,  // Copy security information when performing a cross-volume move operation
        FOFX_DONTDISPLAYSOURCEPATH      = 0x0400_0000,  // Don't display the path of source file in progress dialog
        FOFX_DONTDISPLAYDESTPATH        = 0x0800_0000,  // Don't display the path of destination file in progress dialog

        FOFX_REQUIREELEVATION           = 0x1000_0000,  // The user expects a requirement for rights elevation, so do not display a dialog box asking for a confirmation of the elevation.
        FOFX_ADDUNDORECORD              = 0x2000_0000,  // The file operation was user-invoked and should be placed on the undo stack. This flag is preferred to FOF_ALLOWUNDO
        FOFX_COPYASDOWNLOAD             = 0x4000_0000,  // Display a Downloading instead of Copying message in the progress dialog.
        FOFX_DONTDISPLAYLOCATIONS       = 0x8000_0000,  // Do not display the location line in the progress dialog.
    }
}

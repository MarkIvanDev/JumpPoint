using System;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public enum JPCreationCollisionOption
    {
        //
        // Summary:
        //     Automatically append a number to the base of the specified name if the file or
        //     folder already exists.
        [Description("Generate a unique name")]
        GenerateUniqueName = 0,
        //
        // Summary:
        //     Replace the existing item if the file or folder already exists.
        ReplaceExisting = 1,
        //
        // Summary:
        //     Raise an exception of type **System.Exception** if the file or folder already
        //     exists.
        [Description("Do nothing")]
        FailIfExists = 2,
        //
        // Summary:
        //     Return the existing item if the file or folder already exists.
        [Description("Open existing")]
        OpenIfExists = 3
    }
}

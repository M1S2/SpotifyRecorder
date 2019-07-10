using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericRecorder
{
    /// <summary>
    /// What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)
    /// </summary>
    public enum RecorderFileExistModes
    {
        /// <summary>
        /// Skip the file and don't record anything
        /// </summary>
        SKIP,

        /// <summary>
        /// Override the existing file
        /// </summary>
        OVERRIDE,

        /// <summary>
        /// Create a new file with another file name
        /// </summary>
        CREATENEW
    }
}

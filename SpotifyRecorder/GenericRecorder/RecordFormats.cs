using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericRecorder
{
    /// <summary>
    /// Record Formats that specify in which format(s) the recorder saves the audio data
    /// </summary>
    public enum RecordFormats
    {
        /// <summary>
        /// Record as WAV file
        /// </summary>
        WAV,

        /// <summary>
        /// Record as MP3 file
        /// </summary>
        MP3,

        /// <summary>
        /// Record as WAV and MP3 file
        /// </summary>
        WAV_AND_MP3
    }
}

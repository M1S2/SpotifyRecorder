using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericRecorder
{
    /// <summary>
    /// Recorder states.
    /// </summary>
    public enum RecordStates
    {
        /// <summary>
        /// Recording
        /// </summary>
        RECORDING,

        /// <summary>
        /// Record paused
        /// </summary>
        PAUSED,

        /// <summary>
        /// Record stopped
        /// </summary>
        STOPPED,

        /// <summary>
        /// Normalizing WAV file
        /// </summary>
        NORMALIZING,

        /// <summary>
        /// Converting WAV to MP3
        /// </summary>
        WAV_TO_MP3,

        /// <summary>
        /// Adding tags to file
        /// </summary>
        ADDING_TAGS,

        /// <summary>
        /// Removing fades from file
        /// </summary>
        REMOVING_FADES
    }

}

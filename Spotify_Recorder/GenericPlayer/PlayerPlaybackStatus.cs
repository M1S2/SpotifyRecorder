using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Recorder.GenericPlayer
{
    public class PlayerPlaybackStatus
    {
        /// <summary>
        /// Current playback device name
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Current playback device type
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Current playback device volume in percent 0 ... 100
        /// </summary>
        public int DeviceVolumePercent { get; set; }

        /// <summary>
        /// Playback progress in seconds
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Is the player playing
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// CurrentlyPlayingTrack
        /// </summary>
        public PlayerTrack Track { get; set; }

        public PlayerPlaybackStatus()
        {

        }
    }
}

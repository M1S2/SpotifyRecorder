using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericPlayer
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
        /// Playback progress
        /// </summary>
        public TimeSpan Progress { get; set; }

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

        public override bool Equals(object obj)
        {
            PlayerPlaybackStatus status2 = (PlayerPlaybackStatus)obj;
            return DeviceName == status2.DeviceName &&
                DeviceType == status2.DeviceType &&
                DeviceVolumePercent == status2.DeviceVolumePercent &&
                Progress == status2.Progress &&
                IsPlaying == status2.IsPlaying &&
                Track == status2.Track;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

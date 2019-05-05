using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericPlayer
{
    /// <summary>
    /// Event gets triggered, when the Track is changed
    /// </summary>
    public class PlayerTrackChangeEventArgs
    {
        public PlayerTrack OldTrack { get; set; }
        public PlayerTrack NewTrack { get; set; }
    }

    /// <summary>
    /// Event gets triggered, when the Playin-state is changed (e.g Play --> Pause)
    /// </summary>
    public class PlayerPlayStateEventArgs
    {
        public bool Playing { get; set; }
    }

    /// <summary>
    /// Event gets triggered, when the volume changes
    /// </summary>
    public class PlayerVolumeChangeEventArgs
    {
        public double OldVolume { get; set; }
        public double NewVolume { get; set; }
    }

    /// <summary>
    /// Event gets triggered, when the tracktime changes
    /// </summary>
    public class PlayerTrackTimeChangeEventArgs
    {
        public TimeSpan TrackTime { get; set; }
    }
}

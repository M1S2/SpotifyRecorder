using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.GenericPlayer
{
    /// <summary>
    /// Class representing a track
    /// </summary>
    public class PlayerTrack
    {
        /// <summary>
        /// An ID that is used to identify the track
        /// </summary>
        public string TrackID { get; set; }

        /// <summary>
        /// Type of the track (for example ad, ...)
        /// </summary>
        public string TrackType { get; set; }

        /// <summary>
        /// Is the track an ad?
        /// </summary>
        public bool IsAd { get { return TrackType == "ad"; } }

        /// <summary>
        /// Duration of the track
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Name of the track
        /// </summary>
        public string TrackName { get; set; }

        /// <summary>
        /// Artists of the track
        /// </summary>
        public List<PlayerArtist> Artists { get; set; }

        /// <summary>
        /// Album that the track belongs to
        /// </summary>
        public PlayerAlbum Album { get; set; }
        
        public PlayerTrack()
        {

        }

        public override string ToString()
        {
            return "Name: " + TrackName;
        }

        public override bool Equals(object obj)
        {
            PlayerTrack track2 = (PlayerTrack)obj;
            return TrackID == track2.TrackID &&
                TrackType == track2.TrackType &&
                Duration == track2.Duration &&
                TrackName == track2.TrackName &&
                Artists == track2.Artists &&
                Album == track2.Album;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

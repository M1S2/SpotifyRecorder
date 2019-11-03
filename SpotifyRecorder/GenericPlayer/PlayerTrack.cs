using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpotifyRecorder.GenericPlayer
{
    /// <summary>
    /// Class representing a track
    /// </summary>
    public class PlayerTrack
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changed</param>
        /// see: https://docs.microsoft.com/de-de/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //##############################################################################################################################################################################################

        private string _trackID;
        /// <summary>
        /// An ID that is used to identify the track
        /// </summary>
        public string TrackID
        {
            get { return _trackID; }
            set { _trackID = value; OnPropertyChanged(); }
        }

        private TimeSpan _duration;
        /// <summary>
        /// Duration of the track
        /// </summary>
        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; OnPropertyChanged(); }
        }

        private string _trackName;
        /// <summary>
        /// Name of the track
        /// </summary>
        public string TrackName
        {
            get { return _trackName; }
            set { _trackName = value; OnPropertyChanged(); }
        }

        private List<PlayerArtist> _artists;
        /// <summary>
        /// Artists of the track
        /// </summary>
        public List<PlayerArtist> Artists
        {
            get { return _artists; }
            set { _artists = value; OnPropertyChanged(); OnPropertyChanged("CombinedArtistsString"); }
        }

        /// <summary>
        /// A string with all artists combined (joined with " & ")
        /// </summary>
        public string CombinedArtistsString
        {
            get { return string.Join(" & ", Artists?.Select(a => a.ArtistName)); }
        }

        private PlayerAlbum _album;
        /// <summary>
        /// Album that the track belongs to
        /// </summary>
        public PlayerAlbum Album
        {
            get { return _album; }
            set { _album = value; OnPropertyChanged(); }
        }
        
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

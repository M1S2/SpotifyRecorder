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
    /// Class representing an playlist
    /// </summary>
    public class PlayerPlaylist
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

        private string _playlistName;
        /// <summary>
        /// Name of the playlist
        /// </summary>
        public string PlaylistName
        {
            get { return _playlistName; }
            set { _playlistName = value; OnPropertyChanged(); }
        }

        private string _playlistID;
        /// <summary>
        /// An ID that is used to identify the playlist
        /// </summary>
        public string PlaylistID
        {
            get { return _playlistID; }
            set { _playlistID = value; OnPropertyChanged(); }
        }

        public PlayerPlaylist(string playlistName, string playlistID)
        {
            PlaylistName = playlistName;
            PlaylistID = playlistID;
        }

        public override string ToString()
        {
            return "Name: " + PlaylistName;
        }

        public override bool Equals(object obj)
        {
            PlayerPlaylist playlist2 = (PlayerPlaylist)obj;
            return PlaylistName == playlist2.PlaylistName &&
                PlaylistID == playlist2.PlaylistID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

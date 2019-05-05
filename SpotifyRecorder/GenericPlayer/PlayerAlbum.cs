using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SpotifyRecorder.GenericPlayer
{
    public class PlayerAlbum
    {
        /// <summary>
        /// Name of the album
        /// </summary>
        public string AlbumName { get; set; }

        /// <summary>
        /// An ID that is used to identify the album
        /// </summary>
        public string AlbumID { get; set; }

        /// <summary>
        /// Release date of the album
        /// </summary>
        public string ReleaseDate { get; set; }
        
        /// <summary>
        /// List with album images
        /// </summary>
        public List<Bitmap> Images { get; set; }

        public PlayerAlbum(string albumName, string albumID, string releaseDate, List<Bitmap> images)
        {
            AlbumName = albumName;
            AlbumID = albumID;
            ReleaseDate = releaseDate;
            Images = images;
        }

        public override string ToString()
        {
            return "Name: " + AlbumName;
        }

        public override bool Equals(object obj)
        {
            PlayerAlbum album2 = (PlayerAlbum)obj;
            return AlbumName == album2.AlbumName &&
                AlbumID == album2.AlbumID &&
                ReleaseDate == album2.ReleaseDate;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

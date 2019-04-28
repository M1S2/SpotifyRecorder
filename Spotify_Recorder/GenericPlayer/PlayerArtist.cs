﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Recorder.GenericPlayer
{
    /// <summary>
    /// Class representing an artist
    /// </summary>
    public class PlayerArtist
    {
        /// <summary>
        /// Name of the artist
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// An ID that is used to identify the artist
        /// </summary>
        public string ArtistID { get; set; }

        public PlayerArtist(string artistName, string artistID)
        {
            ArtistName = artistName;
            ArtistID = artistID;
        }

        public override string ToString()
        {
            return "Name: " + ArtistName;
        }

    }
}

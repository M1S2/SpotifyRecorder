using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Net;
using System.Diagnostics;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Spotify_Recorder.GenericPlayer
{
    public class SpotifyPlayer : Player
    {
        private SpotifyWebAPI _spotifyWeb;

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new SpotifyPlayer instance
        /// </summary>
        /// <param name="timerIntervall_ms">Polling interval of track informations that are used to raise events</param>
        public SpotifyPlayer(int timerIntervall_ms = 50) : base(timerIntervall_ms)
        {
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Return the Player name ("Spotify")
        /// </summary>
        public override string PlayerName => "Spotify";

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if the spotify application is running
        /// </summary>
        public override bool IsPlayerApplicationRunning
        {
            get { return Process.GetProcessesByName("spotify").Length >= 1; }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the current playback status
        /// </summary>
        /// <returns>PlayerPlaybackStatus</returns>
        public override PlayerPlaybackStatus CurrentPlaybackStatus
        {
            get
            {
                PlaybackContext _spotifyPlayback = _spotifyWeb.GetPlayback();
                PlayerPlaybackStatus playerPlayback = new PlayerPlaybackStatus()
                {
                    DeviceName = _spotifyPlayback.Device?.Name,
                    DeviceType = _spotifyPlayback.Device?.Type,
                    DeviceVolumePercent = (_spotifyPlayback.Device == null ? -1 : _spotifyPlayback.Device.VolumePercent),
                    IsPlaying = _spotifyPlayback.IsPlaying,
                    Progress = _spotifyPlayback.ProgressMs / (double)1000,
                    Track = convertSpotifyTrackToPlayerTrack(_spotifyPlayback.Item)
                };
                return playerPlayback;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the spotify desktop application if it's not running 
        /// </summary>
        /// <returns>true -> started successful, false -> error while starting application</returns>
        public override bool StartPlayerApplication()
        {
            if (!IsPlayerApplicationRunning)
            {
                ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe", "--enable-audio-graph");     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify, use the --enable-audio-graph option to have the possibility to change the output device
                System.Threading.Thread.Sleep(2000);        //Wait some time before connecting to spotify
            }
            else    //Spotify is running, make sure it was started with the "--enable-audio-graph" option
            {
                List<string> spotifyStartArguments = ProcessHelper.GetProcessStartArguments("Spotify");
                if (!spotifyStartArguments.Any(str => str.Contains("--enable-audio-graph")))     // no process with the name "Spotify.exe" was started with the "--enable-audio-graph" option
                {
                    if (MessageBox.Show("Spotify was started without the \"--enable-audio-graph\" option.\nTo use this recorder, close Spotify and then click on \"OK\".\nYou can click on \"Cancel\" to close Spotify later. After starting it with the correct option, click on \"Connect to spotify\" in the menu.", "Spotify must run with \"--enable-audio-graph\" option.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        StartPlayerApplication();
                        return false;
                    }
                    else { return true; }
                }
            }
            return true;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Connect to the Spotify Web API
        /// </summary>
        /// <returns>true on connection success, otherwise false</returns>
        /// see: https://github.com/JohnnyCrazy/SpotifyAPI-NET/issues/254
        public override async Task<bool> Connect()
        {
            WebAPIFactory webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                // Spotify API Client ID goes here, you SHOULD get your own at https://developer.spotify.com/dashboard/
                // It should be noted, "http://localhost:8000" must be whitelisted in your dashboard after getting your own client key
                "ab0969d9fab2486182e57bfbe8590df4",
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistReadCollaborative |
                Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState | Scope.UserModifyPlaybackState,
                new SpotifyAPI.ProxyConfig());

            try
            {
                _spotifyWeb = await webApiFactory.GetWebApi();
            }
            catch (Exception)
            {
                return false;
            }

            if (_spotifyWeb == null) { return false; }
            return true;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Convert a Spotify Web API FullTrack to a generic PlayerTrack
        /// </summary>
        /// <param name="track">Spotify Web API FullTrack</param>
        /// <returns>Generic PlayerTrack</returns>
        private PlayerTrack convertSpotifyTrackToPlayerTrack(FullTrack track)
        {
            if(track == null) { return null; }

            PlayerTrack pTrack = new PlayerTrack()
            {
                TrackID = track.Id,
                Duration = track.DurationMs / (double)1000,
                TrackType = track.Type,
                TrackName = track.Name,
                Artists = track.Artists.Select(a => new PlayerArtist(a.Name, a.Id)).ToList(),
                Album = new PlayerAlbum(track.Album.Name, track.Album.Id, track.Album.ReleaseDate, track.Album.Images.Select(i => getBitmapFromUrl(i.Url)).ToList())
            };
            return pTrack;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the bitmap from the given url
        /// </summary>
        /// <param name="url">URL to bitmap</param>
        /// <returns>Bitmap</returns>
        /// see: https://stackoverflow.com/questions/12142634/how-to-download-an-image-from-an-uri-and-create-a-bitmap-object-from-it
        private Bitmap getBitmapFromUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Bitmap albumBitmap = new Bitmap(responseStream);
            return albumBitmap;
        }

    }
}

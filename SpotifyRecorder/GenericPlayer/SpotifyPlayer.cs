using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SpotifyRecorder.GenericPlayer
{
    public class SpotifyPlayer : Player
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        // see: https://docs.microsoft.com/de-de/windows/desktop/inputdev/virtual-key-codes
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        private const int VK_MEDIA_PREV_TRACK = 0xB1;

        private SpotifyWebAPI _spotifyWeb;

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new SpotifyPlayer instance
        /// </summary>
        /// <param name="timerIntervall_ms">Polling interval of track informations that are used to raise events</param>
        /// <param name="mainWindow">MahApps.MetroWindow handle to show messages from the player as metro message box</param>
        public SpotifyPlayer(int timerIntervall_ms = 50, MetroWindow mainWindow = null) : base(timerIntervall_ms, mainWindow)
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
        public override void UpdateCurrentPlaybackStatus()
        {
            if (_spotifyWeb == null) { return; }
            PlaybackContext _spotifyPlayback = _spotifyWeb.GetPlayback();
            PlayerPlaybackStatus playerPlayback = new PlayerPlaybackStatus()
            {
                DeviceName = _spotifyPlayback.Device?.Name,
                DeviceType = _spotifyPlayback.Device?.Type,
                DeviceVolumePercent = (_spotifyPlayback.Device == null ? -1 : _spotifyPlayback.Device.VolumePercent),
                IsPlaying = _spotifyPlayback.IsPlaying,
                Progress = new TimeSpan(0, 0, 0, 0, _spotifyPlayback.ProgressMs),
                Track = convertSpotifyTrackToPlayerTrack(_spotifyPlayback.Item)
            };
            CurrentPlaybackStatus = playerPlayback;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the spotify desktop application if it's not running 
        /// </summary>
        /// <returns>true -> started successful, false -> error while starting application</returns>
        public override Task<bool> StartPlayerApplication()
        {
#warning https://community.spotify.com/t5/Desktop-Windows/Missing-enable-audio-graph-has-stopped-giving-option-to-select/td-p/4726519
#warning Set Spotify output device over windows Sound settings (Integrate to setup project) >> App sound settings >> Spotify output = CABLE Input

            if (!IsPlayerApplicationRunning)
            {
                ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe");     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify
                System.Threading.Thread.Sleep(2000);        //Wait some time before connecting to spotify
            }
            return Task.FromResult(true);

            /*if (!IsPlayerApplicationRunning)
            {
                ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe", "--enable-audio-graph");     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify, use the --enable-audio-graph option to have the possibility to change the output device
                System.Threading.Thread.Sleep(2000);        //Wait some time before connecting to spotify
            }
            else    //Spotify is running, make sure it was started with the "--enable-audio-graph" option
            {
                List<string> spotifyStartArguments = ProcessHelper.GetProcessStartArguments("Spotify");
                if (!spotifyStartArguments.Any(str => str.Contains("--enable-audio-graph")))     // no process with the name "Spotify.exe" was started with the "--enable-audio-graph" option
                {
                    bool wasClosed = false;
                    if (_mainWindow == null)
                    {
                        wasClosed = (MessageBox.Show("Spotify was started without the \"--enable-audio-graph\" option.\nTo use this recorder, close Spotify and then click on \"OK\".\nYou can click on \"Cancel\" to close Spotify later. After starting it with the correct option, try to connect again.", "Spotify must run with \"--enable-audio-graph\" option.", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK);
                    }
                    else
                    {
                        wasClosed = (await _mainWindow.ShowMessageAsync("Spotify must run with \"--enable-audio-graph\" option.", "Spotify was started without the \"--enable-audio-graph\" option.\nTo use this recorder, close Spotify and then click on \"OK\".\nYou can click on \"Cancel\" to close Spotify later. After starting it with the correct option, try to connect again.", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative);
                    }

                    if (wasClosed)
                    {
                        return await StartPlayerApplication();
                    }
                    else { return false; }
                }
            }
            return true;*/
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
                IsConnected = false;
                return false;
            }

            if (_spotifyWeb == null) { IsConnected = false; return false; }

            IsConnected = true;
            return true;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the current playback
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void StartPlayback()
        {
            if (CurrentPlaybackStatus?.IsPlaying == false)
            {
                keybd_event((byte)VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Pause the current playback
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void PausePlayback()
        {
            if (CurrentPlaybackStatus?.IsPlaying == true)
            {
                keybd_event((byte)VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the next track
        /// </summary>
        public override void NextTrack()
        {
            keybd_event((byte)VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the previous track
        /// </summary>
        public override void PreviousTrack()
        {
            keybd_event((byte)VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
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
                Duration = new TimeSpan(0, 0, 0, 0, track.DurationMs),
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

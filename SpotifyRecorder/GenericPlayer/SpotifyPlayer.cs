using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        //https://community.spotify.com/t5/Desktop-Windows/Missing-enable-audio-graph-has-stopped-giving-option-to-select/td-p/4726519


        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        // see: https://docs.microsoft.com/de-de/windows/desktop/inputdev/virtual-key-codes
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        private const int VK_MEDIA_NEXT_TRACK = 0xB0;
        private const int VK_MEDIA_PREV_TRACK = 0xB1;
        private const int VK_VOLUME_MUTE = 0xAD;

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
                Track = convertSpotifyTrackToPlayerTrack(_spotifyPlayback.Item),
                IsAd = _spotifyPlayback.CurrentlyPlayingType == TrackType.Ad
            };
            CurrentPlaybackStatus = playerPlayback;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the spotify desktop application if it's not running 
        /// </summary>
        /// <param name="minimized">true -> start minimized; otherwise false</param>
        /// <returns>true -> started successful, false -> error while starting application</returns>
        public async override Task<bool> StartPlayerApplication(bool minimized = false)
        {
            bool startResult = true;
            if (!IsPlayerApplicationRunning)
            {
                startResult = await ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe", (minimized ? "--minimized" : ""));     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify
                //startResult = await ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe", "", (minimized ? ProcessWindowStyle.Minimized : ProcessWindowStyle.Maximized));     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify
                await Task.Delay(2000);        //Wait some time before connecting to spotify
            }
            return startResult;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Close the player application async
        /// </summary>
        /// <returns>true -> closed successful, false -> error while closing application</returns>
        public override Task<bool> ClosePlayerApplication()
        {
            ProcessHelper.StopProcess("Spotify", true);
            return Task.FromResult(true);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Connect to the Spotify Web API
        /// </summary>
        /// <param name="timeout_ms">Connection timeout in ms</param>
        /// <returns>true on connection success, otherwise false</returns>
        /// see: https://johnnycrazy.github.io/SpotifyAPI-NET/SpotifyWebAPI/auth/#implicitgrantauth
        /// Use https://developer.spotify.com/dashboard/ to get a Client ID 
        /// It should be noted, "http://localhost:8000" must be whitelisted in your dashboard after getting your own client key
        public override async Task<bool> Connect(int timeout_ms = 10000)
        {
            await Task.Run(() =>
            {
                _spotifyWeb = null;
                ManualResetEvent waitforAuthFinish = new ManualResetEvent(false);
                ImplicitGrantAuth auth = new ImplicitGrantAuth("ab0969d9fab2486182e57bfbe8590df4", "http://localhost:8000", "http://localhost:8000", Scope.UserReadPlaybackState);
                auth.AuthReceived += (sender, payload) =>
                {
                    auth.Stop(); // `sender` is also the auth instance
                    _spotifyWeb = new SpotifyWebAPI() { TokenType = payload.TokenType, AccessToken = payload.AccessToken };
                    waitforAuthFinish.Set();
                };
                auth.Start(); // Starts an internal HTTP Server
                auth.OpenBrowser();
                waitforAuthFinish.WaitOne(timeout_ms);
            });

            if (_spotifyWeb == null) { IsConnected = false; return false; }
            else { IsConnected = true; return true; }           
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the current playback
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void StartPlayback()
        {
            if (CurrentPlaybackStatus?.IsPlaying == false) { TogglePlayPause(); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Pause the current playback
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void PausePlayback()
        {
            if (CurrentPlaybackStatus?.IsPlaying == true) { TogglePlayPause(); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Toggle between play and pause playback state. If the player is playing the playback is paused. If the player is paused the playback is started again.
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void TogglePlayPause()
        {
            keybd_event((byte)VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Toggle between mute and unmute state.
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void ToggleMuteState()
        {
            keybd_event((byte)VK_VOLUME_MUTE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the next track
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void NextTrack()
        {
            keybd_event((byte)VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the previous track
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
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

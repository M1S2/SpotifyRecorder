﻿using System;
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
using System.Text.RegularExpressions;
using Microsoft.Win32;
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
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        private const int WM_APPCOMMAND = 0x0319;

        // see: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-appcommand
        // Tryout commands with WinSpy++
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 0xE0000;
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 0xB0000;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 0xC0000;
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;

        private SpotifyWebAPI _spotifyWeb;
        private Process _spotifyProcess;
        private bool _wasConnectionTokenExpiredEventRaised = false;

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
            get { return ProcessHelper.IsProcessOpen("Spotify", true); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the current playback status
        /// </summary>
        public override void UpdateCurrentPlaybackStatus()
        {
            if (_spotifyWeb == null) { return; }
            PlaybackContext _spotifyPlayback = _spotifyWeb.GetPlayback();

            if(_spotifyPlayback.Error?.Status == 401)    // Error 401: "The access token expired"
            {
                IsConnectionTokenExpired = true;
                if (_wasConnectionTokenExpiredEventRaised == false)
                {
                    _wasConnectionTokenExpiredEventRaised = true;
                    RaiseOnPlayerConnectionTokenExpiredEvent();
                }
                return;
            }

            string playlistID = "", playlistName = "";

            if (_spotifyPlayback.Context != null && _spotifyPlayback.Context.Type == "playlist")
            {
                playlistID = _spotifyPlayback.Context.Uri.Split(':').Last();
                playlistName = _spotifyWeb?.GetPlaylist(playlistId: playlistID, fields: "name").Name;
            }

            PlayerPlaybackStatus playerPlayback = new PlayerPlaybackStatus()
            {
                DeviceName = _spotifyPlayback.Device?.Name,
                DeviceType = _spotifyPlayback.Device?.Type,
                DeviceVolumePercent = (_spotifyPlayback.Device == null ? -1 : _spotifyPlayback.Device.VolumePercent),
                IsPlaying = _spotifyPlayback.IsPlaying,
                Progress = new TimeSpan(0, 0, 0, 0, _spotifyPlayback.ProgressMs),
                Track = convertSpotifyTrackToPlayerTrack(_spotifyPlayback.Item),
                IsAd = _spotifyPlayback.CurrentlyPlayingType == TrackType.Ad,
                Playlist = playlistID != "" ? new PlayerPlaylist(playlistName, playlistID) : null
            };
            if (playerPlayback.Track != null) { playerPlayback.Track.Playlist = playerPlayback.Playlist; }
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
#warning Try to open Spotify maximized
                // Try to start Spotify installed from downloaded installer (installed in C:\Users\%user%\AppData\Roaming\Spotify)
                startResult = await ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe", (minimized ? "--minimized" : ""), (minimized ? ProcessWindowStyle.Normal : ProcessWindowStyle.Maximized));

                if (!startResult)
                {
                    // Try to start Spotify installed from Microsoft store (installed in C:\Program Files\WindowsApps folder)
                    startResult = await ProcessHelper.StartProcess("explorer", @"shell:appsfolder\SpotifyAB.SpotifyMusic_zpdnekdrzrea0!Spotify");
                }
                await Task.Delay(2000);        //Wait some time before connecting to spotify
            }

            _spotifyProcess = ProcessHelper.FindProcess("Spotify", true).FirstOrDefault();

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
        /// <param name="forceReauthenticate">if true, force the user to reauthenticate to the player application</param>
        /// <returns>true on connection success, otherwise false</returns>
        /// see: https://johnnycrazy.github.io/SpotifyAPI-NET/SpotifyWebAPI/auth/#implicitgrantauth
        /// Use https://developer.spotify.com/dashboard/ to get a Client ID 
        /// It should be noted, "http://localhost:8000" must be whitelisted in your dashboard after getting your own client key
        public override async Task<bool> Connect(int timeout_ms = 10000, bool forceReauthenticate = false)
        {
            await Task.Run(() =>
            {
                bool useExternalBrowser = false;

                _spotifyWeb = null;
                ManualResetEvent waitForAuthFinish = new ManualResetEvent(false);
                ManualResetEvent waitForWindowClosed = new ManualResetEvent(false);
                ImplicitGrantAuth auth = new ImplicitGrantAuth("ab0969d9fab2486182e57bfbe8590df4", "http://localhost:8000", "http://localhost:8000", Scope.UserReadPlaybackState);
                
                if (useExternalBrowser)
                {
                    auth.AuthReceived += (sender, payload) =>
                    {
                        auth.Stop(); // `sender` is also the auth instance
                        _spotifyWeb = new SpotifyWebAPI() { TokenType = payload.TokenType, AccessToken = payload.AccessToken };
                        waitForAuthFinish.Set();
                    };
                    auth.Start(); // Starts an internal HTTP Server
                    auth.OpenBrowser();

                    waitForAuthFinish.WaitOne(timeout_ms);
                }
                else
                {
                    auth.ShowDialog = forceReauthenticate;
                    string url = auth.GetUri();                 // URL to spotify login page

                    bool userInteractionWaiting = false;

                    Thread newThread = new Thread(new ThreadStart(() =>         // Need to use a new thread with ApartmentState STA, otherwise WebBrowser control can't be used like that
                    {
                        Window authWindow = new Window();
                        System.Windows.Controls.WebBrowser webBrowser = new System.Windows.Controls.WebBrowser();
                        authWindow.Title = "Spotify Authentication";
                        authWindow.Content = webBrowser;
                        authWindow.WindowState = forceReauthenticate ? WindowState.Normal : WindowState.Minimized;
                        userInteractionWaiting = forceReauthenticate;

                        bool authWindowClosedByProgram = false;

                        webBrowser.Navigated += (sender, args) =>
                        {
                            string urlFinal = args.Uri.ToString();      // URL maybe containing the access_token
                            if (urlFinal.Contains("access_token"))      // Authentication finished when the URL contains the access_token
                            {
                                string accessToken = "", tokenType = "";

                                Regex regex = new Regex(@"(\?|\&|#)([^=]+)\=([^&]+)");      // Extract the fields from the returned URL
                                MatchCollection matches = regex.Matches(urlFinal);
                                foreach (Match match in matches)
                                {
                                    if (match.Value.Contains("access_token")) { accessToken = match.Value.Replace("#access_token=", ""); }
                                    else if (match.Value.Contains("token_type")) { tokenType = match.Value.Replace("&token_type=", ""); }
                                    else if (match.Value.Contains("expires_in")) { ConnectionTokenExpirationTime = new TimeSpan(0, 0, int.Parse(match.Value.Replace("&expires_in=", ""))); }
                                }

                                _spotifyWeb = new SpotifyWebAPI() { TokenType = tokenType, AccessToken = accessToken };
                                waitForAuthFinish.Set();        // Signal that the authentication finished

                                authWindowClosedByProgram = true;
                                authWindow.Close();
                            }
                            else
                            {
                                authWindow.WindowState = WindowState.Normal;
                                userInteractionWaiting = true;
                            }
                        };

                        authWindow.Closed += (sender, args) =>
                        {
                            waitForWindowClosed.Set();
                            if (!authWindowClosedByProgram) { waitForAuthFinish.Set(); }
                        };

                        webBrowser.Navigate(url);       // Navigate to spotifys login page to begin authentication. If credentials exist, you are redirected to an URL containing the access_token.
                        authWindow.ShowDialog();
                    }));
                    newThread.SetApartmentState(ApartmentState.STA);
                    newThread.Start();

                    waitForAuthFinish.WaitOne(timeout_ms);
                    if (userInteractionWaiting) { waitForWindowClosed.WaitOne(); waitForAuthFinish.WaitOne(timeout_ms); }
                }
            });

            if (_spotifyWeb == null) { IsConnected = false; return false; }
            else { _wasConnectionTokenExpiredEventRaised = false; IsConnected = true; return true; }
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
        /// https://stackoverflow.com/questions/17842612/vb-simulate-a-key-press
        /// https://stackoverflow.com/questions/7181978/special-keys-on-keyboards/7182076#7182076
        public override void TogglePlayPause()
        {
            if (_spotifyProcess == null) { _spotifyProcess = ProcessHelper.FindProcess("Spotify", true).FirstOrDefault(); }
            if (_spotifyProcess != null)
            {
                SendMessage(_spotifyProcess.MainWindowHandle, WM_APPCOMMAND, 0, APPCOMMAND_MEDIA_PLAY_PAUSE);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Toggle between mute and unmute state.
        /// </summary>
        /// see: https://stackoverflow.com/questions/1645815/how-can-i-programmatically-generate-keypress-events-in-c
        public override void ToggleMuteState()
        {
            if (_spotifyProcess == null) { _spotifyProcess = ProcessHelper.FindProcess("Spotify", true).FirstOrDefault(); }
            if (_spotifyProcess != null)
            {
                SendMessage(_spotifyProcess.MainWindowHandle, WM_APPCOMMAND, 0, APPCOMMAND_VOLUME_MUTE);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the next track
        /// </summary>
        public override void NextTrack()
        {
            if (_spotifyProcess == null) { _spotifyProcess = ProcessHelper.FindProcess("Spotify", true).FirstOrDefault(); }
            if (_spotifyProcess != null)
            {
                SendMessage(_spotifyProcess.MainWindowHandle, WM_APPCOMMAND, 0, APPCOMMAND_MEDIA_NEXTTRACK);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the previous track
        /// </summary>
        public override void PreviousTrack()
        {
            if (_spotifyProcess == null) { _spotifyProcess = ProcessHelper.FindProcess("Spotify", true).FirstOrDefault(); }
            if (_spotifyProcess != null)
            {
                SendMessage(_spotifyProcess.MainWindowHandle, WM_APPCOMMAND, 0, APPCOMMAND_MEDIA_PREVIOUSTRACK);
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;

namespace Spotify_Recorder.GenericPlayer
{
    /// <summary>
    /// Abstract class for a generic player application (e.g. Spotify).
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Event gets triggered, when the Track is changed
        /// </summary>
        public event EventHandler<PlayerTrackChangeEventArgs> OnTrackChange;

        /// <summary>
        /// Event gets triggered, when the Playin-state is changed (e.g Play --> Pause)
        /// </summary>
        public event EventHandler<PlayerPlayStateEventArgs> OnPlayStateChange;

        /// <summary>
        /// Event gets triggered, when the volume changes
        /// </summary>
        public event EventHandler<PlayerVolumeChangeEventArgs> OnVolumeChange;

        /// <summary>
        /// Event gets triggered, when the tracktime changes
        /// </summary>
        public event EventHandler<PlayerTrackTimeChangeEventArgs> OnTrackTimeChange;

        //***********************************************************************************************************************************************************************************************************

        private bool _listenForEvents;
        /// <summary>
        /// Listen and fire events
        /// </summary>
        public bool ListenForEvents
        {
            get
            {
                return _listenForEvents;
            }
            set
            {
                _listenForEvents = value;
                _eventTimer.Enabled = value;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Return the Player name
        /// </summary>
        public abstract string PlayerName { get; }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if the player application is running
        /// </summary>
        /// <returns>true if running, false if not running</returns>
        public abstract bool IsPlayerApplicationRunning { get; }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the current playback status
        /// </summary>
        /// <returns>PlayerPlaybackStatus</returns>
        public abstract PlayerPlaybackStatus CurrentPlaybackStatus { get; }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the currently playing track from the PlaybackStatus
        /// </summary>
        /// <returns>PlayerTrack</returns>
        public PlayerTrack CurrentTrack { get { return CurrentPlaybackStatus?.Track; } }

        //***********************************************************************************************************************************************************************************************************

        private PlayerPlaybackStatus _tmpPlaybackStatus;
        private Timer _eventTimer;

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Connect to the player
        /// </summary>
        /// <returns>true on connection success, otherwise false</returns>
        public abstract Task<bool> Connect();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the player application
        /// </summary>
        /// <returns>true -> started successful, false -> error while starting application</returns>
        public abstract bool StartPlayerApplication();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new Player instance
        /// </summary>
        /// <param name="timerIntervall_ms">Polling interval of track informations that are used to raise events</param>
        public Player(int timerIntervall_ms = 50)
        {
            _eventTimer = new Timer() { Interval = timerIntervall_ms, AutoReset = false, Enabled = false };
            _eventTimer.Elapsed += ElapsedTick;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if an event has to be raised.
        /// </summary>
        private void ElapsedTick(object sender, ElapsedEventArgs e)
        {
            if (_tmpPlaybackStatus == null)
            {
                _tmpPlaybackStatus = CurrentPlaybackStatus;
                _eventTimer.Start();
                return;
            }
            PlayerPlaybackStatus newPlaybackStatus = CurrentPlaybackStatus;
            if (newPlaybackStatus == null)
            {
                _eventTimer.Start();
                return;
            }
            if (!newPlaybackStatus.IsPlaying && newPlaybackStatus.Track == null)
            {
                _eventTimer.Start();
                return;
            }
            if (newPlaybackStatus.Track != null && _tmpPlaybackStatus.Track != null)
            {
                if (newPlaybackStatus.Track?.TrackID != _tmpPlaybackStatus.Track?.TrackID || newPlaybackStatus.Track.TrackType == "other" && newPlaybackStatus.Track.Duration != newPlaybackStatus.Track.Duration)
                {
                    OnTrackChange?.Invoke(this, new PlayerTrackChangeEventArgs()
                    {
                        OldTrack = _tmpPlaybackStatus.Track,
                        NewTrack = newPlaybackStatus.Track
                    });
                }
            }
            if (newPlaybackStatus.IsPlaying != _tmpPlaybackStatus.IsPlaying)
            {
                OnPlayStateChange?.Invoke(this, new PlayerPlayStateEventArgs()
                {
                    Playing = newPlaybackStatus.IsPlaying
                });
            }
            if (newPlaybackStatus.DeviceVolumePercent != _tmpPlaybackStatus.DeviceVolumePercent)
            {
                OnVolumeChange?.Invoke(this, new PlayerVolumeChangeEventArgs()
                {
                    OldVolume = _tmpPlaybackStatus.DeviceVolumePercent,
                    NewVolume = newPlaybackStatus.DeviceVolumePercent
                });
            }
            if (newPlaybackStatus.Progress != _tmpPlaybackStatus.Progress)
            {
                OnTrackTimeChange?.Invoke(this, new PlayerTrackTimeChangeEventArgs()
                {
                    TrackTime = newPlaybackStatus.Progress
                });
            }
            _tmpPlaybackStatus = newPlaybackStatus;
            _eventTimer.Start();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls;

namespace SpotifyRecorder.GenericPlayer
{
    /// <summary>
    /// Abstract class for a generic player application (e.g. Spotify).
    /// </summary>
    public abstract class Player : INotifyPropertyChanged
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

        /// <summary>
        /// Create a new Player instance
        /// </summary>
        /// <param name="timerIntervall_ms">Polling interval of track informations that are used to raise events</param>
        /// <param name="mainWindow">MahApps.MetroWindow handle to show messages from the player as metro message box</param>
        public Player(int timerIntervall_ms = 50, MetroWindow mainWindow = null)
        {
            IsConnected = false;
            _eventTimer = new Timer() { Interval = timerIntervall_ms, AutoReset = false, Enabled = false };
            _eventTimer.Elapsed += ElapsedTick;
            ListenForEvents = false;
            _mainWindow = mainWindow;
        }

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
                OnPropertyChanged();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private bool _isConnected;
        /// <summary>
        /// Is the connection to the player established
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            protected set { _isConnected = value; OnPropertyChanged(); }
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

        private PlayerPlaybackStatus _currentPlaybackStatus;
        /// <summary>
        /// The current playback status
        /// </summary>
        /// <returns>PlayerPlaybackStatus</returns>
        public PlayerPlaybackStatus CurrentPlaybackStatus
        {
            get { return _currentPlaybackStatus; }
            protected set
            {
                if (_currentPlaybackStatus != value) { _currentPlaybackStatus = value; OnPropertyChanged(); }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// The currently playing track from the PlaybackStatus
        /// </summary>
        /// <returns>PlayerTrack</returns>
        public PlayerTrack CurrentTrack { get { return CurrentPlaybackStatus?.Track; } }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// MahApps.MetroWindow handle to show messages from the player as metro message box
        /// </summary>
        protected MetroWindow _mainWindow { get; set; }

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
        /// Start the player application async
        /// </summary>
        /// <returns>true -> started successful, false -> error while starting application</returns>
        public abstract Task<bool> StartPlayerApplication();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the current playback status
        /// </summary>
        public abstract void UpdateCurrentPlaybackStatus();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the current playback
        /// </summary>
        public abstract void StartPlayback();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Pause the current playback
        /// </summary>
        public abstract void PausePlayback();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the next track
        /// </summary>
        public abstract void NextTrack();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Skip to the previous track
        /// </summary>
        public abstract void PreviousTrack();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if an event has to be raised.
        /// </summary>
        private void ElapsedTick(object sender, ElapsedEventArgs e)
        {
            if (_tmpPlaybackStatus == null)
            {
                UpdateCurrentPlaybackStatus();
                _tmpPlaybackStatus = CurrentPlaybackStatus;
                _eventTimer.Start();
                return;
            }
            UpdateCurrentPlaybackStatus();
            if (CurrentPlaybackStatus == null)
            {
                _eventTimer.Start();
                return;
            }
            if (!CurrentPlaybackStatus.IsPlaying && CurrentPlaybackStatus.Track == null)
            {
                _eventTimer.Start();
                return;
            }
            if (CurrentPlaybackStatus.Track != null && _tmpPlaybackStatus.Track != null)
            {
                if (CurrentPlaybackStatus.Track?.TrackID != _tmpPlaybackStatus.Track?.TrackID || CurrentPlaybackStatus.Track.TrackType == "other" && CurrentPlaybackStatus.Track.Duration != CurrentPlaybackStatus.Track.Duration)
                {
                    OnTrackChange?.Invoke(this, new PlayerTrackChangeEventArgs()
                    {
                        OldTrack = _tmpPlaybackStatus.Track,
                        NewTrack = CurrentPlaybackStatus.Track
                    });
                }
            }
            if (CurrentPlaybackStatus.IsPlaying != _tmpPlaybackStatus.IsPlaying)
            {
                OnPlayStateChange?.Invoke(this, new PlayerPlayStateEventArgs()
                {
                    Playing = CurrentPlaybackStatus.IsPlaying
                });
            }
            if (CurrentPlaybackStatus.DeviceVolumePercent != _tmpPlaybackStatus.DeviceVolumePercent)
            {
                OnVolumeChange?.Invoke(this, new PlayerVolumeChangeEventArgs()
                {
                    OldVolume = _tmpPlaybackStatus.DeviceVolumePercent,
                    NewVolume = CurrentPlaybackStatus.DeviceVolumePercent
                });
            }
            if (CurrentPlaybackStatus.Progress != _tmpPlaybackStatus.Progress)
            {
                OnTrackTimeChange?.Invoke(this, new PlayerTrackTimeChangeEventArgs()
                {
                    TrackTime = CurrentPlaybackStatus.Progress
                });
            }
            _tmpPlaybackStatus = CurrentPlaybackStatus;
            _eventTimer.Start();
            OnPropertyChanged("CurrentPlaybackStatus");
            OnPropertyChanged("CurrentTrack");
        }

    }
}

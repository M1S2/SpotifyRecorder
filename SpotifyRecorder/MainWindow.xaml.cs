using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SpotifyRecorder.GenericPlayer;
using SpotifyRecorder.GenericRecorder;

namespace SpotifyRecorder
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
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

        private IProgress<LogBox.LogEvent> _logHandle;

        private Player _playerApp;
        public Player PlayerApp
        {
            get { return _playerApp; }
            set { _playerApp = value; OnPropertyChanged(); }
        }

        private Recorder _recorderHandle;
        public Recorder RecorderHandle
        {
            get { return _recorderHandle; }
            set { _recorderHandle = value; OnPropertyChanged(); }
        }

        private bool _isRecorderArmed;
        public bool IsRecorderArmed
        {
            get { return _isRecorderArmed; }
            set
            {
                _isRecorderArmed = value;
                logBox1.LogEvent(new LogBox.LogEventInfo("SpotifyRecorder " + (_isRecorderArmed ? "armed." : "disarmed.")));
                OnPropertyChanged();
            }
        }

        //##############################################################################################################################################################################################

        #region Commands

        private ICommand _infoCommand;
        public ICommand InfoCommand
        {
            get
            {
                if (_infoCommand == null)
                {
                    _infoCommand = new WindowTheme.RelayCommand(param =>
                    {
                        AssemblyInfoHelper_WPF.WindowAssemblyInfo windowAssemblyInfo = new AssemblyInfoHelper_WPF.WindowAssemblyInfo();
                        windowAssemblyInfo.ShowDialog();
                    });
                }
                return _infoCommand;
            }
        }
        
        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new WindowTheme.RelayCommand(async param =>
                    {
                        await startAndConnectToPlayer();
                    }); //, param => { return PlayerApp?.IsConnected == false; });
                }
                return _connectCommand;
            }
        }

        private ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                if (_playPauseCommand == null)
                {
                    _playPauseCommand = new WindowTheme.RelayCommand(param =>
                    {
                        PlayerApp.TogglePlayPause();
                    }, param => { return PlayerApp?.CurrentPlaybackStatus != null; });
                }
                return _playPauseCommand;
            }
        }

        private ICommand _openFileNamePrototypeCommand;
        public ICommand OpenFileNamePrototypeCommand
        {
            get
            {
                if (_openFileNamePrototypeCommand == null)
                {
                    _openFileNamePrototypeCommand = new WindowTheme.RelayCommand(param =>
                    {
                        FileNamePrototypeCreator fileNamePrototypeCreator = new FileNamePrototypeCreator(RecorderHandle.FileNamePrototype, RecorderHandle.RecordBasePath, PlayerApp.CurrentTrack?.TrackName, PlayerApp.CurrentTrack?.Artists[0].ArtistName, PlayerApp.CurrentTrack?.Album.AlbumName, RecorderHandle.FileExistMode);
                        if(fileNamePrototypeCreator.ShowDialog().Value == true)
                        {
                            RecorderHandle.FileNamePrototype = fileNamePrototypeCreator.FileNamePrototype;
                        }
                    });
                }
                return _openFileNamePrototypeCommand;
            }
        }

        #endregion

        //##############################################################################################################################################################################################

        public MainWindow()
        {
            InitializeComponent();

            logBox1.AutoScrollToLastLogEntry = true;
            _logHandle = new Progress<LogBox.LogEvent>(progressValue =>
            {
                logBox1.LogEvent(progressValue);
            });
        }

        //***********************************************************************************************************************************************************************************************************

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayerApp = new SpotifyPlayer(10, this);

            await startAndConnectToPlayer();

            RecorderHandle = new Recorder(_logHandle);
            RecorderHandle.TrackInfo = PlayerApp.CurrentTrack;
        }

        //***********************************************************************************************************************************************************************************************************

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (RecorderHandle != null) { RecorderHandle.StopRecord(); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the player application if it's not running and connect to it
        /// </summary>
        private async Task startAndConnectToPlayer()
        {
            if (!PlayerApp.IsPlayerApplicationRunning) { _logHandle.Report(new LogBox.LogEventInfo("Starting " + PlayerApp.PlayerName + " application.")); }
            bool playerStarted = await PlayerApp.StartPlayerApplication();

            if (playerStarted)
            {
                _logHandle.Report(new LogBox.LogEventInfo("Connecting..."));
                await PlayerApp.Connect();

                if (PlayerApp.IsConnected) { _logHandle.Report(new LogBox.LogEventInfo("Connected successfully.")); }
                else { _logHandle.Report(new LogBox.LogEventWarning("Connection failed.")); }

                if (PlayerApp.IsConnected)
                {
                    PlayerApp.OnTrackChange += PlayerApp_OnTrackChange;
                    PlayerApp.OnPlayStateChange += PlayerApp_OnPlayStateChange;
                    PlayerApp.OnTrackTimeChange += PlayerApp_OnTrackTimeChange;
                    PlayerApp.ListenForEvents = true;
                }
            }
        }

        //##############################################################################################################################################################################################

        private void PlayerApp_OnTrackChange(object sender, PlayerTrackChangeEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo("Track changed to \"" + e.NewTrack?.TrackName + "\" (" + e.NewTrack?.Artists[0].ArtistName + ")"));

            bool spotifyPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;
            //_spotify.Pause();
            RecorderHandle.StopRecord();
            //if(spotifyPlaying) { _spotify.Play(); }

            StartRecord();
        }

        //***********************************************************************************************************************************************************************************************************

        private void PlayerApp_OnPlayStateChange(object sender, PlayerPlayStateEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo(PlayerApp.PlayerName + " playback " + (e.Playing ? "started" : "paused")));

            PlayerPlaybackStatus status = PlayerApp.CurrentPlaybackStatus;

            if (e.Playing && status.Progress.TotalSeconds <= 0.5 && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                StartRecord();
            }
            else if (e.Playing && status.Progress.TotalSeconds > 0.5 && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                RecorderHandle.ResumeRecord();
            }
            else if (!e.Playing && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                RecorderHandle.PauseRecord();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        PlayerTrack _lastTrack = null;
        bool block_recorderStartStop = false;
        private void PlayerApp_OnTrackTimeChange(object sender, PlayerTrackTimeChangeEventArgs e)
        {
            try
            {
                PlayerPlaybackStatus status = PlayerApp.CurrentPlaybackStatus;
                if (IsRecorderArmed && status.IsPlaying && status.Progress.TotalSeconds < 0.5 && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd && _lastTrack.TrackID == status.Track.TrackID)
                {
                    if (RecorderHandle.RecordState == RecordStates.STOPPED)
                    {
                        block_recorderStartStop = true;
                        StartRecord();
                    }
                    else if (RecorderHandle.RecordState == RecordStates.RECORDING && !block_recorderStartStop)
                    {
                        RecorderHandle.StopRecord();
                        StartRecord();
                    }
                }
                else
                {
                    block_recorderStartStop = false;
                }
                _lastTrack = status?.Track;
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                logBox1.LogEvent(new LogBox.LogEventError("OnTrackTimeChange event: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void StartRecord()
        {
            PlayerApp.ListenForEvents = false;

            bool isPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;

            if (!isPlaying || !IsRecorderArmed)     //Only start a new record if music is playing and the recorder is armed
            {
                return;
            }
            
            if (!isPlaying) { return; }

            //PlayerApp.PausePlayback();
            
            RecorderHandle.TrackInfo = PlayerApp.CurrentTrack;

            if (PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                RecorderHandle.StartRecord();

                //System.Threading.Thread.Sleep(silentTimeBeforeTrack_ms);
            }

            //PlayerApp.StartPlayback();

            PlayerApp.ListenForEvents = true;
        }

    }
}

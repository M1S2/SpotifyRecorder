using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private object _recordersListLock = new object();
        private ObservableCollection<Recorder> _recorders;
        public ObservableCollection<Recorder> Recorders
        {
            get { return _recorders; }
            set { _recorders = value; OnPropertyChanged(); OnPropertyChanged("CurrentRecorder"); OnPropertyChanged("AreRecorderSettingsChanged"); }
        }
        
        public Recorder CurrentRecorder
        {
            get { return (Recorders == null || Recorders.Count == 0) ? null : Recorders.Last(); }
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

        private RecorderSettings _recSettings;
        public RecorderSettings RecSettings
        {
            get { return _recSettings; }
            set
            {
                if (_recSettings != null) { _recSettings.PropertyChanged -= RecSettingsPropertyChanged; }
                _recSettings = value;
                if (_recSettings != null) { _recSettings.PropertyChanged += RecSettingsPropertyChanged; }
                
                OnPropertyChanged();
                OnPropertyChanged("AreRecorderSettingsChanged");
            }
        }

        void RecSettingsPropertyChanged(object sender, PropertyChangedEventArgs args) { OnPropertyChanged("RecSettings"); OnPropertyChanged("AreRecorderSettingsChanged"); }

        public bool AreRecorderSettingsChanged
        {
            get
            {
                return (CurrentRecorder != null && CurrentRecorder.RecordState != RecordStates.STOPPED && !CurrentRecorder.RecorderRecSettings.Equals(RecSettings));
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
                        FileNamePrototypeCreator fileNamePrototypeCreator = new FileNamePrototypeCreator(CurrentRecorder.RecorderRecSettings.FileNamePrototype, CurrentRecorder.RecorderRecSettings.BasePath, PlayerApp.CurrentTrack?.TrackName, PlayerApp.CurrentTrack?.Artists[0].ArtistName, PlayerApp.CurrentTrack?.Album.AlbumName, CurrentRecorder.RecorderRecSettings.FileExistMode);
                        if(fileNamePrototypeCreator.ShowDialog().Value == true)
                        {
                            CurrentRecorder.RecorderRecSettings.FileNamePrototype = fileNamePrototypeCreator.FileNamePrototype;
                        }
                    });
                }
                return _openFileNamePrototypeCommand;
            }
        }

        private ICommand _resetRecSettingsCommand;
        public ICommand ResetRecSettingsCommand
        {
            get
            {
                if (_resetRecSettingsCommand == null)
                {
                    _resetRecSettingsCommand = new WindowTheme.RelayCommand(param =>
                    {
                        RecSettings = (CurrentRecorder == null ? new RecorderSettings() : (RecorderSettings)CurrentRecorder.RecorderRecSettings.Clone());
                    });
                }
                return _resetRecSettingsCommand;
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
            if (Properties.Settings.Default.RecSettings == null) { Properties.Settings.Default.RecSettings = new RecorderSettings(); }
            RecSettings = Properties.Settings.Default.RecSettings;

            PlayerApp = new SpotifyPlayer(10, this);

            await startAndConnectToPlayer();

            Recorders = new ObservableCollection<Recorder>();
            BindingOperations.EnableCollectionSynchronization(Recorders, _recordersListLock);

            /*Recorder tmpRecorder = new Recorder(RecSettings, new PlayerTrack() { TrackName = "Track1" }, _logHandle);
            Recorders.Add(tmpRecorder);
            
            tmpRecorder = new Recorder(RecSettings, new PlayerTrack() { TrackName = "Track2" }, _logHandle);
            Recorders.Add(tmpRecorder);

            tmpRecorder = new Recorder(RecSettings, new PlayerTrack() { TrackName = "Track3" }, _logHandle);
            Recorders.Add(tmpRecorder);*/

            Recorder tmpRecorder = new Recorder((RecorderSettings)RecSettings.Clone(), PlayerApp.CurrentTrack, _logHandle);
            Recorders.Add(tmpRecorder);
        }

        //***********************************************************************************************************************************************************************************************************

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.RecSettings = this.RecSettings;
            Properties.Settings.Default.Save();

            foreach(Recorder rec in Recorders) { rec?.StopRecord(); }
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

#warning Ad after normal Track doesn't fire Track changed event!!!
        private void PlayerApp_OnTrackChange(object sender, PlayerTrackChangeEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo("Track changed to \"" + e.NewTrack?.TrackName + "\" (" + e.NewTrack?.Artists[0].ArtistName + ")"));

            bool isPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;
            //PlayerApp.PausePlayback();
            CurrentRecorder?.StopRecord();
            //if(isPlaying) { PlayerApp.StartPlayback(); }

            StartRecord();
        }

        //***********************************************************************************************************************************************************************************************************

        private void PlayerApp_OnPlayStateChange(object sender, PlayerPlayStateEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo(PlayerApp.PlayerName + " playback " + (e.Playing ? "started" : "paused")));

            PlayerPlaybackStatus status = PlayerApp.CurrentPlaybackStatus;

            if (e.Playing && status.Progress.TotalSeconds <= 1 && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                StartRecord();
            }
            else if (e.Playing && status.Progress.TotalSeconds > 1 && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                CurrentRecorder?.ResumeRecord();
            }
            else if (!e.Playing && PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                CurrentRecorder?.PauseRecord();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        PlayerTrack _lastTrack = null;
        TimeSpan _lastProgress = new TimeSpan();
        private void PlayerApp_OnTrackTimeChange(object sender, PlayerTrackTimeChangeEventArgs e)
        {
            try
            {
                PlayerPlaybackStatus status = PlayerApp.CurrentPlaybackStatus;
                if(status == null) { return; }
                if (status.IsPlaying && status.Progress.TotalSeconds < 1 && status.Track != null && !status.Track.IsAd && _lastTrack.TrackID == status.Track.TrackID && _lastProgress.TotalSeconds > 2)
                {
                    StartRecord();
                }
                _lastTrack = status?.Track;
                _lastProgress = (status == null ? TimeSpan.Zero : status.Progress);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
#warning NullReference error while ad?!
                _logHandle.Report(new LogBox.LogEventError("OnTrackTimeChange event: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void StartRecord()
        {
            PlayerApp.ListenForEvents = false;

            bool isPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;
            OnPropertyChanged("AreRecorderSettingsChanged");

            if (!isPlaying || !IsRecorderArmed)     //Only start a new record if music is playing and the recorder is armed
            {
                return;
            }
            
            if (!isPlaying) { return; }

            Recorder tmpRecorder = new Recorder((RecorderSettings)RecSettings.Clone(), PlayerApp.CurrentTrack, _logHandle);
            tmpRecorder.OnRecorderPostStepsFinished += TmpRecorder_OnRecorderPostStepsFinished;
            Recorders.Add(tmpRecorder);
            
            if (PlayerApp.CurrentTrack != null && !PlayerApp.CurrentTrack.IsAd)
            {
                tmpRecorder?.StartRecord();
            }

            CleanupRecordersList();

            PlayerApp.ListenForEvents = true;
        }

        //***********************************************************************************************************************************************************************************************************

        private void TmpRecorder_OnRecorderPostStepsFinished(object sender, EventArgs e)
        {
            CleanupRecordersList();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Cleanup all recorders that are stopped. Leave the last recorder in the list.
        /// </summary>
        private void CleanupRecordersList()
        {
            List<Recorder> recordersCleanedup = Recorders.Where(r => r.RecordState != RecordStates.STOPPED || Recorders.IndexOf(r) == (Recorders.Count - 1)).ToList();

            Recorders.Clear();
            foreach (Recorder item in recordersCleanedup) { Recorders.Add(item); }
        }
    }
}

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
using SpotifyRecorder.WindowTheme;
using LogBox.LogEvents;

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

        public IProgress<LogEvent> _logHandle;

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

        private void Recorders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Recorders"); OnPropertyChanged("CurrentRecorder"); OnPropertyChanged("AreRecorderSettingsChanged");
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
                logBox1.LogEvent(new LogEventInfo("SpotifyRecorder " + (_isRecorderArmed ? "armed." : "disarmed.")));
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
                OnPropertyChanged("Recorders");
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

        private bool _isPlayerAdblockerEnabled;
        public bool IsPlayerAdblockerEnabled
        {
            get { return _isPlayerAdblockerEnabled; }
            set { _isPlayerAdblockerEnabled = value; OnPropertyChanged(); }
        }

        //##############################################################################################################################################################################################

        #region Commands

        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new WindowTheme.RelayCommand(async param =>
                    {
                        await ((App)Application.Current).StartAndConnectToPlayer(true, true);
                    });
                }
                return _connectCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _openSettingsFlyoutCommand;
        public ICommand OpenSettingsFlyoutCommand
        {
            get
            {
                if (_openSettingsFlyoutCommand == null)
                {
                    _openSettingsFlyoutCommand = new WindowTheme.RelayCommand(param =>
                    {
                        Flyout flyout = this.Flyouts.Items[0] as Flyout;
                        flyout.IsOpen = !flyout.IsOpen;
                    });
                }
                return _openSettingsFlyoutCommand;
            }
        }
        
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _openFileNamePrototypeCommand;
        public ICommand OpenFileNamePrototypeCommand
        {
            get
            {
                if (_openFileNamePrototypeCommand == null)
                {
                    _openFileNamePrototypeCommand = new WindowTheme.RelayCommand(param =>
                    {
                        FileNamePrototypeCreator fileNamePrototypeCreator = new FileNamePrototypeCreator(RecSettings.FileNamePrototype, RecSettings.BasePath, PlayerApp.CurrentTrack?.TrackName, PlayerApp.CurrentTrack?.CombinedArtistsString, PlayerApp.CurrentTrack?.Album.AlbumName, PlayerApp.CurrentPlaybackStatus?.Playlist?.PlaylistName, RecSettings.FileExistMode);
                        if(fileNamePrototypeCreator.ShowDialog().Value == true)
                        {
                            RecSettings.FileNamePrototype = fileNamePrototypeCreator.FileNamePrototype;
                        }
                    });
                }
                return _openFileNamePrototypeCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ICommand _chooseRecorderBasePathCommand;
        public ICommand ChooseRecorderBasePathCommand
        {
            get
            {
                if (_chooseRecorderBasePathCommand == null)
                {
                    _chooseRecorderBasePathCommand = new WindowTheme.RelayCommand(param =>
                    {
                        System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                        folderBrowserDialog.Description = "Please choose a recorder base path.";
                        folderBrowserDialog.SelectedPath = RecSettings.BasePath;
                        
                        if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            RecSettings.BasePath = folderBrowserDialog.SelectedPath;
                        }
                    });
                }
                return _chooseRecorderBasePathCommand;
            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

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
            _logHandle = new Progress<LogEvent>(progressValue =>
            {
                logBox1.LogEvent(progressValue);
            });

            PlayerApp = new SpotifyPlayer(10, this);
        }

        //***********************************************************************************************************************************************************************************************************

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.SetPlacementString(Properties.Settings.Default.MainWindowPlacement);
        }

        //***********************************************************************************************************************************************************************************************************

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.RecSettings == null) { Properties.Settings.Default.RecSettings = new RecorderSettings(); }
            RecSettings = Properties.Settings.Default.RecSettings;
            IsPlayerAdblockerEnabled = Properties.Settings.Default.IsPlayerAdblockerEnabled;

            _logHandle.Report(new LogEventInfo("Track \"" + PlayerApp.CurrentTrack?.TrackName + "\" (" + PlayerApp.CurrentTrack?.CombinedArtistsString + ")"));

            PlayerApp.OnTrackChange += PlayerApp_OnTrackChange;
            PlayerApp.OnPlayStateChange += PlayerApp_OnPlayStateChange;
            PlayerApp.OnTrackTimeChange += PlayerApp_OnTrackTimeChange;
            PlayerApp.OnPlayerConnectionTokenExpired += PlayerApp_OnPlayerConnectionTokenExpired;

            Recorders = new ObservableCollection<Recorder>();
            BindingOperations.EnableCollectionSynchronization(Recorders, _recordersListLock);
            Recorders.CollectionChanged += Recorders_CollectionChanged;

            //Recorder tmpRecorder = new SpotifyRecorderImplementierung((RecorderSettings)RecSettings.Clone(), PlayerApp.CurrentTrack, _logHandle);
            //Recorders.Add(tmpRecorder);
        }

        //***********************************************************************************************************************************************************************************************************

        private void MetroWindow_Closing(object sender, EventArgs e)
        {
            Properties.Settings.Default.RecSettings = this.RecSettings;
            Properties.Settings.Default.IsPlayerAdblockerEnabled = this.IsPlayerAdblockerEnabled;
            Properties.Settings.Default.MainWindowPlacement = this.GetPlacementString();
            Properties.Settings.Default.Save();

            if (Recorders != null)
            {
                foreach (Recorder rec in Recorders) { rec?.StopRecord(); }
            }
        }

        //##############################################################################################################################################################################################
        
        private async void PlayerApp_OnTrackChange(object sender, PlayerTrackChangeEventArgs e)
        {
            //#warning TESTCODE
            //PlayerApp.CurrentPlaybackStatus.IsAd = true;      //Use this for ad blocker testing

            if (PlayerApp.CurrentPlaybackStatus.IsAd) { _logHandle.Report(new LogEventInfo("Advertisement is playing")); }
            else { _logHandle.Report(new LogEventInfo("Track changed to \"" + e.NewTrack?.TrackName + "\" (" + e.NewTrack?.CombinedArtistsString + ")")); }

            bool blockAd = PlayerApp.CurrentPlaybackStatus.IsAd && IsPlayerAdblockerEnabled;

            if (blockAd || PlayerApp.IsConnectionTokenExpired)
            {
                if(PlayerApp.IsConnectionTokenExpired) { PlayerApp.IsConnectionTokenExpired = false; }

                CurrentRecorder?.StopRecord();
                bool wasPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;
                bool wasMinimized = (ProcessHelper.GetProcessWindowState(PlayerApp.PlayerName).showCmd == WindowTheme.WindowPlacement.ShowWindowStates.Minimized);
                if (wasPlaying) { PlayerApp.PausePlayback(); }

                if (blockAd)
                {
                    await Task.Delay(300);
                    await PlayerApp.ClosePlayerApplication();
                    await Task.Delay(1000);
                }

                await ((App)Application.Current).StartAndConnectToPlayer(wasMinimized);
                await Task.Delay(1500);
                if (wasPlaying)
                {
                    PlayerApp.ListenForEvents = false;

                    if (blockAd) { PlayerApp.NextTrack(); }     // after closing and reopening spotify opens with the last played track. So skip to the next track. Skipping already starts the playback.
                    else { PlayerApp.StartPlayback(); }

                    await Task.Delay(500);
                    PlayerApp.ListenForEvents = true;
                    PlayerApp.UpdateCurrentPlaybackStatus();

                    _logHandle.Report(new LogEventInfo("Track \"" + PlayerApp.CurrentTrack?.TrackName + "\" (" + PlayerApp.CurrentTrack?.CombinedArtistsString + ")"));
                    StartRecord();
                }
                else
                {
                    PlayerApp.UpdateCurrentPlaybackStatus();
                }
            }
            else
            {
                StartRecord();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void PlayerApp_OnPlayStateChange(object sender, PlayerPlayStateEventArgs e)
        {
            _logHandle.Report(new LogEventInfo(PlayerApp.PlayerName + " playback " + (e.Playing ? "started" : "paused")));

            PlayerPlaybackStatus status = PlayerApp.CurrentPlaybackStatus;

            if (e.Playing && status.Progress.TotalSeconds <= 2 && PlayerApp.CurrentTrack != null && !status.IsAd)
            {
                StartRecord();
            }
            else if (e.Playing && status.Progress.TotalSeconds > 2 && PlayerApp.CurrentTrack != null && !status.IsAd)
            {
                CurrentRecorder?.ResumeRecord();
            }
            else if (!e.Playing && PlayerApp.CurrentTrack != null && !status.IsAd)
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
                PlayerPlaybackStatus status = PlayerApp?.CurrentPlaybackStatus;
                if(status == null) { return; }
                if (status.IsPlaying && status.Progress != null && status.Progress.TotalSeconds < 2 && status.Track != null && !status.IsAd && _lastTrack != null && _lastTrack.TrackID == status.Track.TrackID && _lastProgress.TotalSeconds > 3)
                {
                    StartRecord();
                }
                _lastTrack = status?.Track;
                _lastProgress = ((status == null || status.Progress == null) ? TimeSpan.Zero : status.Progress);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                _logHandle.Report(new LogEventError("OnTrackTimeChange event: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private async void PlayerApp_OnPlayerConnectionTokenExpired(object sender, PlayerConnectionTokenExpiredEventArgs e)
        {
            _logHandle.Report(new LogEventInfo("Connection token expired (was valid for " + e.ConnectionTokenExpirationTime.TotalSeconds.ToString() + " s)."));
            if (!IsRecorderArmed && CurrentRecorder?.RecordState != RecordStates.RECORDING && CurrentRecorder?.RecordState != RecordStates.PAUSED)
            {
                PlayerApp.IsConnectionTokenExpired = false;
                bool wasMinimized = (ProcessHelper.GetProcessWindowState(PlayerApp.PlayerName).showCmd == WindowTheme.WindowPlacement.ShowWindowStates.Minimized);
                await ((App)Application.Current).StartAndConnectToPlayer(wasMinimized);
            }
            else
            {
                // Nothing to do here. Handle the token expired event in the Track changed event (when the recorder is armed or the recorder is recording).
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void StartRecord()
        {
            PlayerApp.ListenForEvents = false;
            
            bool isPlaying = PlayerApp.CurrentPlaybackStatus.IsPlaying;
            OnPropertyChanged("AreRecorderSettingsChanged");

            CurrentRecorder?.StopRecord();

            if (!isPlaying || !IsRecorderArmed)     //Only start a new record if music is playing and the recorder is armed
            {
                PlayerApp.ListenForEvents = true;
                return;
            }

            if (Recorders.Count > 0 && Recorders.Select(r => r.TrackInfo.TrackID).Contains(PlayerApp.CurrentTrack.TrackID))
            {
                PlayerApp.ListenForEvents = true;
                return;
            }
            
            Recorder tmpRecorder = new SpotifyRecorderImplementierung((RecorderSettings)RecSettings.Clone(), PlayerApp.CurrentTrack, _logHandle);
            tmpRecorder.OnRecorderPostStepsFinished += TmpRecorder_OnRecorderPostStepsFinished;
            Recorders.Add(tmpRecorder);
            
            if (PlayerApp.CurrentTrack != null && !PlayerApp.CurrentPlaybackStatus.IsAd)
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

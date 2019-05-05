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
                    }, param => { return PlayerApp?.IsConnected == false; });
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
                        if (PlayerApp.CurrentPlaybackStatus?.IsPlaying == true) { PlayerApp.PausePlayback(); }
                        else if (PlayerApp.CurrentPlaybackStatus?.IsPlaying == false) { PlayerApp.StartPlayback(); }
                    }, param => { return PlayerApp?.CurrentPlaybackStatus != null; });
                }
                return _playPauseCommand;
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
                _logHandle.Report(new LogBox.LogEventInfo(PlayerApp.IsConnected ? "Connected successfully." : "Connection failed."));

                if (PlayerApp.IsConnected)
                {
                    PlayerApp.OnTrackChange += PlayerApp_OnTrackChange;
                    PlayerApp.OnPlayStateChange += PlayerApp_OnPlayStateChange;
                    PlayerApp.ListenForEvents = true;
                }
            }
        }

        //##############################################################################################################################################################################################

        private void PlayerApp_OnTrackChange(object sender, PlayerTrackChangeEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo("Track changed to \"" + e.NewTrack?.TrackName + "\" (" + e.NewTrack?.Artists[0].ArtistName + ")"));
        }

        private void PlayerApp_OnPlayStateChange(object sender, PlayerPlayStateEventArgs e)
        {
            _logHandle.Report(new LogBox.LogEventInfo("Playback " + (e.Playing ? "started" : "paused")));
        }
    }
}

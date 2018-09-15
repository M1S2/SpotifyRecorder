using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;

namespace Spotify_Recorder
{
    public partial class SpotifyRecorder : Form
    {
        SpotifyLocalAPI _spotify = new SpotifyLocalAPI();
        Track _currentTrack;

        //***********************************************************************************************************************************************************************************************************

        private Recorder _recorder;
        private bool isRecorderArmed;
        private bool block_update = false;
        private Size windowSize;
        private int silentTimeBeforeTrack_ms = 750;        //The time between _recorder.StartRecord() and _spotify.Play() in ms

        //***********************************************************************************************************************************************************************************************************

        public SpotifyRecorder()
        {
            InitializeComponent();
        }

        private void SpotifyRecorder_Load(object sender, EventArgs e)
        {
            this.FormClosing += SpotifyRecorder_FormClosing;
            this.Resize += SpotifyRecorder_Resize;
            this.prog_track_time.MouseClick += Prog_track_time_MouseClick;
            isRecorderArmed = false;

            _spotify.OnPlayStateChange += _spotify_OnPlayStateChange;
            _spotify.OnTrackChange += _spotify_OnTrackChange;
            _spotify.OnTrackTimeChange += _spotify_OnTrackTimeChange;
            _spotify.OnVolumeChange += _spotify_OnVolumeChange;

            _recorder = new Recorder();
            _recorder.AllowedDifferenceToExpectedRecordTime = 4;
            _recorder.RecordStateChangedEvent += new Recorder.RecordStateChangedDelegate(RecorderRecordingStateChanged);
            _recorder.LogEvent += logBox1.LogEvent;

            logBox1.LogEvent(LogTypes.INFO, "SpotifyRecorder loaded.");

            StartAudioRouter();
            InitGUI();
            SpotifyConnect();

            string path = @"D:\Benutzer\V17\Dokumente\Visual Studio 2015\Projects\Spotify_Recorder\";
            //string path = @"C:\Users\masc107\Desktop\C#\Spotify_Recorder\";

            //string filename = "American Idiot.wav";
            //string filename_temp = "American Idiot_temp.wav";
            //string filename = "Cascada - Miracle - Nightcore Edit.wav";
            //string filename_temp = "Cascada - Miracle - Nightcore Edit_temp.wav";
            string filename = "Marvel Opening Fanfare.wav"; //_paused.wav";
            string filename_temp = "Marvel Opening Fanfare_paused.wav"; // _temp.wav";
            //string filename = "test_faded.wav";
            //string filename_temp = "test_UNfaded.wav";
            //string filename = "test.wav";
            //string filename_temp = "test_temp.wav";

            return;

            WAV_Compare wav_comp = new WAV_Compare(path + filename, path + filename_temp);
            wav_comp.ShowDialog();

            return;

            WAV_Visualization wav_visualization = new WAV_Visualization(path + filename, 8000, 11000, 10000);
            wav_visualization.ShowDialog();

            WaveFile file = new WaveFile(path + filename);            
            List<SilenceParts> silence = file.RemoveSilence(10, -1, 0.001);

            List<PointF> fadeIn = new List<PointF>();
            fadeIn.Add(new PointF(0, 0.05f));
            fadeIn.Add(new PointF(9, 0.05f));
            fadeIn.Add(new PointF(10, 0.06f));
            fadeIn.Add(new PointF(200, 1));

            List<PointF> fadeOut = new List<PointF>();
            fadeOut.Add(new PointF(0, 1));
            fadeOut.Add(new PointF(190, 0.06f));
            fadeOut.Add(new PointF(191, 0.05f));
            fadeOut.Add(new PointF(200, 0.05f));

            List<FadeSettings> fades = new List<FadeSettings>();
            foreach (SilenceParts sil in silence)
            {
                fades.Add(new FadeSettings(sil.New_Start_ms - 200, FadeTypes.UNDO_CUSTOM, fadeOut, AudioChannels.RIGHT_AND_LEFT));
                fades.Add(new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, fadeIn, AudioChannels.RIGHT_AND_LEFT));

                //fades.Add(new FadeSettings(sil.New_Start_ms - 200, 200, 1, 0.05f, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_LINEAR));
                //fades.Add(new FadeSettings(sil.New_Start_ms, 200, 0.05f, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_LINEAR));

                //fades.Add(new FadeSettings(sil.New_Start_ms - 200, 200, 1, 0.02f, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_HYPERBEL, 1500));
                //fades.Add(new FadeSettings(sil.New_Start_ms, 200, 0.02f, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_HYPERBEL, 1500));
            }

            file.ApplyFading(fades);
            file.SaveFile(path + filename_temp);


            //List<FadeSettings> fades = new List<FadeSettings>();

            //List<PointF> customFactors = new List<PointF>() { new PointF(0, 1), new PointF(2000, 1), new PointF(10000, 0.1f) };
            //fades.Add(new FadeSettings(0, FadeTypes.CUSTOM, customFactors, AudioChannels.RIGHT_AND_LEFT));

            //fades.Add(new FadeSettings(1000, file.Length_s * 1000 - 1000, 0, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.LINEAR));
            //////fades.Add(new FadeSettings(2000, 500, 0, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.LINEAR));
            ////////fades.Add(new FadeSettings(2000, 500, 0, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_LINEAR));
            //file.ApplyFading(fades);
            //file.SaveFile(path + filename_temp);

            //file.Normalize(AudioChannels.RIGHT_AND_LEFT);
            //file.SaveFile(path + filename_temp);

            wav_visualization = new WAV_Visualization(path + filename_temp, 8000, 11000, 10000);
            wav_visualization.ShowDialog();
        }

        //***********************************************************************************************************************************************************************************************************

        private void SpotifyRecorder_Resize(object sender, EventArgs e)
        {
            if(this.WindowState != FormWindowState.Minimized)
            {
                windowSize = this.Size;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void SpotifyRecorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_recorder != null)
            {
                _recorder.StopRecord();
            }
            
            if (windowSize.Height < this.MinimumSize.Height) { windowSize.Height = this.MinimumSize.Height; }
            if (windowSize.Width < this.MinimumSize.Width) { windowSize.Width = this.MinimumSize.Width; }

            Properties.Settings.Default.WindowSize = windowSize;
            Properties.Settings.Default.WindowLocation = this.Location;
            Properties.Settings.Default.WindowState = this.WindowState;
            Properties.Settings.Default.Save();

            ProcessHelper.StopProcess("Audio Router");          //Close the AudioRouter
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the AudioRouter and make sure that it was open before spotify was open. If the audio router couldn't be started, close the application.
        /// </summary>
        private void StartAudioRouter()
        {
            bool isAudioRouterRunning = ProcessHelper.IsProcessOpen("Audio Router");
            bool isSpotifyRunning = SpotifyLocalAPI.IsSpotifyRunning();
            bool audioRouterStartSuccess = false;

            if (!isAudioRouterRunning && !isSpotifyRunning)         //Only start the AudioRouter
            {
                audioRouterStartSuccess = ProcessHelper.StartProcess(Path.Combine(Application.StartupPath, @"Audio-router-v0.10.5\Audio Router.exe"), "/min");
                if(!audioRouterStartSuccess)
                {
                    MessageBox.Show("The Audio Router couldn't be started. The application closes now because the router is needed for correct operation.", "Audio Router not started.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else if(!isAudioRouterRunning && isSpotifyRunning)      //Close spotify, start the audio router, reopen spotify
            {
                ProcessHelper.StopProcess("Spotify");
                audioRouterStartSuccess = ProcessHelper.StartProcess(Path.Combine(Application.StartupPath, @"Audio-router-v0.10.5\Audio Router.exe"), "/min");
                if (!audioRouterStartSuccess)
                {
                    MessageBox.Show("The Audio Router couldn't be started. The application closes now because the router is needed for correct operation.", "Audio Router not started.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                //StartSpotify();
            }
            else if(isAudioRouterRunning)           //Close audio router and call StartAudioRouter again, because it's not clear what was opened first (AudioRouter or Spotify)
            {
                ProcessHelper.StopProcess("Audio Router"); 
                StartAudioRouter();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start spotify
        /// </summary>
        private void StartSpotify()
        {
#warning when opening spotify with this code snippet, the saved routings aren't applied correctly. Opening the same path manually works ?!

            //ProcessHelper.StartProcess(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Spotify\SpotifyLauncher.exe"));
            ProcessHelper.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Spotify\Spotify.exe");     //Start spotify in C:\Users\%user%\AppData\Roaming\Spotify
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Initialize all GUI elements
        /// </summary>
        private void InitGUI()
        {
            btn_arm_disarm.Enabled = false;

            windowSize = Properties.Settings.Default.WindowSize;
            if(windowSize == null || windowSize.Width < this.MinimumSize.Width || windowSize.Height < this.MinimumSize.Height)
            {
                windowSize = this.MinimumSize;
            }
            this.Size = windowSize;

            Point windowLocation = Properties.Settings.Default.WindowLocation;
            if (windowLocation != null && windowLocation.X > 0 && windowLocation.Y > 0)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = windowLocation;
            }

            this.WindowState = Properties.Settings.Default.WindowState;

            if (!Directory.Exists(Properties.Settings.Default.RecordPath)) { txt_output_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); }
            else  { txt_output_path.Text = Properties.Settings.Default.RecordPath; }

            if (!Properties.Settings.Default.RecordWAV && !Properties.Settings.Default.RecordWAV)
            {
                chk_output_wav.Checked = false;
                chk_output_mp3.Checked = true;
            }
            else
            {
                chk_output_wav.Checked = Properties.Settings.Default.RecordWAV;
                chk_output_mp3.Checked = Properties.Settings.Default.RecordMP3;
            }

            cmb_fileExistMode.Items.Clear();
            foreach(string fileExistMode in Enum.GetNames(typeof(FileExistModes)))
            {
                cmb_fileExistMode.Items.Add(fileExistMode);
            }
            cmb_fileExistMode.Text = Properties.Settings.Default.FileExistMode.ToString();

            chk_mute.Checked = Properties.Settings.Default.MuteAllApps;

            txt_filename_prototype.Text = Properties.Settings.Default.FileNamePrototype;
            txt_filename_prototype_TextChanged(null, null);
            
            SetRecordFileLabel();
            RecorderRecordingStateChanged(RecordStates.STOPPED);

            logBox1.LogEvent(LogTypes.INFO, "GUI initialized.");
        }

        //***********************************************************************************************************************************************************************************************************
        //********* S P O T I F Y   E V E N T S *********************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        private void _spotify_OnVolumeChange(object sender, VolumeChangeEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => _spotify_OnVolumeChange(sender, e)));
                return;
            }
            UpdateSpotifyInformation();
        }

        //***********************************************************************************************************************************************************************************************************

        Track _lastTrack = null;
        bool block_onTrackTimeChangeEvent = false;
        private void _spotify_OnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            if (block_onTrackTimeChangeEvent) { return; }

            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => _spotify_OnTrackTimeChange(sender, e)));
                    return;
                }
                lbl_track_time.Text = FormatTime(e.TrackTime);
                prog_track_time.Value = (int)e.TrackTime;

                StatusResponse status = _spotify.GetStatus();
                if (isRecorderArmed && _recorder.RecordState == RecordStates.STOPPED && status.Playing && status.PlayingPosition < 0.5 && _currentTrack != null && !_currentTrack.IsAd() && _lastTrack.TrackResource.Uri == status.Track.TrackResource.Uri)
                {
                    StartRecord();
                }
                _lastTrack = status?.Track;
            }
            catch(ObjectDisposedException)
            { }
            catch(Exception ex)
            {
                logBox1.LogEvent(LogTypes.ERROR, "OnTrackTimeChange event: " + ex.Message);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void _spotify_OnTrackChange(object sender, TrackChangeEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => _spotify_OnTrackChange(sender, e)));
                return;
            }

            logBox1.LogEvent(LogTypes.INFO, "Track changed to \"" + e.NewTrack.TrackResource.Name + "\" (" + e.NewTrack.ArtistResource.Name + ") ");

            bool spotifyPlaying = _spotify.GetStatus().Playing;
            _spotify.Pause();
            _recorder.StopRecord();
            if(spotifyPlaying) { _spotify.Play(); }

            StartRecord();

            //if (!isRecorderArmed) { VolumeControl.RestoreMuteSettings(); }
        }

        //***********************************************************************************************************************************************************************************************************

        bool block_onPlayStateChangeEvent = false;
        private void _spotify_OnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            if (block_onPlayStateChangeEvent) { return; }

            if (InvokeRequired)
            {
                Invoke(new Action(() => _spotify_OnPlayStateChange(sender, e)));
                return;
            }

            logBox1.LogEvent(LogTypes.INFO, "Spotify playback " + (e.Playing ? "started." : "paused."));

            StatusResponse status = _spotify.GetStatus();       

            if(e.Playing && status.PlayingPosition <= 0.5 && _currentTrack != null && !_currentTrack.IsAd())
            {
                StartRecord();
            }
            else if (e.Playing && status.PlayingPosition > 0.5 && _currentTrack != null && !_currentTrack.IsAd())
            {
                _recorder.ResumeRecord();
            }
            else if (!e.Playing && _currentTrack != null && !_currentTrack.IsAd())
            {
                _recorder.PauseRecord();
            }

            UpdateSpotifyInformation();
        }

        //***********************************************************************************************************************************************************************************************************

        private void StartRecord()
        {
            bool spotifyPlaying = _spotify.GetStatus().Playing;

            if (!spotifyPlaying || !isRecorderArmed)
            {
                UpdateTrackInformation();
                return;
            }
            
            block_onTrackTimeChangeEvent = true;
            block_onPlayStateChangeEvent = true;
            
            if (!spotifyPlaying) { return; }                        //Only start a new record if music is playing

            _spotify.Pause();
            UpdateTrackInformation();

            SetRecordFileLabel();

            if (_currentTrack != null && !_currentTrack.IsAd())
            {
                //if (chk_mute.Checked) { VolumeControl.MuteAllExcept("spotify"); }
                block_onPlayStateChangeEvent = true;
#warning PlayingPosition can't be set in this way (_spotify.GetStatus().PlayingPosition = 0)
                _spotify.GetStatus().PlayingPosition = 0;       //Play track from beginning
                _recorder.StartRecord();

                System.Threading.Thread.Sleep(silentTimeBeforeTrack_ms);
                block_onPlayStateChangeEvent = false;
            }
            
            _spotify.Play();
            
            block_onTrackTimeChangeEvent = false;
            block_onPlayStateChangeEvent = false;
        }

        //***********************************************************************************************************************************************************************************************************
        //********* F O R M   C O N T R O L S ***********************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************


        private void toolStripButton_sp_connect_Click(object sender, EventArgs e)
        {
            SpotifyConnect();
        }

        //***********************************************************************************************************************************************************************************************************

        private void btn_arm_disarm_Click(object sender, EventArgs e)
        {
            if(!isRecorderArmed && !Directory.Exists(_recorder.RecordPath))
            {
                MessageBox.Show("No valid path selected", "Path error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isRecorderArmed)
            {
                btn_arm_disarm.Text = "Arm Recorder";
                btn_arm_disarm.BackColor = Color.Lime;
            }
            else
            {
                btn_arm_disarm.Text = "Disarm Recorder";
                btn_arm_disarm.BackColor = Color.Red;
            }
            isRecorderArmed = !isRecorderArmed;
            logBox1.LogEvent(LogTypes.INFO, "SpotifyRecorder " + (isRecorderArmed ? "armed." : "disarmed."));
        }

        //***********************************************************************************************************************************************************************************************************

        private void btn_change_output_path_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;
                dialog.SelectedPath = _recorder.RecordPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _recorder.RecordPath = dialog.SelectedPath;
                }
            }
            txt_output_path.Text = _recorder.RecordPath;
        }

        private void txt_output_path_Leave(object sender, EventArgs e)
        {
            _recorder.RecordPath = txt_output_path.Text;

            txt_output_path.Text = _recorder.RecordPath;

            SetRecordFileLabel();
            Properties.Settings.Default.RecordPath = _recorder.RecordPath;
            Properties.Settings.Default.Save();
        }

        private void txt_output_path_TextChanged(object sender, EventArgs e)
        {
            if(block_update) { return; }
            if (Directory.Exists(txt_output_path.Text))
            {
                _recorder.RecordPath = txt_output_path.Text;

                block_update = true;
                txt_output_path.Text = _recorder.RecordPath;
                block_update = false;
            }
            SetRecordFileLabel();
            Properties.Settings.Default.RecordPath = _recorder.RecordPath;
            Properties.Settings.Default.Save();
        }

        //***********************************************************************************************************************************************************************************************************

        private void btn_create_file_name_prototype_Click(object sender, EventArgs e)
        {
            if (_recorder == null) { return; }

            FileNameSubFolderCreator fileNameCreator = new FileNameSubFolderCreator(txt_filename_prototype.Text, _recorder.RecordPath, _recorder.Title, _recorder.Interpret, _recorder.Album, _recorder.FileExistMode);
            if (fileNameCreator.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_filename_prototype.Text = fileNameCreator.FileNamePrototype;
            }
        }

        private void txt_filename_prototype_TextChanged(object sender, EventArgs e)
        {
            if(!FileNameSubFolderCreator.IsFileNamePrototypValid(txt_filename_prototype.Text))
            {
                txt_filename_prototype.Text = FileNameSubFolderCreator.GetValidFileNamePrototyp(txt_filename_prototype.Text);
            }
            _recorder.FileNamePrototype = txt_filename_prototype.Text;
            SetRecordFileLabel();
            Properties.Settings.Default.FileNamePrototype = _recorder.FileNamePrototype;
            Properties.Settings.Default.Save();
        }

        //***********************************************************************************************************************************************************************************************************

        private void chk_output_wav_CheckedChanged(object sender, EventArgs e)
        {
            if(block_update) { return; }
            RecordTypeChanged();
        }

        private void chk_output_mp3_CheckedChanged(object sender, EventArgs e)
        {
            if (block_update) { return; }
            RecordTypeChanged();
        }

        private void RecordTypeChanged()
        {
            if (!chk_output_wav.Checked && !chk_output_mp3.Checked)
            {
                MessageBox.Show("At least one output type must be selected.", "Output Type.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chk_output_mp3.Checked = true;
            }

            if (chk_output_wav.Checked && chk_output_mp3.Checked)
            {
                _recorder.RecordType = RecordTypes.WAV_AND_MP3;
            }
            else if(chk_output_wav.Checked && !chk_output_mp3.Checked)
            {
                _recorder.RecordType = RecordTypes.WAV;
            }
            else if(!chk_output_wav.Checked && chk_output_mp3.Checked)
            {
                _recorder.RecordType = RecordTypes.MP3;
            }

            block_update = true;
            chk_output_wav.Checked = (_recorder.RecordType == RecordTypes.WAV || _recorder.RecordType == RecordTypes.WAV_AND_MP3);      //Read back the recorder settings
            chk_output_mp3.Checked = (_recorder.RecordType == RecordTypes.MP3 || _recorder.RecordType == RecordTypes.WAV_AND_MP3);
            block_update = false;

            SetRecordFileLabel();
            Properties.Settings.Default.RecordWAV = chk_output_wav.Checked;
            Properties.Settings.Default.RecordMP3 = chk_output_mp3.Checked;
            Properties.Settings.Default.Save();
        }

        //***********************************************************************************************************************************************************************************************************

        private void chk_mute_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_mute.Checked && (_recorder.RecordState == RecordStates.RECORDING || _recorder.RecordState == RecordStates.PAUSED)) 
            { 
                //VolumeControl.MuteAllExcept("spotify"); 
            }
            else if(!chk_mute.Checked)
            {
                //VolumeControl.RestoreMuteSettings();
            }
            Properties.Settings.Default.MuteAllApps = chk_mute.Checked;
            Properties.Settings.Default.Save();
        }

        //***********************************************************************************************************************************************************************************************************

        private void cmb_fileExistMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(block_update) { return; }

            _recorder.FileExistMode = (FileExistModes)Enum.Parse(typeof(FileExistModes), cmb_fileExistMode.Text);

            block_update = true;
            cmb_fileExistMode.Text = _recorder.FileExistMode.ToString();
            block_update = false;

            SetRecordFileLabel();
            Properties.Settings.Default.FileExistMode = _recorder.FileExistMode;
        }

        //***********************************************************************************************************************************************************************************************************

        private void toolStripButton_recorder_start_manually_Click(object sender, EventArgs e)
        {
            if (_recorder.RecordState == RecordStates.STOPPED)
            {
                if (!Directory.Exists(_recorder.RecordPath))
                {
                    MessageBox.Show("No valid path selected", "Path error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //if (chk_mute.Checked) { VolumeControl.MuteAllExcept("spotify"); }
                UpdateTrackInformation();
                logBox1.LogEvent(LogTypes.INFO, "SpotifyRecorder manually started.");
                _recorder.StartRecord();
                SetRecordFileLabel();
            }
            else
            {
                MessageBox.Show("Record is already running.", "Record running.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void toolStripButton_recorder_stop_manually_Click(object sender, EventArgs e)
        {
            if (_recorder.RecordState != RecordStates.STOPPED)
            {
                DialogResult result = MessageBox.Show("Do you want to delete to short records?", "Delete to short records?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if(result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.No)
                {
                    _recorder.ExpectedRecordTime = -1;
                }
                logBox1.LogEvent(LogTypes.INFO, "SpotifyRecorder manually stopped.");
                _recorder.StopRecord();
                
                //VolumeControl.RestoreMuteSettings();
            }
        }

        //***********************************************************************************************************************************************************************************************************
        //********* C O N N E C T  /  U P D A T E   F U N C T I O N S ***********************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Connect to a running spotify client
        /// </summary>
        public void SpotifyConnect()
        {
            logBox1.LogEvent(LogTypes.INFO, "Try to connect to Spotify.");
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                //if (MessageBox.Show("Spotify isn't running. Open Spotify?", "Open Spotify?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                //{
                //    StartSpotify();
                //}
                //else
                //{
                    logBox1.LogEvent(LogTypes.WARNING, "Spotify isn't running! Please open Spotify first.");
                    return;
                //}
            }
            if (!SpotifyLocalAPI.IsSpotifyWebHelperRunning())
            {
                logBox1.LogEvent(LogTypes.WARNING, "SpotifyWebHelper isn't running!");
                MessageBox.Show("SpotifyWebHelper isn't running!", "SpotifyWebHelper not running!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool successful = false;
            try
            {
                successful = _spotify.Connect();
            }
            catch (Exception) { /* Connect not successful. See the else path of the following if-else-construct. */ }

            if (successful)
            {
                btn_arm_disarm.Enabled = true;

                logBox1.LogEvent(LogTypes.INFO, "Connection to Spotify successful.");
                toolStripButton_sp_connect.Text = "Connection to Spotify successful";
                toolStripButton_sp_connect.Enabled = false;
                btn_arm_disarm.BackColor = Color.Lime;
                UpdateSpotifyInformation();
                UpdateTrackInformation();
                _spotify.ListenForEvents = true;
                Track track = _spotify.GetStatus().Track;
                logBox1.LogEvent(LogTypes.INFO, "Track \"" + track?.TrackResource.Name + "\" (" + track?.ArtistResource.Name + ") ");
            }
            else
            {
                logBox1.LogEvent(LogTypes.WARNING, "Connection to Spotify failed.");
                if (MessageBox.Show("Couldn't connect to the spotify client. Retry?", "Spotify connection error.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SpotifyConnect();
                }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void UpdateSpotifyInformation()
        {
            StatusResponse status = _spotify.GetStatus();
            if (status == null) { return; }

            lbl_sp_isPlaying.Text = status.Playing.ToString();
            lbl_sp_volume.Text = Math.Round(status.Volume * 100, 0).ToString() + " %";
        }

        //***********************************************************************************************************************************************************************************************************

        private void UpdateTrackInformation()
        {
            try
            {
                StatusResponse status = _spotify.GetStatus();
                if (status == null || status.Track == null) { return; }

                _currentTrack = status.Track;

                if (_currentTrack != null)
                {
                    if (_currentTrack.TrackResource != null)
                    {
                        lbl_track_title.Text = _currentTrack.TrackResource.Name;
                        _recorder.Title = _currentTrack.TrackResource.Name;
                    }
                    if (_currentTrack.ArtistResource != null)
                    {
                        lbl_track_interpret.Text = _currentTrack.ArtistResource.Name;
                        _recorder.Interpret = _currentTrack.ArtistResource.Name;
                    }
                    if (_currentTrack.AlbumResource != null)
                    {
                        lbl_track_album.Text = _currentTrack.AlbumResource.Name;
                        _recorder.Album = _currentTrack.AlbumResource.Name;
                    }

                    lbl_track_duration.Text = FormatTime(_currentTrack.Length);

                    pic_album_art.Image = _currentTrack.GetAlbumArt(AlbumArtSize.Size640);
                    prog_track_time.Maximum = _currentTrack.Length;

                    _recorder.AlbumArt = _currentTrack.GetAlbumArt(AlbumArtSize.Size640);
                    _recorder.ExpectedRecordTime = _currentTrack.Length;
                }

                lbl_track_time.Text = FormatTime(status.PlayingPosition);
                prog_track_time.Value = (int)status.PlayingPosition;
                SetRecordFileLabel();
            }
            catch(Exception ex)
            {
                logBox1.LogEvent(LogTypes.ERROR, "UpdateTrackInformation: " + ex.Message);
            }
        }

        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        private void toolStripButton_sp_start_Click(object sender, EventArgs e)
        {
            _spotify.Play();
        }

        private void toolStripButton_sp_pause_Click(object sender, EventArgs e)
        {
            _spotify.Pause();
        }

        private void toolStripButton_sp_previous_Click(object sender, EventArgs e)
        {
            _spotify.Previous();
        }

        private void toolStripButton_sp_skip_Click(object sender, EventArgs e)
        {
            _spotify.Skip();
        }

        private void toolStripButton_sp_update_infos_Click(object sender, EventArgs e)
        {
            UpdateSpotifyInformation();
            UpdateTrackInformation();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Show a big album art
        /// </summary>
        private void pic_album_art_DoubleClick(object sender, EventArgs e)
        {
            string title = null;
            if(_recorder != null)
            {
                title = "AlbumArt \"" + _recorder.Title + "\" (" + _recorder.Interpret + ")";
            }
            PictureDisplay pictureDisplay = new PictureDisplay(title, pic_album_art.Image);
            pictureDisplay.Show();
        }

        //***********************************************************************************************************************************************************************************************************

        private string FormatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            string secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
            {
                secs = "0" + secs;
            }
            return mins + ":" + secs;
        }

        //***********************************************************************************************************************************************************************************************************

        private void SetRecordFileLabel()
        {
            if(_recorder == null) { return; }

            lbl_record_file_path.Text = _recorder.FilestrWAV.Replace(Path.GetExtension(_recorder.FilestrWAV), "");
            if(_recorder.RecordType == RecordTypes.WAV)
            {
               lbl_record_file_path.Text += ".wav";
            }
            else if(_recorder.RecordType == RecordTypes.MP3)
            {
                lbl_record_file_path.Text += ".mp3";
            }
            else if(_recorder.RecordType == RecordTypes.WAV_AND_MP3)
            {
                lbl_record_file_path.Text += " (.wav && .mp3)";
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Called whenever the the recording state of the _recorder changes.
        /// </summary>
        /// <param name="recordingState">current recording state</param>
        private void RecorderRecordingStateChanged(RecordStates recordingState)
        {
            lbl_isRecording.Text = recordingState.ToString();
            if(recordingState == RecordStates.RECORDING)
            {
                lbl_isRecording.Font = new Font(lbl_isRecording.Font, FontStyle.Bold);
                lbl_isRecording.ForeColor = Color.Red;
            }
            else
            {
                lbl_isRecording.Font = new Font(lbl_isRecording.Font, FontStyle.Regular);
                lbl_isRecording.ForeColor = Color.Black;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void Prog_track_time_MouseClick(object sender, MouseEventArgs e)
        {
            //decimal pos = 0;
            //pos = ((decimal)e.X / (decimal)prog_track_time.Width) * prog_track_time.Maximum;

            //if (pos >= prog_track_time.Minimum && pos <= prog_track_time.Maximum)
            //{
            //    prog_track_time.Value = (int)pos;
            //    StatusResponse _status = _spotify.GetStatus();
            //    _status.PlayingPosition = (double)pos;
            //}
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Restore the original log area height.
        /// </summary>
        private void splitContainer1_DoubleClick(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Height - 230;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Show a help form.
        /// </summary>
        private void hilfeToolStripButton_Click(object sender, EventArgs e)
        {
            AssemblyInfoHelper.FormAssemblyInfo infoForm = new AssemblyInfoHelper.FormAssemblyInfo();
            infoForm.ShowDialog();
        }
    }
}

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

using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

using Spotify_Recorder.GenericPlayer;


namespace Spotify_Recorder
{
    public partial class SpotifyRecorder : Form
    {
        //private SpotifyLocalAPI _spotify = new SpotifyLocalAPI();
        private Player _player;

        private PlayerTrack _currentTrack;
        
        private Recorder _recorder;
        private bool isRecorderArmed;
        private bool block_update = false;
        private Size windowSize;
        //private int silentTimeBeforeTrack_ms = 750;        //The time between _recorder.StartRecord() and _spotify.Play() in ms

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

            _player = new SpotifyPlayer(10);
            _player.OnPlayStateChange += _player_OnPlayStateChange;
            _player.OnTrackTimeChange += _player_OnTrackTimeChange;
            _player.OnVolumeChange += _player_OnVolumeChange;
            _player.OnTrackChange += _player_OnTrackChange;

            _recorder = new Recorder();
            _recorder.AllowedDifferenceToExpectedRecordTime = 4;
            _recorder.RecordStateChangedEvent += new Recorder.RecordStateChangedDelegate(RecorderRecordingStateChanged);
            _recorder.LogEvent += logBox1.LogEvent;

            logBox1.LogEvent(LogTypes.INFO, "SpotifyRecorder loaded.");


            //TestWaveFileFunctions.TestSpotifyDefade();

            InitGUI();
            PlayerConnect();
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

            if (!Properties.Settings.Default.RecordWAV && !Properties.Settings.Default.RecordMP3)
            {
                chk_output_wav.Checked = false;
                chk_output_mp3.Checked = true;
            }
            else
            {
                chk_output_wav.Checked = Properties.Settings.Default.RecordWAV;
                chk_output_mp3.Checked = Properties.Settings.Default.RecordMP3;
            }

            rdb_markPausedFiles_yes.Checked = Properties.Settings.Default.MarkPausedFiles;
            rdb_markPausedFiles_no.Checked = !rdb_markPausedFiles_yes.Checked;
            _recorder.MarkPausedFiles = rdb_markPausedFiles_yes.Checked;

            cmb_fileExistMode.Items.Clear();
            foreach(string fileExistMode in Enum.GetNames(typeof(FileExistModes)))
            {
                cmb_fileExistMode.Items.Add(fileExistMode);
            }
            cmb_fileExistMode.Text = Properties.Settings.Default.FileExistMode.ToString();

            txt_filename_prototype.Text = Properties.Settings.Default.FileNamePrototype;
            txt_filename_prototype_TextChanged(null, null);
            
            SetRecordFileLabel();
            RecorderRecordingStateChanged(RecordStates.STOPPED);

            logBox1.LogEvent(LogTypes.INFO, "GUI initialized.");
        }

        //***********************************************************************************************************************************************************************************************************
        //********* P L A Y E R   E V E N T S ***********************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        private void _player_OnVolumeChange(object sender, PlayerVolumeChangeEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => _player_OnVolumeChange(sender, e)));
                return;
            }
            UpdateSpotifyInformation();
        }

        //***********************************************************************************************************************************************************************************************************

        PlayerTrack _lastTrack = null;
        bool block_onTrackTimeChangeEvent = false;
        bool block_recorderStartStop = false;

        private void _player_OnTrackTimeChange(object sender, PlayerTrackTimeChangeEventArgs e)
        {
            if (block_onTrackTimeChangeEvent) { return; }

            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => _player_OnTrackTimeChange(sender, e)));
                    return;
                }
                lbl_track_time.Text = FormatTime(e.TrackTime);
                prog_track_time.Value = (int)e.TrackTime;

                PlayerPlaybackStatus status = _player.CurrentPlaybackStatus;
                if (isRecorderArmed && status.IsPlaying && status.Progress < 0.5 && _currentTrack != null && !_currentTrack.IsAd && _lastTrack.TrackID == status.Track.TrackID)
                {
                    if (_recorder.RecordState == RecordStates.STOPPED)
                    {
                        block_recorderStartStop = true;
                        StartRecord();
                    }
                    else if(_recorder.RecordState == RecordStates.RECORDING && !block_recorderStartStop)
                    {
                        _recorder.StopRecord();
                        StartRecord();
                    }
                }
                else
                {
                    block_recorderStartStop = false;
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

        private void _player_OnTrackChange(object sender, PlayerTrackChangeEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => _player_OnTrackChange(sender, e)));
                return;
            }

            logBox1.LogEvent(LogTypes.INFO, "Track changed to \"" + e.NewTrack.TrackName + "\" (" + e.NewTrack.Artists[0].ArtistName + ") ");

            bool spotifyPlaying = _player.CurrentPlaybackStatus.IsPlaying;
            //_spotify.Pause();
            _recorder.StopRecord();
            //if(spotifyPlaying) { _spotify.Play(); }

            StartRecord();
        }

        //***********************************************************************************************************************************************************************************************************

        bool block_onPlayStateChangeEvent = false;

        private void _player_OnPlayStateChange(object sender, PlayerPlayStateEventArgs e)
        {
            if (block_onPlayStateChangeEvent) { return; }

            if (InvokeRequired)
            {
                Invoke(new Action(() => _player_OnPlayStateChange(sender, e)));
                return;
            }

            logBox1.LogEvent(LogTypes.INFO, _player.PlayerName + " playback " + (e.Playing ? "started." : "paused."));

            PlayerPlaybackStatus status = _player.CurrentPlaybackStatus;       

            if(e.Playing && status.Progress <= 0.5 && _currentTrack != null && !_currentTrack.IsAd)
            {
                StartRecord();
            }
            else if (e.Playing && status.Progress > 0.5 && _currentTrack != null && !_currentTrack.IsAd)
            {
                _recorder.ResumeRecord();
            }
            else if (!e.Playing && _currentTrack != null && !_currentTrack.IsAd)
            {
                _recorder.PauseRecord();
            }

            UpdateSpotifyInformation();
        }

        //***********************************************************************************************************************************************************************************************************

        private void StartRecord()
        {
            bool isPlaying = _player.CurrentPlaybackStatus.IsPlaying;

            if (!isPlaying || !isRecorderArmed)
            {
                UpdateTrackInformation();
                return;
            }
            
            block_onTrackTimeChangeEvent = true;
            block_onPlayStateChangeEvent = true;
            
            if (!isPlaying) { return; }                        //Only start a new record if music is playing

            //_spotify.Pause();
            UpdateTrackInformation();

            SetRecordFileLabel();

            if (_currentTrack != null && !_currentTrack.IsAd)
            {
                block_onPlayStateChangeEvent = true;
//#warning PlayingPosition can't be set in this way (_spotify.GetStatus().PlayingPosition = 0)
                //_spotify.GetStatus().PlayingPosition = 0;       //Play track from beginning
                _recorder.StartRecord();

                //System.Threading.Thread.Sleep(silentTimeBeforeTrack_ms);
                block_onPlayStateChangeEvent = false;
            }
            
            //_spotify.Play();
            
            block_onTrackTimeChangeEvent = false;
            block_onPlayStateChangeEvent = false;
        }

        //***********************************************************************************************************************************************************************************************************
        //********* F O R M   C O N T R O L S ***********************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        private void toolStripButton_sp_connect_Click(object sender, EventArgs e)
        {
            PlayerConnect();
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

        private void rdb_markPausedFiles_yes_CheckedChanged(object sender, EventArgs e)
        {
            _recorder.MarkPausedFiles = rdb_markPausedFiles_yes.Checked;
            Properties.Settings.Default.MarkPausedFiles = rdb_markPausedFiles_yes.Checked;
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
            }
        }

        //***********************************************************************************************************************************************************************************************************
        //********* C O N N E C T  /  U P D A T E   F U N C T I O N S ***********************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Connect to The Player.
        /// </summary>
        public async void PlayerConnect()
        {
            logBox1.LogEvent(LogTypes.INFO, "Try to connect to " + _player.PlayerName + ".");

            _player.StartPlayerApplication();

            MessageBox.Show("Make sure that the output device is set to the virtual audio cable output (settings > advanced settings > playback device).");

            bool successful = false;
            try
            {
                successful = await _player.Connect();
            }
            catch (Exception) { /* Connect not successful. See the else path of the following if-else-construct. */ }

            toolStripDropDownButton_recorder_ctrl.Enabled = successful;
            toolStripDropDownButton_spotify_ctrl.Enabled = successful;

            if (successful)
            {
                btn_arm_disarm.Enabled = true;

                logBox1.LogEvent(LogTypes.INFO, "Connection to " + _player.PlayerName + " successful.");
                toolStripButton_sp_connect.Text = "Connection to " + _player.PlayerName + " successful";
                toolStripButton_sp_connect.Enabled = false;
                btn_arm_disarm.BackColor = Color.Lime;
                UpdateSpotifyInformation();
                UpdateTrackInformation();
                _player.ListenForEvents = true;
                PlayerTrack track = _player.CurrentTrack;
                logBox1.LogEvent(LogTypes.INFO, "Track \"" + track?.TrackName + "\" (" + track?.Artists[0].ArtistName + ") ");
            }
            else
            {
                logBox1.LogEvent(LogTypes.WARNING, "Connection to " + _player.PlayerName +  " failed.");
                if (MessageBox.Show("Couldn't connect to the " + _player.PlayerName + ". Retry?", "Player connection error.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    PlayerConnect();
                }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private void UpdateSpotifyInformation()
        {
            PlayerPlaybackStatus status = _player.CurrentPlaybackStatus;
            if (status == null) { return; }

            lbl_sp_isPlaying.Text = status.IsPlaying.ToString();
            lbl_sp_volume.Text = status.DeviceVolumePercent.ToString() + " %";
        }

        //***********************************************************************************************************************************************************************************************************

        private void UpdateTrackInformation()
        {
            try
            {
                PlayerPlaybackStatus status = _player.CurrentPlaybackStatus;
                if (status == null || status.Track == null) { return; }

                _currentTrack = status.Track;

                if (_currentTrack != null)
                {
                    lbl_track_title.Text = _currentTrack.TrackName;
                    _recorder.Title = _currentTrack.TrackName;
                    if (_currentTrack.Artists.Count > 0)
                    {
                        lbl_track_interpret.Text = _currentTrack.Artists[0].ArtistName;
                        _recorder.Interpret = _currentTrack.Artists[0].ArtistName;
                    }
                    lbl_track_album.Text = _currentTrack.Album.AlbumName;
                    _recorder.Album = _currentTrack.Album.AlbumName;


                    lbl_track_duration.Text = FormatTime(_currentTrack.Duration);

                    pic_album_art.Image = _currentTrack.Album.Images[0];
                    prog_track_time.Maximum = (int)_currentTrack.Duration;

                    _recorder.AlbumArt = _currentTrack.Album.Images[0];
                    _recorder.ExpectedRecordTime = _currentTrack.Duration;
                }

                lbl_track_time.Text = FormatTime(status.Progress);
                prog_track_time.Value = (int)status.Progress;
                SetRecordFileLabel();
            }
            catch (Exception ex)
            {
                logBox1.LogEvent(LogTypes.ERROR, "UpdateTrackInformation: " + ex.Message);
            }
        }

        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        private void toolStripButton_sp_start_Click(object sender, EventArgs e)
        {
            //_spotify.Play();
        }

        private void toolStripButton_sp_pause_Click(object sender, EventArgs e)
        {
            //_spotify.Pause();
        }

        private void toolStripButton_sp_previous_Click(object sender, EventArgs e)
        {
            //_spotify.Previous();
        }

        private void toolStripButton_sp_skip_Click(object sender, EventArgs e)
        {
            //_spotify.Skip();
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
            toolStripButton_recorder_start_manually.Enabled = (recordingState == RecordStates.STOPPED);     // Only allow to start a new record if the recorder is stopped.
            toolStripButton_recorder_stop_manually.Enabled = (recordingState != RecordStates.STOPPED);      // Allow to stop the record if the recorder isn't stopped.
            switch (recordingState)
            {
                case RecordStates.RECORDING: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Record; break;
                case RecordStates.PAUSED: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Pause; break;
                case RecordStates.STOPPED: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Stop; break;
                case RecordStates.NORMALIZING_WAV: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Normalize; break;
                case RecordStates.CONVERTING_WAV_TO_MP3: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Convert_Audio; break;
                case RecordStates.ADDING_TAGS: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Tag; break;
                case RecordStates.REMOVING_FADES: pic_recordState.Image = Spotify_Recorder.Properties.Resources.Fade; break;
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
            //AssemblyInfoHelper.FormAssemblyInfo infoForm = new AssemblyInfoHelper.FormAssemblyInfo();
            //infoForm.ShowDialog();
        }

    }
}

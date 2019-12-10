using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using CSCore;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Codecs.WAV;
using CSCore.Codecs.MP3;
using CSCore.MediaFoundation;
using CSCore.CoreAudioAPI;
using CSCore.Streams;
using CSCore.Streams.SampleConverter;
using CSCore.Codecs;
using TagLib;
using LogBox.LogEvents;

namespace SpotifyRecorder.GenericRecorder
{
    /// <summary>
    /// Class that can be used to record any sound from the soundcard
    /// </summary>
    public abstract class Recorder : INotifyPropertyChanged
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
        /// Return a list with all active capture device names
        /// </summary>
        /// <returns>List with all active capture device names</returns>
        public static List<string> GetRecordingDeviceNames()
        {
            MMDeviceEnumerator devEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection mMDevices = devEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
            List<string> deviceNames = mMDevices.Select(d => d.FriendlyName).ToList();
            deviceNames.Add("Default playback device");
            return deviceNames;
        }

        //##############################################################################################################################################################################################

        #region Recorder Properties

        private GenericPlayer.PlayerTrack _trackInfo;
        /// <summary>
        /// Track infos for the record
        /// </summary>
        public GenericPlayer.PlayerTrack TrackInfo
        {
            get { return _trackInfo; }
            set
            {
                if(_trackInfo == value) { return; }
                if (RecordState == RecordStates.STOPPED)
                {
                    _trackInfo = value;
                    OnPropertyChanged();
                    CreateFilePath();
                }
                else
                {
                    _logHandle.Report(new LogEventWarning("Record is currently running. Please stop it before changing the track info."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private RecorderSettings _recorderRecSettings;
        /// <summary>
        /// Settings for the recorder
        /// </summary>
        public RecorderSettings RecorderRecSettings
        {
            get { return _recorderRecSettings; }
            set { _recorderRecSettings = value; OnPropertyChanged(); CreateFilePath(); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private RecordStates _recordState;
        /// <summary>
        /// Current recorder state (Recording, Paused, Stopped).
        /// </summary>    
        public RecordStates RecordState  
        {
            get { return _recordState; }
            protected set { _recordState = value; OnPropertyChanged(); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private TimeSpan _allowedDifferenceToTrackDuration;
        /// <summary>
        /// The allowed difference of the record time to the expected length of the record.
        /// </summary>
        public TimeSpan AllowedDifferenceToTrackDuration
        {
            get { return _allowedDifferenceToTrackDuration; }
            set { _allowedDifferenceToTrackDuration = value; OnPropertyChanged(); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _recordFilepathWithoutExtension;
        /// <summary>
        /// Filepath created from RecordBasePath and FileNamePrototype without extension. You have to add ".wav" or ".mp3" yourself.
        /// </summary>
        public string RecordFilepathWithoutExtension
        {
            get { return _recordFilepathWithoutExtension; }
            protected set { _recordFilepathWithoutExtension = value; OnPropertyChanged(); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private bool _markPausedFiles;
        /// <summary>
        /// Mark paused files (add "_paused" to the file name)
        /// </summary>
        public bool MarkPausedFiles
        {
            get { return _markPausedFiles; }
            set { _markPausedFiles = value; OnPropertyChanged(); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private bool _wasRecordPause;
        /// <summary>
        /// true -> the record was paused at least once; false -> the record wasn't paused
        /// </summary>
        public bool WasRecordPaused
        {
            get { return _wasRecordPause; }
            set { _wasRecordPause = value; OnPropertyChanged(); }
        }

        #endregion

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Event that gets called when the recorder finished all post recording steps (normalizing, converting, ...).
        /// </summary>
        public event EventHandler OnRecorderPostStepsFinished;

        /// <summary>
        /// Event that gets called after normalizing (while stopping) and before convertion to mp3.
        /// </summary>
        public event EventHandler RecorderPostSteps;

        //***********************************************************************************************************************************************************************************************************

        protected string FileStrWAV { get { return RecordFilepathWithoutExtension + ".wav"; } }
        protected string FileStrMP3 { get { return RecordFilepathWithoutExtension + ".mp3"; } }

        protected IProgress<LogEvent> _logHandle;

        protected List<double> _recordPauseTimes_s = new List<double>();      // list with times when the record was paused

        private WasapiCapture _capture;
        private WaveWriter _wavWriter;
        private WaveFormat _wavWriterFormat;
        private double _wavWriterPositionBytes;

        private ISoundOut _silenceOut;      //This object is used to play silence during each record. Otherwise blank parts won't be recorded. "Another oddity is that WASAPI will only push data down to the render endpoint when there are active streams. When nothing is playing, there is nothing to capture." (see: https://stackoverflow.com/questions/24135557/cscore-loopback-recording-when-muted)

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the Recorder class
        /// </summary>
        /// <param name="recorderSettings">Settings for the recorder</param>
        /// <param name="trackInfo">Track info of the record</param>
        /// <param name="logHandle">Handle that is used to write log entries</param>
        public Recorder(RecorderSettings recorderSettings, GenericPlayer.PlayerTrack trackInfo, IProgress<LogEvent> logHandle)
        {
            _logHandle = logHandle;
            RecordState = RecordStates.STOPPED;
            RecorderRecSettings = recorderSettings;
            TrackInfo = trackInfo;
            AllowedDifferenceToTrackDuration = new TimeSpan(0, 0, 10);
            WasRecordPaused = false;
            MarkPausedFiles = true;
            CreateFilePath();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the Recorder class
        /// </summary>
        /// <param name="logHandle">Handle that is used to write log entries</param>
        public Recorder(IProgress<LogEvent> logHandle)
        {
            _logHandle = logHandle;
            RecordState = RecordStates.STOPPED;
            RecorderRecSettings.FileNamePrototype = FileNamePrototypeCreator.PROTOTYPSTRING_DEFAULT;
            RecorderRecSettings.BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            RecorderRecSettings.FileExistMode = RecorderFileExistModes.SKIP;
            TrackInfo = null;
            RecorderRecSettings.RecordFormat = RecordFormats.MP3;
            AllowedDifferenceToTrackDuration = new TimeSpan(0, 0, 10);
            WasRecordPaused = false;
            MarkPausedFiles = true;
            CreateFilePath();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create the file path for the wav or mp3 files based on the title, interpret...
        /// </summary>
        private void CreateFilePath()
        {
            if (TrackInfo == null) { RecordFilepathWithoutExtension = ""; }
            else
            {
                RecordFilepathWithoutExtension = FileNamePrototypeCreator.GetCompleteFileNameWithoutExtension(RecorderRecSettings.FileNamePrototype, RecorderRecSettings.BasePath, TrackInfo?.TrackName, ((TrackInfo?.Artists == null ||TrackInfo?.Artists.Count == 0) ? "" : TrackInfo?.CombinedArtistsString), TrackInfo?.Album?.AlbumName, TrackInfo?.Playlist?.PlaylistName, RecorderRecSettings.FileExistMode);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        #region Control recorder (start, stop, pause, resume)

        /// <summary>
        /// Start a new record
        /// </summary>
        public void StartRecord()
        {
            try
            {
                if (RecordState == RecordStates.RECORDING) { return; }

                if (TrackInfo == null)
                {
                    _logHandle.Report(new LogEventWarning("Record not started, because no track info exists."));
                    return;
                }
                CreateFilePath();
                if (RecorderRecSettings.FileExistMode == RecorderFileExistModes.SKIP && (System.IO.File.Exists(FileStrWAV) || System.IO.File.Exists(FileStrMP3)))
                {
                    _logHandle.Report(new LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") not started, because FileExistMode == SKIP and file already exists."));
                    return;
                }

                if (!Directory.Exists(RecorderRecSettings.BasePath))
                {
                    _logHandle.Report(new LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") not started, because RecordPath is invalid."));
                    return;
                }

                if (WasapiOut.IsSupportedOnCurrentPlatform) { _silenceOut = new WasapiOut(); }
                else { _silenceOut = new DirectSoundOut(); }
                _silenceOut.Initialize(new SilenceGenerator());
                _silenceOut.Play();         //Play silence because otherwise silent parts aren't recorded

                _capture = new WasapiLoopbackCapture();

                MMDeviceEnumerator devEnumerator = new MMDeviceEnumerator();
                MMDeviceCollection mMDevices = devEnumerator.EnumAudioEndpoints(DataFlow.All, DeviceState.All);

                MMDevice dev;
                if(RecorderRecSettings.RecorderDeviceName.ToLower().Contains("default"))
                {
                    dev = devEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                }
                else
                {
                    dev = mMDevices.Where(d => d.DeviceState == DeviceState.Active && d.FriendlyName == RecorderRecSettings.RecorderDeviceName)?.First();
                }

                if (dev == null)
                {
                    _logHandle.Report(new LogEventError("Record (\"" + TrackInfo?.TrackName + "\") not started, because device \"" + RecorderRecSettings.RecorderDeviceName + "\" wasn't found." + (RecorderRecSettings.RecorderDeviceName.Contains("CABLE Input") ? " Make sure that \"VB Cable\" is installed correctly." : "")));
                    return;
                }

                _capture.Device = dev;
                _capture.Initialize();      // Important!!! First set the capture device, then call Initialize(); otherwise audio is captured from the previous device

                SoundInSource soundInSource = new SoundInSource(_capture);
                SampleToPcm16 soundInSourcePCM = new SampleToPcm16(soundInSource.ToSampleSource());     //Used to convert _capture to Pcm16 format

                Directory.CreateDirectory(Path.GetDirectoryName(FileStrWAV));
                _wavWriterFormat = new WaveFormat(_capture.WaveFormat.SampleRate, soundInSourcePCM.WaveFormat.BitsPerSample, _capture.WaveFormat.Channels, AudioEncoding.Pcm, _capture.WaveFormat.ExtraSize);      //WAV file must be 16-bit PCM file for normalizing with normalize.exe
                _wavWriter = new WaveWriter(FileStrWAV, _wavWriterFormat);
                _wavWriterPositionBytes = 0;

                soundInSource.DataAvailable += (s, capData) =>
                {
                    if (RecordState == RecordStates.RECORDING)              //Only record when RecordState is RECORDING
                {
                        byte[] buffer = new byte[soundInSourcePCM.WaveFormat.BytesPerSecond / 2];
                        int read;

                        while ((read = soundInSourcePCM.Read(buffer, 0, buffer.Length)) > 0)        //keep reading as long as we still get some data
                    {
                            _wavWriter.Write(buffer, 0, read);          //write the read data to a file
                        _wavWriterPositionBytes += read;
                        }
                    }
                };

                _capture.Start();

                RecordState = RecordStates.RECORDING;
                _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") started."));
                WasRecordPaused = false;
            }
            catch (Exception ex)
            {
                _logHandle.Report(new LogEventError("Error while starting record: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Pause a record
        /// </summary>
        public void PauseRecord()
        {
            try
            {
                if (RecordState == RecordStates.RECORDING)
                {
                    _silenceOut.Pause();
                    RecordState = RecordStates.PAUSED;
                    _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") paused."));
                    WasRecordPaused = true;

                    double recordLength_s = (_wavWriterPositionBytes / _wavWriterFormat.BytesPerSecond);
                    _recordPauseTimes_s.Add(recordLength_s);
                }
            }
            catch (Exception ex)
            {
                _logHandle.Report(new LogEventError("Error while pausing record: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************
        
        /// <summary>
        /// Resume a record
        /// </summary>
        public void ResumeRecord()
        {
            try
            {
                if (RecordState == RecordStates.PAUSED)
                {
                    _silenceOut.Play();
                    RecordState = RecordStates.RECORDING;
                    _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") resumed."));
                }
            }
            catch (Exception ex)
            {
                _logHandle.Report(new LogEventError("Error while resuming record: " + ex.Message));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Stop a record
        /// </summary>
        public async void StopRecord()
        {
            try
            {
                if (RecordState == RecordStates.STOPPED) { return; }

                if (_capture != null)
                {
                    try
                    {
                        _capture.Stop();
                        _capture.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logHandle.Report(new LogEventError("Error while stopping record: " + ex.Message));
                    }

                    _silenceOut.Stop();
                }
                if (_silenceOut != null) { _silenceOut.Stop(); _silenceOut.Dispose(); }
                if (_wavWriter != null) { _wavWriter.Dispose(); }
                _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") stopped."));

                if (System.IO.File.Exists(FileStrWAV))      //Delete too short records
                {
                    TimeSpan wavLength = TimeSpan.Zero;

                    try
                    {
                        FileInfo fileInfo = new FileInfo(FileStrWAV);
                        if (fileInfo.Length > 44)       //"Empty" files are 44 bytes big
                        {
                            IWaveSource wavSource = new WaveFileReader(FileStrWAV);
                            wavLength = wavSource.GetLength();
                            wavSource?.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        wavLength = TimeSpan.Zero;
                        _logHandle.Report(new LogEventError("Error while stopping record: " + ex.Message));
                    }

                    if (TrackInfo?.Duration > TimeSpan.Zero && (wavLength < (TrackInfo?.Duration - AllowedDifferenceToTrackDuration) || wavLength > (TrackInfo?.Duration + AllowedDifferenceToTrackDuration)))
                    {
                        System.IO.File.Delete(FileStrWAV);
                        DirectoryManager.DeleteEmptyFolders(RecorderRecSettings.BasePath);
                        _logHandle.Report(new LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") deleted, because of wrong length (Length = " + wavLength.ToString() + " s, Expected Range = [" + (TrackInfo?.Duration - AllowedDifferenceToTrackDuration).ToString() + ", " + (TrackInfo?.Duration + AllowedDifferenceToTrackDuration).ToString() + "])."));
                        RecordState = RecordStates.STOPPED;
                        return;
                    }
                }

                if (!System.IO.File.Exists(FileStrWAV))
                {
                    RecordState = RecordStates.STOPPED;
                    return;
                }

                await Task.Run(() =>
                {
                    if (NormalizeWAVFile(FileStrWAV) == false) { RecordState = RecordStates.STOPPED; return; }
                    _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") normalized."));

                    RecorderPostSteps?.Invoke(this, new EventArgs());

                    if (RecorderRecSettings.RecordFormat == RecordFormats.WAV_AND_MP3)
                    {
                        if (ConvertWAVToMP3(FileStrWAV, FileStrMP3) == false) { RecordState = RecordStates.STOPPED; return; }
                        _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") converted to MP3."));
                    }
                    else if (RecorderRecSettings.RecordFormat == RecordFormats.MP3)
                    {
                        if (ConvertWAVToMP3(FileStrWAV, FileStrMP3) == false) { RecordState = RecordStates.STOPPED; return; }
                        if (System.IO.File.Exists(FileStrWAV)) { System.IO.File.Delete(FileStrWAV); }
                        _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") converted to MP3 and WAV deleted."));
                    }

                    AddTrackTags(FileStrWAV, TrackInfo);
                    AddTrackTags(FileStrMP3, TrackInfo);
                    _logHandle.Report(new LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") tagged."));

                    RecordState = RecordStates.STOPPED;
                    OnRecorderPostStepsFinished?.Invoke(this, new EventArgs());
                });
            }
            catch (Exception ex)
            {
                _logHandle.Report(new LogEventError("Error while stopping record: " + ex.Message));
                RecordState = RecordStates.STOPPED;
            }
        }

#endregion

        //***********************************************************************************************************************************************************************************************************

#region Convert .wav to .mp3

        //#warning Delete LAME mp3 encoder after testing the other built in solution of cscore
        /// <summary>
        /// Convert the wav file in the RecordPath to an mp3 file.
        /// This function uses the LAME mp3 encoder
        /// other option for WAV to MP3 : https://github.com/filoe/cscore/tree/master/Samples/ConvertWavToMp3
        /// </summary>
        /// <param name="filenameWAV">input file path of the .wav file to convert</param>
        /// <param name="filenameMP3">output file path of the converted .mp3 file</param>
        /// <returns>true on success, false when failed</returns>
        private bool ConvertWAVToMP3(string filenameWAV, string filenameMP3)
        {
            string lame_path = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "lame.exe";
            if (!System.IO.File.Exists(lame_path))
            {
                _logHandle.Report(new LogEventError("Conversion to MP3 failed, because lame.exe not found: " + lame_path));
                return false;
            }
            if(!System.IO.File.Exists(filenameWAV))
            {
                _logHandle.Report(new LogEventError("Conversion to MP3 failed, because WAV file doesn't exist."));
                return false;
            }

            RecordState = RecordStates.WAV_TO_MP3;
            Directory.CreateDirectory(Path.GetDirectoryName(filenameMP3));
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = lame_path,
                Arguments = "-V2 \"" + filenameWAV + "\" \"" + filenameMP3 + "\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            System.Diagnostics.Process converter = System.Diagnostics.Process.Start(processStartInfo);
            converter.WaitForExit();
            return true;
        }

        //#warning test wav to mp3 conversion
        //        /// <summary>
        //        /// Convert the wav file to an mp3 file
        //        /// </summary>
        //        /// <param name="filenameWAV">input file name of the wav file</param>
        //        /// <param name="filenameMP3">output file name of the mp3 file</param>
        //        /// <param name="deleteWAV">delete the wav file after conversion or not</param>
        //        /// see: https://github.com/filoe/cscore/blob/master/Samples/ConvertWavToMp3/Program.cs
        //        private void ConvertWAVToMP3(string filenameWAV, string filenameMP3, bool deleteWAV)
        //        {
        //            if (!System.IO.File.Exists(filenameWAV)) { return; }

        //            RecordState = RecordStates.CONVERTING_WAV_TO_MP3;
        //            IWaveSource source = CodecFactory.Instance.GetCodec(filenameWAV);
        //            MediaFoundationEncoder encoder = MediaFoundationEncoder.CreateMP3Encoder(source.WaveFormat, filenameMP3);

        //            byte[] buffer = new byte[source.WaveFormat.BytesPerSecond];
        //            int read;
        //            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
        //            {
        //                encoder.Write(buffer, 0, read);
        //            }

        //            try
        //            {
        //                encoder.Dispose();
        //                source.Dispose();
        //            }
        //            catch (Exception) { }

        //            if (deleteWAV) { System.IO.File.Delete(filenameWAV); }
        //        }

#endregion

        //***********************************************************************************************************************************************************************************************************

#region Add tags to sound file

        /// <summary>
        /// Add additional infos to the .wav and/or .mp3 file
        /// </summary>
        private void AddTrackTags(string fileName, GenericPlayer.PlayerTrack trackInfo)
        {
            Image _albumImage = null;
            if (trackInfo.Album.Images != null && trackInfo.Album.Images.Count > 0)
            {
                _albumImage = new Bitmap(trackInfo.Album.Images.First());
            }

            if (System.IO.File.Exists(fileName))
            {
                RecordState = RecordStates.ADDING_TAGS;

#warning Tagging WAV file with title with special characters (ä, ö, ü) shows some hyroglyphical characters in file (seems to be a display problem of windows explorer (using no unicode))
                TagLib.File wavFile = TagLib.File.Create(fileName);
                wavFile.Tag.Title = trackInfo.TrackName;
                wavFile.Tag.AlbumArtists = trackInfo.Artists.Select(a => a.ArtistName).ToArray();
                wavFile.Tag.Album = trackInfo.Album.AlbumName;
                try
                {
                    if (_albumImage != null) { wavFile.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(new TagLib.ByteVector((byte[])new System.Drawing.ImageConverter().ConvertTo(_albumImage, typeof(byte[])))) }; }
                }
                catch (Exception) { /* AlbumArt couldn't be set */ }

                wavFile.Save();
            }
        }

#endregion

        //***********************************************************************************************************************************************************************************************************

#region Normalize .wav file

        /// <summary>
        /// Normalize the given .wav file using the tool from Manuel Kasper (see: https://neon1.net/prog/normalizer.html)
        /// </summary>
        /// <param name="wavFileName">file path of the .wav file to normalize. This file is overwritten by the normalized file</param>
        /// <returns>true on success, false when failed</returns>
        private bool NormalizeWAVFile(string wavFileName)
        {
            return NormalizeWAVFile(wavFileName, wavFileName);
        }

        /// <summary>
        /// Normalize the given .wav file using the tool from Manuel Kasper (see: https://neon1.net/prog/normalizer.html). The input .wav file must be a 16-bit PCM file.
        /// </summary>
        /// <param name="wavFileNameOriginal">input .wav file path (16-bit PCM required)</param>
        /// <param name="wavFileNameOutput">output .wav file path</param>
        /// <returns>true on success, false when failed</returns>
        private bool NormalizeWAVFile(string wavFileNameOriginal, string wavFileNameOutput)
        {
            string normalizer_path = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + "normalize.exe";
            if (!System.IO.File.Exists(normalizer_path))
            {
                _logHandle.Report(new LogEventError("Normalizing failed, because normalize.exe not found: " + normalizer_path));
                return false;
            }
            if (!System.IO.File.Exists(wavFileNameOriginal))
            {
                _logHandle.Report(new LogEventError("Normalizing failed, because WAV file doesn't exist."));
                return false;
            }

            RecordState = RecordStates.NORMALIZING;

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo() { FileName = normalizer_path, CreateNoWindow = true, UseShellExecute = false };
            if (wavFileNameOriginal != wavFileNameOutput)
            {
                processStartInfo.Arguments = "-o \"" + wavFileNameOutput + "\" \"" + wavFileNameOriginal + "\"";        //use -o option to specify another output path    
            }
            else
            {
                processStartInfo.Arguments = "\"" + wavFileNameOriginal + "\"";
            }
            System.Diagnostics.Process normalizer = System.Diagnostics.Process.Start(processStartInfo);
            normalizer.WaitForExit();
            return true;
        }

#endregion

    }
}

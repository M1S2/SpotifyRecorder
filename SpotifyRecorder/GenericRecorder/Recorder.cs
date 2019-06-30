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

namespace SpotifyRecorder.GenericRecorder
{
    /// <summary>
    /// Class that can be used to record any sound from the soundcard
    /// </summary>
    public class Recorder : INotifyPropertyChanged
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
                    _logHandle.Report(new LogBox.LogEventWarning("Record is currently running. Please stop it before changing the track info."));
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
            private set { _recordState = value; OnPropertyChanged(); }
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
            private set { _recordFilepathWithoutExtension = value; OnPropertyChanged(); }
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

        #endregion

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Event that gets called when the recorder finished all post recording steps (normalizing, converting, ...).
        /// </summary>
        public event EventHandler OnRecorderPostStepsFinished;

        //***********************************************************************************************************************************************************************************************************

        private string FileStrWAV { get { return RecordFilepathWithoutExtension + ".wav"; } }
        private string FileStrMP3 { get { return RecordFilepathWithoutExtension + ".mp3"; } }

        private IProgress<LogBox.LogEvent> _logHandle;

        private bool _wasRecordPaused;                                      // true -> the record was paused at least once; false -> the record wasn't paused
        private List<double> _recordPauseTimes_s = new List<double>();      // list with times when the record was paused

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
        public Recorder(RecorderSettings recorderSettings, GenericPlayer.PlayerTrack trackInfo, IProgress<LogBox.LogEvent> logHandle)
        {
            _logHandle = logHandle;
            RecordState = RecordStates.STOPPED;
            RecorderRecSettings = recorderSettings;
            TrackInfo = trackInfo;
            AllowedDifferenceToTrackDuration = new TimeSpan(0, 0, 1);
            _wasRecordPaused = false;
            MarkPausedFiles = true;
            CreateFilePath();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the Recorder class
        /// </summary>
        /// <param name="logHandle">Handle that is used to write log entries</param>
        public Recorder(IProgress<LogBox.LogEvent> logHandle)
        {
            _logHandle = logHandle;
            RecordState = RecordStates.STOPPED;
            RecorderRecSettings.FileNamePrototype = FileNamePrototypeCreator.PROTOTYPSTRING_DEFAULT;
            RecorderRecSettings.BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            RecorderRecSettings.FileExistMode = RecorderFileExistModes.SKIP;
            TrackInfo = null;
            RecorderRecSettings.RecordFormat = RecordFormats.MP3;
            AllowedDifferenceToTrackDuration = new TimeSpan(0, 0, 1);
            _wasRecordPaused = false;
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
                RecordFilepathWithoutExtension = FileNamePrototypeCreator.GetCompleteFileNameWithoutExtension(RecorderRecSettings.FileNamePrototype, RecorderRecSettings.BasePath, TrackInfo?.TrackName, ((TrackInfo?.Artists == null ||TrackInfo?.Artists.Count == 0) ? "" : TrackInfo?.Artists[0].ArtistName), TrackInfo?.Album?.AlbumName, RecorderRecSettings.FileExistMode);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        #region Control recorder (start, stop, pause, resume)

        /// <summary>
        /// Start a new record
        /// </summary>
        public void StartRecord()
        {
            if(RecordState == RecordStates.RECORDING) { return; }
            if(TrackInfo == null)
            {
                _logHandle.Report(new LogBox.LogEventWarning("Record not started, because no track info exists."));
                return;
            }
            CreateFilePath();
            if(RecorderRecSettings.FileExistMode == RecorderFileExistModes.SKIP && (System.IO.File.Exists(FileStrWAV) || System.IO.File.Exists(FileStrMP3)))
            {
                _logHandle.Report(new LogBox.LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") not started, because FileExistMode == SKIP and file already exists."));
                return;
            }
            
            if(!Directory.Exists(RecorderRecSettings.BasePath))
            {
                _logHandle.Report(new LogBox.LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") not started, because RecordPath is invalid."));
                return;
            }

            if (WasapiOut.IsSupportedOnCurrentPlatform) { _silenceOut = new WasapiOut(); }
            else { _silenceOut = new DirectSoundOut(); }
            _silenceOut.Initialize(new SilenceGenerator());
            _silenceOut.Play();         //Play silence because otherwise silent parts aren't recorded

            _capture = new WasapiLoopbackCapture();

            MMDeviceEnumerator devEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection mMDevices = devEnumerator.EnumAudioEndpoints(DataFlow.All, DeviceState.All);

            MMDevice dev = mMDevices.Where(d => d.FriendlyName.StartsWith("CABLE Input"))?.First();
            if (dev == null)
            {
                _logHandle.Report(new LogBox.LogEventError("Record (\"" + TrackInfo?.TrackName + "\") not started, because device \"CABLE Input\" wasn't found. Make sure that \"VB Cable\" is installed correctly."));
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
            _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") started."));
            _wasRecordPaused = false;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Pause a record
        /// </summary>
        public void PauseRecord()
        {
            if (RecordState == RecordStates.RECORDING)
            {
                _silenceOut.Pause();
                RecordState = RecordStates.PAUSED;
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") paused."));
                _wasRecordPaused = true;

                double recordLength_s = (_wavWriterPositionBytes / _wavWriterFormat.BytesPerSecond);
                _recordPauseTimes_s.Add(recordLength_s);
            }
        }

        //***********************************************************************************************************************************************************************************************************
        
        /// <summary>
        /// Resume a record
        /// </summary>
        public void ResumeRecord()
        {
            if(RecordState == RecordStates.PAUSED)
            {
                _silenceOut.Play();
                RecordState = RecordStates.RECORDING;
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") resumed."));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Stop a record
        /// </summary>
        public async void StopRecord()
        {
            await Task.Run(() =>
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
                        _logHandle.Report(new LogBox.LogEventError("Error while stopping record: " + ex.Message));
                    }

                    _silenceOut.Stop();
                }
                if (_silenceOut != null) { _silenceOut.Stop(); _silenceOut.Dispose(); }
                if (_wavWriter != null) { _wavWriter.Dispose(); }
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") stopped."));

#warning uncomment
#if false
                if (System.IO.File.Exists(FileStrWAV))
                {
                    IWaveSource wavSource = new WaveFileReader(FileStrWAV);
                    TimeSpan wavLength = wavSource.GetLength();
                    wavSource.Dispose();
                    if (TrackInfo?.Duration > TimeSpan.Zero && (wavLength < (TrackInfo?.Duration - AllowedDifferenceToTrackDuration) || wavLength > (TrackInfo?.Duration + AllowedDifferenceToTrackDuration)))
                    {
                        System.IO.File.Delete(FileStrWAV);
                        DirectoryManager.DeleteEmptyFolders(RecorderRecSettings.BasePath);
                        RecordState = RecordStates.STOPPED;
                        _logHandle.Report(new LogBox.LogEventWarning("Record (\"" + TrackInfo?.TrackName + "\") deleted, because of wrong length (Length = " + wavLength.ToString() + " s, Expected Range = [" + (TrackInfo?.Duration - AllowedDifferenceToTrackDuration).ToString() + ", " + (TrackInfo?.Duration + AllowedDifferenceToTrackDuration).ToString() + "])."));
                        return;
                    }
                }
#endif
                if (!System.IO.File.Exists(FileStrWAV))
                {
                    RecordState = RecordStates.STOPPED;
                    return;
                }

                if(NormalizeWAVFile(FileStrWAV) == false) { return; }
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") normalized."));

#warning Remove Player specific parts (Spotify) !!!
                /*if(_wasRecordPaused && MarkPausedFiles)
                {
                    string newFilestrWAV = FilestrWAV.Remove(FilestrWAV.Length - 4, 4) + "_paused.wav";
                    RemoveSpotifyFades(FilestrWAV, newFilestrWAV);
                    System.IO.File.Delete(FilestrWAV);
                    FilestrWAV = newFilestrWAV;
                    FilestrMP3 = newFilestrWAV.Replace(".wav", ".mp3");
                    _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + Title + "\") spotify fades removed."));
                }
                else if(_wasRecordPaused && !MarkPausedFiles)
                {

                    RemoveSpotifyFades(FilestrWAV, FilestrWAV);
                    _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + Title + "\") spotify fades removed."));
                }*/

                if (RecorderRecSettings.RecordFormat == RecordFormats.WAV_AND_MP3)
                {
                    //Task t = Task.Run(() => ConvertWAVToMP3(FilestrWAV_temp, FilestrMP3, true));    //Run Conversion in new Task
                    if(ConvertWAVToMP3(FileStrWAV, FileStrMP3) == false) { return; }
                    _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") converted to MP3."));
                }
                else if (RecorderRecSettings.RecordFormat == RecordFormats.MP3)
                {
                    if(ConvertWAVToMP3(FileStrWAV, FileStrMP3) == false) { return; }
                    if (System.IO.File.Exists(FileStrWAV)) { System.IO.File.Delete(FileStrWAV); }
                    _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") converted to MP3 and WAV deleted."));
                }

                AddTrackTags(FileStrWAV, TrackInfo);
                AddTrackTags(FileStrMP3, TrackInfo);
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo?.TrackName + "\") tagged."));

                RecordState = RecordStates.STOPPED;
                OnRecorderPostStepsFinished?.Invoke(this, new EventArgs());
            });
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
                _logHandle.Report(new LogBox.LogEventError("Conversion to MP3 failed, because lame.exe not found: " + lame_path));
                return false;
            }
            if(!System.IO.File.Exists(filenameWAV))
            {
                _logHandle.Report(new LogBox.LogEventError("Conversion to MP3 failed, because WAV file doesn't exist."));
                return false;
            }

            RecordState = RecordStates.WAV_TO_MP3;
            Directory.CreateDirectory(Path.GetDirectoryName(filenameMP3));
            System.Diagnostics.Process converter = System.Diagnostics.Process.Start(lame_path, "-V2 \"" + filenameWAV + "\" \"" + filenameMP3 + "\"");
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
                _logHandle.Report(new LogBox.LogEventError("Normalizing failed, because normalize.exe not found: " + normalizer_path));
                return false;
            }
            if (!System.IO.File.Exists(wavFileNameOriginal))
            {
                _logHandle.Report(new LogBox.LogEventError("Normalizing failed, because WAV file doesn't exist."));
                return false;
            }

            RecordState = RecordStates.NORMALIZING;

            System.Diagnostics.Process normalizer;
            if (wavFileNameOriginal != wavFileNameOutput)
            {
                normalizer = System.Diagnostics.Process.Start(normalizer_path, "-o \"" + wavFileNameOutput + "\" \"" + wavFileNameOriginal + "\"");     //use -o option to specify another output path
            }
            else
            {
                normalizer = System.Diagnostics.Process.Start(normalizer_path, "\"" + wavFileNameOriginal + "\"");
            }
            normalizer.WaitForExit();
            return true;
        }

#endregion

        //***********************************************************************************************************************************************************************************************************
        /*
#region Remove spotify fades

        /// <summary>
        /// Find all places where the record was paused and remove the fade outs and fade ins that spotify applies when pausing and resuming a playback.
        /// </summary>
        /// <param name="inputWavFileName">WAV file to remove fades</param>
        /// <param name="outputWavFileName">WAV file with removed fades</param>
        private void RemoveSpotifyFades(string inputWavFileName, string outputWavFileName)
        {
            RecordState = RecordStates.REMOVING_FADES;
            
            string spotifyPauseFadeOutPointsPath = System.AppDomain.CurrentDomain.BaseDirectory + @"\SpotifyPauseFadeOutPoints.xml";
            string spotifyPlayFadeInPointsPath = System.AppDomain.CurrentDomain.BaseDirectory + @"\SpotifyPlayFadeInPoints.xml";

            WaveFile file = new WaveFile(inputWavFileName);
            List<SilenceParts> silence = file.RemoveSilence(200, -1, 0.0001);

            double allowedPauseSilenceDifference_ms = 500;
            List<FadeSettings> fades = new List<FadeSettings>();
            foreach (SilenceParts sil in silence)
            {
                List<double> validPauseTimes = _recordPauseTimes_s.Where(p => ((p * 1000) > (sil.Original_Start_ms - allowedPauseSilenceDifference_ms)) && ((p * 1000) < (sil.Original_Start_ms + allowedPauseSilenceDifference_ms))).ToList();
                if (validPauseTimes.Count() == 0)   //Only remove fades if the difference between the current silencePart and any of the pauseTimes is small enough
                {
                    continue;
                }

                FadeSettings fadeOutSettings = new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPauseFadeOutPointsPath, AudioChannels.RIGHT_AND_LEFT);
                fadeOutSettings.FadeStartTime_ms -= fadeOutSettings.FadePoints.Last().X;
                fades.Add(fadeOutSettings);
                fades.Add(new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPlayFadeInPointsPath, AudioChannels.RIGHT_AND_LEFT));
            }
            file.ApplyFading(fades);
            file.SaveFile(outputWavFileName);
        }

#endregion
    */
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
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

namespace Spotify_Recorder
{
    /// <summary>
    /// Record Types that specify in which format(s) the recorder saves the audio data
    /// </summary>
    public enum RecordTypes { WAV, MP3, WAV_AND_MP3 }

    /// <summary>
    /// Recorder states.
    /// </summary>
    public enum RecordStates { RECORDING, PAUSED, STOPPED, NORMALIZING_WAV, CONVERTING_WAV_TO_MP3, ADDING_TAGS, REMOVING_FADES }

    /// <summary>
    /// What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)
    /// </summary>
    public enum FileExistModes { SKIP, OVERRIDE, CREATENEW }

    //###########################################################################################################################################################################################################
    //###########################################################################################################################################################################################################
    //###########################################################################################################################################################################################################

    /// <summary>
    /// Class that can be used to record any sound from the soundcard
    /// </summary>
    public class Recorder
    {
        #region Recorder Properties

        private string _recordPath;
        /// <summary>
        /// Path where the records are saved
        /// </summary>
        public string RecordPath
        {
            get { return _recordPath; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    if (!Directory.Exists(value))
                    {
                        LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Invalid RecordPath selected: \"" + value + "\""));
                        _isRecordPathValid = false;
                    }
                    else
                    {
                        _recordPath = value;
                        _isRecordPathValid = true;
                        CreateFilePaths();
                    }
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the record path."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _fileNamePrototype;
        /// <summary>
        /// String that represents a filename with placeholders and without extension (for example "{RecordPath}\{Album}\{Interpret} - {Title}")
        /// </summary>
        public string FileNamePrototype
        {
            get { return _fileNamePrototype; }
            set 
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    _fileNamePrototype = FileNameSubFolderCreator.GetValidFileNamePrototyp(value);
                    CreateFilePaths();
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the file name prototype."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private FileExistModes _fileExistMode;
        /// <summary>
        /// What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)
        /// </summary>
        public FileExistModes FileExistMode
        {
            get { return _fileExistMode; }
            set
            {
                if(RecordState == RecordStates.STOPPED)
                {
                    _fileExistMode = value;
                    CreateFilePaths();
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the file exist mode."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _interpret;
        /// <summary>
        /// Record interpret
        /// </summary>
        public string Interpret
        {
            get { return _interpret; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    if (value == String.Empty)
                    {
                        _interpret = "NoInterpret";
                    }
                    else
                    {
                        _interpret = value.Replace("/", "").Replace("\\", "");
                    }
                    CreateFilePaths();
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the interpret."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _title;
        /// <summary>
        /// Record title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    if (value == String.Empty)
                    {
                        _title = "NoTitle";
                    }
                    else
                    {
                        _title = value.Replace("/", "").Replace("\\", "");
                    }
                    CreateFilePaths();
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the title."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private string _album;
        /// <summary>
        /// Record album
        /// </summary>
        public string Album
        {
            get { return _album; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    if (value == String.Empty)
                    {
                        _album = "NoAlbum";
                    }
                    else
                    {
                        _album = value.Replace("/", "").Replace("\\", "");
                    }
                    CreateFilePaths();
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the album."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private Bitmap _albumArt;
        /// <summary>
        /// Record album art
        /// </summary>
        public Bitmap AlbumArt
        {
            get { return _albumArt; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    _albumArt = value;
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the album."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private RecordTypes _recordType;
        /// <summary>
        /// Type of the record
        /// </summary>
        public RecordTypes RecordType
        {
            get { return _recordType; }
            set
            {
                if (RecordState == RecordStates.STOPPED)
                {
                    _recordType = value;
                }
                else
                {
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record is currently running. Please stop it before changing the record type."));
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private RecordStates _recordState;
        /// <summary>
        /// Current recorder state (Recording, Paused, Stopped).
        /// </summary>    
        public RecordStates RecordState  
        {
            get { return _recordState; } 
            private set
            {
                _recordState = value;
                RecordStateChangedEvent?.Invoke(_recordState);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The expected length of the record in seconds. If the record is shorter or longer than this time on stopping (+- AllowedDifferenceToExpectedRecordTime) it is deleted. Set this property to a negative time if you don't want to use it.
        /// </summary>
        public double ExpectedRecordTime { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The allowed difference of the record time to the expected length of the record in seconds.
        /// </summary>
        public double AllowedDifferenceToExpectedRecordTime { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public string FilestrWAV { get; private set; }      //Filename: "interpret - title.wav"
        public string FilestrMP3 { get; private set; }      //Filename: "interpret - title.mp3"      

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private bool _isRecordPathValid;
        private bool _wasRecordPaused;

        private WasapiCapture _capture;
        private WaveWriter _wavWriter;

        private ISoundOut _silenceOut;      //This object is used to play silence during each record. Otherwise blank parts won't be recorded. "Another oddity is that WASAPI will only push data down to the render endpoint when there are active streams. When nothing is playing, there is nothing to capture." (see: https://stackoverflow.com/questions/24135557/cscore-loopback-recording-when-muted)

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Delegate void for RecordStateChangedEvent
        /// </summary>
        /// <param name="recordingState">new RecordState</param>
        public delegate void RecordStateChangedDelegate(RecordStates recordingState);

        /// <summary>
        /// Raise whenever the RecordState changes
        /// </summary>
        public event RecordStateChangedDelegate RecordStateChangedEvent;

        /// <summary>
        /// Delegate void for LogEvent
        /// </summary>
        /// <param name="logEvent">log event</param>
        public delegate void LogDelegate(LogEvent logEvent);

        /// <summary>
        /// Raised whenever a log event occurs
        /// </summary>
        public event LogDelegate LogEvent;

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the Recorder class
        /// </summary>
        /// <param name="recordpath">Path where the records are saved</param>
        /// <param name="fileExistMode">What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)</param>
        /// <param name="interpret">Record interpret</param>
        /// <param name="title">Record title</param>
        /// <param name="album">Record album</param>
        /// <param name="recordType">Type of the record</param>
        public Recorder(string recordpath, FileExistModes fileExistMode, string interpret, string title, string album, RecordTypes recordType)
        {
            RecordState = RecordStates.STOPPED;
            FileNamePrototype = FileNameSubFolderCreator.PROTOTYPSTRING_DEFAULT;
            RecordPath = recordpath;
            FileExistMode = fileExistMode;
            Interpret = interpret;
            Title = title;
            Album = album;
            AlbumArt = null;
            RecordType = recordType;
            ExpectedRecordTime = -1;
            AllowedDifferenceToExpectedRecordTime = 1;
            _wasRecordPaused = false;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the Recorder class
        /// </summary>
        public Recorder()
        {
            RecordState = RecordStates.STOPPED;
            FileNamePrototype = FileNameSubFolderCreator.PROTOTYPSTRING_DEFAULT;
            RecordPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            FileExistMode = FileExistModes.SKIP;
            Interpret = "";
            Title = "";
            Album = "";
            AlbumArt = null;
            RecordType = RecordTypes.MP3;
            ExpectedRecordTime = -1;
            AllowedDifferenceToExpectedRecordTime = 1;
            _wasRecordPaused = false;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create the file paths for the wav and mp3 files based on the title, interpret...
        /// </summary>
        private void CreateFilePaths()
        {
            string filename = FileNameSubFolderCreator.GetCompleteFileNameWithoutExtension(FileNamePrototype, RecordPath, Title, Interpret, Album, FileExistMode);
            FilestrWAV = filename + ".wav";
            FilestrMP3 = filename + ".mp3";
        }

        //***********************************************************************************************************************************************************************************************************

        #region Control recorder (start, stop, pause, resume)

        /// <summary>
        /// Start a new record
        /// </summary>
        public void StartRecord()
        {
            if(RecordState == RecordStates.RECORDING) { return; }
            CreateFilePaths();
            if(FileExistMode == FileExistModes.SKIP && (System.IO.File.Exists(FilestrWAV) || System.IO.File.Exists(FilestrMP3)))
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record (\"" + Title + "\") not started, because FileExistMode == SKIP and file already exists."));
                return;
            }
            
            if(!_isRecordPathValid)
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Record (\"" + Title + "\") not started, because RecordPath is invalid."));
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
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Record (\"" + Title + "\") not started, because device \"CABLE Input\" wasn't found. Make sure that \"VB Cable\" is installed correctly."));
                return;
            }

            //MMDevice dev = devEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _capture.Device = dev;

            _capture.Initialize();      // Important!!! First set the capture device, then call Initialize(); otherwise audio is captured from the previous device

            SoundInSource soundInSource = new SoundInSource(_capture);
            SampleToPcm16 soundInSourcePCM = new SampleToPcm16(soundInSource.ToSampleSource());     //Used to convert _capture to Pcm16 format
            
            Directory.CreateDirectory(Path.GetDirectoryName(FilestrWAV));
            WaveFormat wavFormat = new WaveFormat(_capture.WaveFormat.SampleRate, soundInSourcePCM.WaveFormat.BitsPerSample, _capture.WaveFormat.Channels, AudioEncoding.Pcm, _capture.WaveFormat.ExtraSize);      //WAV file must be 16-bit PCM file for normalizing with normalize.exe
            _wavWriter = new WaveWriter(FilestrWAV, wavFormat); // _capture.WaveFormat);
            
            soundInSource.DataAvailable += (s, capData) =>
            {
                if (RecordState == RecordStates.RECORDING)              //Only record when RecordState is RECORDING
                {
                    byte[] buffer = new byte[soundInSourcePCM.WaveFormat.BytesPerSecond / 2];
                    int read;
                    
                    while ((read = soundInSourcePCM.Read(buffer, 0, buffer.Length)) > 0)        //keep reading as long as we still get some data
                    {
                        _wavWriter.Write(buffer, 0, read);          //write the read data to a file
                    }
                }
                
                //if (RecordState == RecordStates.RECORDING)
                //{
                //    _wavWriter.Write(capData.Data, capData.Offset, capData.ByteCount);      //save the recorded audio
                //}
            };

            _capture.Start();
            RecordState = RecordStates.RECORDING;
            LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") started."));
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
                LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") paused."));
                _wasRecordPaused = true;
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
                LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") resumed."));
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Stop a record
        /// </summary>
        public void StopRecord()
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
                    LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Error while stopping record: " + ex.Message));
                }

                _silenceOut.Stop();
            }
            if(_silenceOut != null) { _silenceOut.Stop(); _silenceOut.Dispose(); }
            if (_wavWriter != null) { _wavWriter.Dispose(); }
            LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") stopped."));

            if (System.IO.File.Exists(FilestrWAV))
            {
                IWaveSource wavSource = new WaveFileReader(FilestrWAV);
                double wavLength = wavSource.GetLength().TotalSeconds;
                wavSource.Dispose();
                if (ExpectedRecordTime > 0 && (wavLength < (ExpectedRecordTime - AllowedDifferenceToExpectedRecordTime) || wavLength > (ExpectedRecordTime + AllowedDifferenceToExpectedRecordTime)))
                {
                    System.IO.File.Delete(FilestrWAV);
                    DirectoryManager.DeleteEmptyFolders(RecordPath);
                    RecordState = RecordStates.STOPPED;
                    LogEvent?.Invoke(new LogEvent(LogTypes.WARNING, DateTime.Now, "Record (\"" + Title + "\") deleted, because of wrong length (Length = " + wavLength.ToString() + " s, Expected Range = [" + (ExpectedRecordTime - AllowedDifferenceToExpectedRecordTime).ToString() + " s, " + (ExpectedRecordTime + AllowedDifferenceToExpectedRecordTime).ToString() + " s])."));
                    return;
                }
            }

            if(!System.IO.File.Exists(FilestrWAV))
            {
                RecordState = RecordStates.STOPPED;
                return;
            }

            NormalizeWAVFile(FilestrWAV);
            LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") normalized."));

            if (_wasRecordPaused)
            {
                RemoveSpotifyFades(FilestrWAV);
                LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") spotify fades removed."));
            }

            if (RecordType == RecordTypes.WAV_AND_MP3)
            {
                //Task t = Task.Run(() => ConvertWAVToMP3(FilestrWAV_temp, FilestrMP3, true));    //Run Conversion in new Task
                ConvertWAVToMP3(FilestrWAV, FilestrMP3);
                LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") converted to MP3."));
            }
            else if(RecordType == RecordTypes.MP3)
            {
                ConvertWAVToMP3(FilestrWAV, FilestrMP3);
                if (System.IO.File.Exists(FilestrWAV)) { System.IO.File.Delete(FilestrWAV); }
                LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") converted to MP3 and WAV deleted."));
            }

            AddTrackTags(FilestrWAV, Title, Interpret, Album, AlbumArt);
            AddTrackTags(FilestrMP3, Title, Interpret, Album, AlbumArt);
            LogEvent?.Invoke(new LogEvent(LogTypes.INFO, DateTime.Now, "Record (\"" + Title + "\") tagged."));

            RecordState = RecordStates.STOPPED;
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
        private void ConvertWAVToMP3(string filenameWAV, string filenameMP3)
        {
            string lame_path = Application.StartupPath + "\\" + "lame.exe";
            if (!System.IO.File.Exists(lame_path))
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Conversion to MP3 failed, because lame.exe not found: " + lame_path));
                return;
            }
            if(!System.IO.File.Exists(filenameWAV))
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Conversion to MP3 failed, because WAV file doesn't exist."));
                return;
            }

            RecordState = RecordStates.CONVERTING_WAV_TO_MP3;
            Directory.CreateDirectory(Path.GetDirectoryName(filenameMP3));
            System.Diagnostics.Process converter = System.Diagnostics.Process.Start(lame_path, "-V2 \"" + filenameWAV + "\" \"" + filenameMP3 + "\"");
            converter.WaitForExit();
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
        private void AddTrackTags(string fileName, string title, string interpret, string album, Bitmap albumArt)
        {
            Image _albumImage = new Bitmap(100, 100);
            if (albumArt != null)
            {
                _albumImage = new Bitmap(albumArt);
            }

            if (System.IO.File.Exists(fileName))
            {
                RecordState = RecordStates.ADDING_TAGS;

                TagLib.File wavFile = TagLib.File.Create(fileName);
                wavFile.Tag.Title = title;
                wavFile.Tag.AlbumArtists = new string[] { interpret };
                wavFile.Tag.Album = album;
                try
                {
                    if (albumArt != null) { wavFile.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(new TagLib.ByteVector((byte[])new System.Drawing.ImageConverter().ConvertTo(_albumImage, typeof(byte[])))) }; }
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
        private void NormalizeWAVFile(string wavFileName)
        {
            NormalizeWAVFile(wavFileName, wavFileName);
        }

        /// <summary>
        /// Normalize the given .wav file using the tool from Manuel Kasper (see: https://neon1.net/prog/normalizer.html). The input .wav file must be a 16-bit PCM file.
        /// </summary>
        /// <param name="wavFileNameOriginal">input .wav file path (16-bit PCM required)</param>
        /// <param name="wavFileNameOutput">output .wav file path</param>
        private void NormalizeWAVFile(string wavFileNameOriginal, string wavFileNameOutput)
        {
            string normalizer_path = Application.StartupPath + "\\" + "normalize.exe";
            if (!System.IO.File.Exists(normalizer_path))
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Normalizing failed, because normalize.exe not found: " + normalizer_path));
                return;
            }
            if (!System.IO.File.Exists(wavFileNameOriginal))
            {
                LogEvent?.Invoke(new LogEvent(LogTypes.ERROR, DateTime.Now, "Normalizing failed, because WAV file doesn't exist."));
                return;
            }

            RecordState = RecordStates.NORMALIZING_WAV;

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
        }

        #endregion

        //***********************************************************************************************************************************************************************************************************

        #region Remove spotify fades

        /// <summary>
        /// Find all places where the record was paused and remove the fade outs and fade ins that spotify applies when pausing and resuming a playback.
        /// </summary>
        /// <param name="wavFileName">WAV file to remove fades</param>
        private void RemoveSpotifyFades(string wavFileName)
        {
            RecordState = RecordStates.REMOVING_FADES;
            
            string spotifyPauseFadeOutPointsPath = Application.StartupPath + @"\SpotifyPauseFadeOutPoints.xml";
            string spotifyPlayFadeInPointsPath = Application.StartupPath + @"\SpotifyPlayFadeInPoints.xml";

            WaveFile file = new WaveFile(wavFileName);
            List<SilenceParts> silence = file.RemoveSilence(10, -1, 0.001);

            List<FadeSettings> fades = new List<FadeSettings>();
            foreach (SilenceParts sil in silence)
            {
                FadeSettings fadeOutSettings = new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPauseFadeOutPointsPath, AudioChannels.RIGHT_AND_LEFT);
                fadeOutSettings.FadeStartTime_ms -= fadeOutSettings.FadePoints.Last().X;
                fades.Add(fadeOutSettings);
                fades.Add(new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPlayFadeInPointsPath, AudioChannels.RIGHT_AND_LEFT));
            }
            file.ApplyFading(fades);
            file.SaveFile(wavFileName);
        }

        #endregion
    }
}

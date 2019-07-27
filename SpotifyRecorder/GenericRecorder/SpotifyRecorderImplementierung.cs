using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyRecorder.GenericRecorder;
using SpotifyRecorder.WaveFile;

namespace SpotifyRecorder.GenericRecorder
{
    public class SpotifyRecorderImplementierung : Recorder
    {
        /// <summary>
        /// Constructor of the SpotifyRecorderImplementierung class
        /// </summary>
        /// <param name="recorderSettings">Settings for the recorder</param>
        /// <param name="trackInfo">Track info of the record</param>
        /// <param name="logHandle">Handle that is used to write log entries</param>
        public SpotifyRecorderImplementierung(RecorderSettings recorderSettings, GenericPlayer.PlayerTrack trackInfo, IProgress<LogBox.LogEvent> logHandle) : base(recorderSettings, trackInfo, logHandle)
        {
            RecorderPostSteps += SpotifyRecorderImplementierung_RecorderPostSteps;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of the SpotifyRecorderImplementierung class
        /// </summary>
        /// <param name="logHandle">Handle that is used to write log entries</param>
        public SpotifyRecorderImplementierung(IProgress<LogBox.LogEvent> logHandle) : base(logHandle)
        {
            RecorderPostSteps += SpotifyRecorderImplementierung_RecorderPostSteps;
        }

        //***********************************************************************************************************************************************************************************************************

        private void SpotifyRecorderImplementierung_RecorderPostSteps(object sender, EventArgs e)
        {
            /*if (WasRecordPaused && MarkPausedFiles)
            {
                string newFilestrWAV = RecordFilepathWithoutExtension + "_paused.wav";
                RemoveSpotifyFades(FileStrWAV, newFilestrWAV);
                System.IO.File.Delete(FileStrWAV);
                RecordFilepathWithoutExtension = newFilestrWAV.Replace(".wav", "");
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo.TrackName + "\") spotify fades removed."));
            }
            else if (WasRecordPaused && !MarkPausedFiles)
            {
                RemoveSpotifyFades(FileStrWAV, FileStrWAV);
                _logHandle.Report(new LogBox.LogEventInfo("Record (\"" + TrackInfo.TrackName + "\") spotify fades removed."));
            }*/
        }

        //***********************************************************************************************************************************************************************************************************

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

            WaveFileClass file = new WaveFileClass(inputWavFileName);
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

    }
}

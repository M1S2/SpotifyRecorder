using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Spotify_Recorder
{
    public static class TestWaveFileFunctions
    {
        public static void TestSpotifyDefade()
        {
            bool calculatePoints = false;        // true -> calculate fade points from pure sinus; false -> use calculated points on real song
            
            string path = @"D:\Benutzer\V17\Dokumente\Visual Studio 2015\Projects\Spotify_Recorder\";
            string filename, filename_temp;
            string spotifyPauseFadeOutPointsPath = Application.StartupPath + @"\SpotifyPauseFadeOutPoints.xml";
            string spotifyPlayFadeInPointsPath = Application.StartupPath + @"\SpotifyPlayFadeInPoints.xml";

            if (calculatePoints)
            {
                filename = "Mono Wave - Sine Mono Tone 440 Hz - Rms -4.56Db 20 Sec - Clear Wave.wav";
                filename_temp = "Mono Wave - Sine Mono Tone 440 Hz - Rms -4.56Db 20 Sec - Clear Wave_temp.wav";
                /* Fades Sine Mono Tone 440 Hz
                 1. FadeOut:    5475 ms - 5700 ms
                 1. FadeIn:     6130 ms - 6385 ms
                 2. FadeOut:    10390 ms - 10625 ms
                 2. FadeIn:     11068 ms - 11300 ms
                 */
            }
            else
            {
                filename = "Santiano - Brueder im Herzen.wav";
                filename_temp = "Santiano - Brueder im Herzen_temp.wav";
            }

            WAV_Visualization wav_visualization_complete = new WAV_Visualization(path + filename, -1, -1, 100000);
            wav_visualization_complete.ShowDialog();

            if (calculatePoints)
            {
                WAV_Visualization wav_visualization_fadeOut = new WAV_Visualization(path + filename, 10390, 10625, 100000, spotifyPauseFadeOutPointsPath); //5475, 5700, 100000, spotifyPauseFadeOutPointsPath);
                wav_visualization_fadeOut.ShowDialog();

                WAV_Visualization wav_visualization_fadeIn = new WAV_Visualization(path + filename, 11068, 11300, 100000, spotifyPlayFadeInPointsPath); //6130, 6385, 100000, spotifyPlayFadeInPointsPath);
                wav_visualization_fadeIn.ShowDialog();
            }

            WaveFile file = new WaveFile(path + filename);

            List<SilenceParts> silence = file.RemoveSilence(10, -1, 0.001);

            /*List<PointF> fadeOut = new List<PointF>();
            fadeOut.Add(new PointF(0, 1));
            fadeOut.Add(new PointF(75, 0.2f));
            fadeOut.Add(new PointF(100, 0.06f));
            fadeOut.Add(new PointF(200, 0.002f));

            List<PointF> fadeIn = new List<PointF>();
            fadeIn.Add(new PointF(0, 0.001f));   //0, 0.05f));
            fadeIn.Add(new PointF(50, 0.02f));   //9, 0.05f));
            fadeIn.Add(new PointF(125, 0.3f));   //10, 0.06f));
            fadeIn.Add(new PointF(150, 0.7f));
            fadeIn.Add(new PointF(200, 1));   //200, 1));
            */

            List<FadeSettings> fades = new List<FadeSettings>();
            foreach (SilenceParts sil in silence)
            {
                FadeSettings fadeOutSettings = new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPauseFadeOutPointsPath, AudioChannels.RIGHT_AND_LEFT);
                fadeOutSettings.FadeStartTime_ms -= fadeOutSettings.FadePoints.Last().X;
                fades.Add(fadeOutSettings);
                fades.Add(new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, spotifyPlayFadeInPointsPath, AudioChannels.RIGHT_AND_LEFT));


                //fades.Add(new FadeSettings(sil.New_Start_ms - 223.9585, 223.9585, 0.8, 0.7, AudioChannels.RIGHT_AND_LEFT, FadeTypes.LINEAR));
                //fades.Add(new FadeSettings(sil.New_Start_ms, 223.9585, 0.6, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.LINEAR));

                //fades.Add(new FadeSettings(sil.New_Start_ms - 200, FadeTypes.UNDO_CUSTOM, fadeOut, AudioChannels.RIGHT_AND_LEFT));
                //fades.Add(new FadeSettings(sil.New_Start_ms, FadeTypes.UNDO_CUSTOM, fadeIn, AudioChannels.RIGHT_AND_LEFT));

                //fades.Add(new FadeSettings(sil.New_Start_ms - 200, 200, 1, 0.05f, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_LINEAR));
                //fades.Add(new FadeSettings(sil.New_Start_ms, 200, 0.05f, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_LINEAR));

                //fades.Add(new FadeSettings(sil.New_Start_ms - 200, 200, 1, 0.02f, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_HYPERBEL, 100));
                //fades.Add(new FadeSettings(sil.New_Start_ms, 200, 0.02f, 1, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_HYPERBEL, 100));
            }

            //fades.Add(new FadeSettings(2000, 229.6875, 0.987091064, 0.00164794922, AudioChannels.RIGHT_AND_LEFT, FadeTypes.HYPERBEL, 100));
            //fades.Add(new FadeSettings(2000, 230, 0.987091064, 0.00164794922, AudioChannels.RIGHT_AND_LEFT, FadeTypes.UNDO_HYPERBEL, 100));

            file.ApplyFading(fades);
            file.SaveFile(path + filename_temp);


            if (calculatePoints)
            {
                WAV_Visualization wav_visualization_fadeOut_Undone = new WAV_Visualization(path + filename_temp, 10390 - silence[0].Original_Length_ms - 10, 10625 - silence[0].Original_Length_ms + 10, 100000);
                wav_visualization_fadeOut_Undone.ShowDialog();

                WAV_Visualization wav_visualization_fadeIn_Undone = new WAV_Visualization(path + filename_temp, 6130 - silence[0].Original_Length_ms - 10, 6385 - silence[0].Original_Length_ms + 10, 100000);
                wav_visualization_fadeIn_Undone.ShowDialog();
            }

            WAV_Visualization wav_visualization_complete_AfterFading = new WAV_Visualization(path + filename_temp, -1, 17000, 100000);
            wav_visualization_complete_AfterFading.ShowDialog();
        }

    }
}

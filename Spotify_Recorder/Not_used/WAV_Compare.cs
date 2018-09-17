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

namespace Spotify_Recorder
{
    public partial class WAV_Compare : Form
    {
        private WaveFile wavFile1, wavFile2;
        private List<AudioSample> samples1, samples2;
        private string diffFilePath;

        public WAV_Compare(string file1, string file2)
        {
            InitializeComponent();

            wavFile1 = new WaveFile(file1);
            wavFile2 = new WaveFile(file2);

            InitWithFiles();
        }

        public WAV_Compare(WaveFile waveFile1, WaveFile waveFile2)
        {
            InitializeComponent();

            wavFile1 = waveFile1;
            wavFile2 = waveFile2;

            InitWithFiles();
        }

        private void InitWithFiles()
        {
            wavFile1.Samples.Clear();
            wavFile2.Samples.Clear();
            wavFile1.Samples.Add(new AudioSample(0.1f, 0, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(0, 0.5, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(0.1f, 1, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(0.15f, 1.5, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(-0.2f, 2, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(-0.5f, 2.5, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(0.5f, 3, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(-0.5f, 3.5, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(-0.15f, 4, AudioChannels.LEFT));
            wavFile1.Samples.Add(new AudioSample(0.1f, 4.5, AudioChannels.LEFT));

            wavFile2.Samples.Add(new AudioSample(0.2f, 0, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(0, 0.5, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(-0.4f, 1, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(0.5f, 1.5, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(0.1f, 2, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(-0.3f, 2.5, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(0.4f, 3, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(-0.7f, 3.5, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(0.3f, 4, AudioChannels.LEFT));
            wavFile2.Samples.Add(new AudioSample(-0.6f, 4.5, AudioChannels.LEFT));

            txt_file1.Text = wavFile1.Filepath;
            txt_file2.Text = wavFile2.Filepath;

            diffFilePath = Path.GetDirectoryName(wavFile1.Filepath) + "/differenceFadeSettings.txt";

            samples1 = WAV_Visualization.GroupSamples(wavFile1.GetChannelSamples(AudioChannels.LEFT), 100000);
            samples2 = WAV_Visualization.GroupSamples(wavFile2.GetChannelSamples(AudioChannels.LEFT), 100000);

            ShowSamples(0, samples1);
            ShowSamples(1, samples2);
        }

        private void btn_compare_Click(object sender, EventArgs e)
        {
            CompareSamples(wavFile1.GetChannelSamples(AudioChannels.LEFT), wavFile2.GetChannelSamples(AudioChannels.LEFT), (int)num_shift.Value, diffFilePath);
        }

        private void btn_undoFade_Click(object sender, EventArgs e)
        {
            UndoFade(wavFile2, diffFilePath, 0.5f * (int)num_shift.Value); //2617);
        }

        private void num_shift_ValueChanged(object sender, EventArgs e)
        {
            samples2 = (List<AudioSample>)wavFile2.Samples.Clone();

            ShiftSamples(samples2, (int)num_shift.Value);
            ShowSamples(1, samples2);
        }

        private void ShiftSamples(List<AudioSample> samples, int shiftSamples)
        {
            if(shiftSamples <= 0) { return; }

            double timeBetweenSamples = samples[1].Time_ms - samples[0].Time_ms;
            double samplesEndTime = samples.Last().Time_ms;

            double shiftTime = 0;
            List<AudioSample> samplesInsert = new List<AudioSample>();
            for (int i = 0; i < shiftSamples; i++)
            {
                shiftTime = i * timeBetweenSamples;
                
                samplesInsert.Add(new AudioSample(1, shiftTime, AudioChannels.RIGHT_AND_LEFT));
                //samplesInsert.Add(new AudioSample(1, samplesEndTime + i * timeBetweenSamples, AudioChannels.RIGHT_AND_LEFT));
            }
            shiftTime += timeBetweenSamples;

            samples.InsertRange(0, samplesInsert);

            for (int i = shiftSamples; i < samples.Count; i++)
            {
                samples[i].Time_ms += shiftTime;
            }
        }

        private void FillSamples(List<AudioSample> samples, int fillSamples, float fillValue)
        {
            if (fillSamples == 0) { return; }

            double timeBetweenSamples = samples[1].Time_ms - samples[0].Time_ms;
            double samplesEndTime = samples.Last().Time_ms;
            
            for (int i = 1; i <= fillSamples; i++)
            {
                samples.Add(new AudioSample(fillValue, samplesEndTime + i * timeBetweenSamples, AudioChannels.RIGHT_AND_LEFT));
            }
        }

        /// <summary>
        /// Compare the two lists of samples and save the factors of the different parts to the file in the given path
        /// </summary>
        /// <param name="samp1">list 1 to compare</param>
        /// <param name="samp2">list 2 to compare</param>
        /// <param name="shiftSamples">number of samples to shift the samples against each other</param>
        /// <param name="differenceFactorsPath">path to an XML-file where to save the factors of the different parts</param>
        private void CompareSamples(List<AudioSample> samp1, List<AudioSample> samp2, int shiftSamples, string differenceFactorsPath)
        {
            List<AudioSample> samples1 = samp1.Clone();
            List<AudioSample> samples2 = samp2.Clone();

            ShiftSamples(samples2, shiftSamples);
            FillSamples(samples1, samples2.Count - samples1.Count, 1);

            ShowSamples(0, samples1);
            ShowSamples(1, samples2);

            if (samples1.Count == samples2.Count)
            {
                List<PointF> differenceFactors = new List<PointF>();
                for (int i = 0; i < samples1.Count; i++)
                {
                    float diffValue = samples2[i].Value / samples1[i].Value;
                    if (float.IsNaN(diffValue)) { diffValue = 1; }
                    differenceFactors.Add(new PointF((float)samples1[i].Time_ms, diffValue));
                }

                ShowSamples(2, PointFToSamples(differenceFactors));

                differenceFactors = SamplesToPointF(WAV_Visualization.GroupSamples(PointFToSamples(differenceFactors), 100));

                List<PointF> filteredDifferenceFactors = new List<PointF>(differenceFactors);
                //float firstTime = float.MinValue;
                //for (int i = 0; i < differenceFactors.Count; i++)
                //{
                //    if (Math.Abs(differenceFactors[i].Y) < 0.95 || Math.Abs(differenceFactors[i].Y) > 1.05)
                //    {
                //        if (firstTime == float.MinValue) { firstTime = differenceFactors[i].X; }
                //        filteredDifferenceFactors.Add(new PointF(differenceFactors[i].X - firstTime, differenceFactors[i].Y));
                //    }
                //}

                ShowSamples(3, PointFToSamples(filteredDifferenceFactors));
                FadeSettings customFadeSettings = new FadeSettings(0, FadeTypes.CUSTOM, filteredDifferenceFactors, AudioChannels.RIGHT_AND_LEFT);
                customFadeSettings.SaveFadePoints(differenceFactorsPath);
            }
            else
            {
                MessageBox.Show("Files haven't the same length!");
            }
        }

        /// <summary>
        /// Apply a UNDO_CUSTOM fade to the given waveFile with the FadePoints in the XML-file in the differenceFactorsPath
        /// </summary>
        /// <param name="waveFile">file to unfade</param>
        /// <param name="differenceFactorsPath">path to the XML-file with the FadePoints</param>
        /// <param name="beginTime">begin time for the fade</param>
        private void UndoFade(WaveFile waveFileIn, string differenceFactorsPath, float beginTime)
        {
            WaveFile waveFile = (WaveFile)waveFileIn.Clone();

            FadeSettings customFadeSettings = new FadeSettings(beginTime, FadeTypes.UNDO_CUSTOM, differenceFactorsPath, AudioChannels.RIGHT_AND_LEFT);
            waveFile.ApplyFading(new List<FadeSettings>() { customFadeSettings });

            List<PointF> equalizerPoints = new List<PointF>();
            foreach(PointF p in customFadeSettings.FadePoints)
            {
                equalizerPoints.Add(new PointF(p.X, 1 / p.Y));
            }

            ShowSamples(4, PointFToSamples(equalizerPoints));

            foreach (AudioSample sample in waveFile.Samples)
            {
                if (sample.Value > 1) { sample.Value = 1; }
                else if (sample.Value < -1) { sample.Value = -1; }
            }

            ShowSamples(5, waveFile.Samples);
        }

        /// <summary>
        /// Convert list of samples to list of points
        /// </summary>
        /// <param name="samples">list of samples to convert</param>
        /// <returns>list of points</returns>
        private List<PointF> SamplesToPointF(List<AudioSample> samples)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < samples.Count; i++)
            {
                points.Add(new PointF((float)samples[i].Time_ms, samples[i].Value));
            }
            return points;
        }

        /// <summary>
        /// Convert list of points to list of samples
        /// </summary>
        /// <param name="points">list of points to convert</param>
        /// <returns>list of samples</returns>
        private List<AudioSample> PointFToSamples(List<PointF> points)
        {
            List<AudioSample> samples = new List<AudioSample>();
            for (int i = 0; i < points.Count; i++)
            {
                samples.Add(new AudioSample(points[i].Y, (float)points[i].X, AudioChannels.RIGHT_AND_LEFT));
            }
            return samples;
        }

        /// <summary>
        /// Show the given list of samples in the chart1
        /// </summary>
        /// <param name="seriesNumber">series number of the chart</param>
        /// <param name="points">samples to show</param>
        private void ShowSamples(int seriesNumber, List<AudioSample> samples)
        {
            samples = WAV_Visualization.GroupSamples(samples, 10000);

            chart1.Series[seriesNumber].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;

            chart1.Series[seriesNumber].Points.Clear();
            for (int i = 0; i < samples.Count; i++)
            {
                if (float.IsPositiveInfinity(samples[i].Value))
                {
                    chart1.Series[seriesNumber].Points.AddXY(samples[i].Time_ms, 1);
                }
                else if (float.IsNegativeInfinity(samples[i].Value))
                {
                    chart1.Series[seriesNumber].Points.AddXY(samples[i].Time_ms, -1);
                }
                else
                {
                    chart1.Series[seriesNumber].Points.AddXY(samples[i].Time_ms, samples[i].Value);
                }
            }
        }

    }
}

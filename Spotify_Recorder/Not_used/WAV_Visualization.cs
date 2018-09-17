using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCore;
using CSCore.Codecs.WAV;

namespace Spotify_Recorder
{
    public partial class WAV_Visualization : Form
    {
        public string Filepath { get; private set; }
        public double BeginTime_ms { get; private set; }
        public double EndTime_ms { get; private set; }
        public long NumberOfSamplesAfterGrouping { get; private set; }
        public WaveFile WavFile { get; private set; }

        //##########################################################################################################################################################################################################

        public WAV_Visualization(string filepath)
        {
            InitializeComponent();

            WaveFile wavFile = new WaveFile(filepath);
            Init(wavFile, -1, -1, -1);
        }

        public WAV_Visualization(WaveFile wavFile)
        {
            InitializeComponent();
            Init(wavFile, -1, -1, -1);
        }

        public WAV_Visualization(string filepath, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping)
        {
            InitializeComponent();

            WaveFile wavFile = new WaveFile(filepath);
            Init(wavFile, beginTime_ms, endTime_ms, numberSamplesAfterGrouping);
        }

        public WAV_Visualization(WaveFile wavFile, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping)
        {
            InitializeComponent();
            Init(wavFile, beginTime_ms, endTime_ms, numberSamplesAfterGrouping);
        }

        //##########################################################################################################################################################################################################

        private void Init(WaveFile wavFile, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping)
        {
            WavFile = wavFile;
            Filepath = (wavFile == null ? "" : wavFile.Filepath);
            BeginTime_ms = beginTime_ms;
            EndTime_ms = endTime_ms;
            NumberOfSamplesAfterGrouping = numberSamplesAfterGrouping;
        }

        //##########################################################################################################################################################################################################
        
        private void WAV_Visualization_Load(object sender, EventArgs e)
        {
            this.Text = "WAV Visualization: " + Filepath;

            List<AudioSample> samples_L = new List<AudioSample>();
            List<AudioSample> samples_R = new List<AudioSample>();

            if (WavFile == null) { return; }

            samples_L = WavFile.GetChannelSamples(AudioChannels.LEFT, BeginTime_ms, EndTime_ms);
            samples_R = WavFile.GetChannelSamples(AudioChannels.RIGHT, BeginTime_ms, EndTime_ms);

            samples_L = GroupSamples(samples_L, NumberOfSamplesAfterGrouping);
            samples_R = GroupSamples(samples_R, NumberOfSamplesAfterGrouping);

            foreach (AudioSample s in samples_L)
            {
                chart1.Series["Series_Left"].Points.AddXY(s.Time_ms, s.Value);
            }
            foreach (AudioSample s in samples_R)
            {
                chart1.Series["Series_Right"].Points.AddXY(s.Time_ms, s.Value);
            }
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Group the samples. The returned list contains numberOfRequiredSamples that approximate the real shape of the given samples
        /// </summary>
        /// <param name="samples">list of samples to group</param>
        /// <param name="numberOfRequiredSamples">number of required samples</param>
        /// <returns>list of grouped samples</returns>
        public static List<AudioSample> GroupSamples(List<AudioSample> samples, long numberOfRequiredSamples)
        {
            if (numberOfRequiredSamples < 0) { return samples; }

            List<AudioSample> grouped_samples = new List<AudioSample>();

            if (samples != null && samples.Count > 0)
            {
                if (numberOfRequiredSamples > samples.Count) { return samples; }

                long group_factor = samples.Count / numberOfRequiredSamples;

                List<AudioSample> temp_samples = new List<AudioSample>();
                foreach (AudioSample s in samples)
                {
                    if (temp_samples.Count < group_factor)       //collect samples (group_factor times)
                    {
                        temp_samples.Add(s);
                    }
                    else
                    {
                        double time_sum = 0;
                        float value_max_abs = 0;
                        foreach (AudioSample temp_s in temp_samples)
                        {
                            time_sum += temp_s.Time_ms;
                            if (Math.Abs(temp_s.Value) > Math.Abs(value_max_abs)) { value_max_abs = temp_s.Value; }
                        }
                        grouped_samples.Add(new AudioSample(value_max_abs, time_sum / temp_samples.Count, samples.First().Channel));
                        temp_samples.Clear();
                    }
                }
            }

            return grouped_samples;
        }



        //##########################################################################################################################################################################################################

        /*WaveFormat format = new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat);                                        //Create 5 second long wav file
        WaveWriter w = new WaveWriter(Filepath, format);
        long nSamples = (format.MillisecondsToBytes(5000) / format.BytesPerSample) / format.Channels;
        for (int i = 0; i < nSamples; i++)
        {
            long time = format.BytesToMilliseconds(i * format.Channels * format.BytesPerSample);
            if ((time >= 1000 && time <= 2000) || (time >= 3000 && time <= 3500))
            {
                w.WriteSample(0);
                w.WriteSample(0);
            }
            else
            {
                w.WriteSample((float)Math.Pow(-1, i));
                w.WriteSample((float)Math.Pow(-1, i));
            }
        }
        w.Dispose();*/
    }
}

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
        public string EnvelopeSaveFilePath { get; private set; }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Display the WAV file in the given path. The whole file is displayed and no envelope points are saved.
        /// </summary>
        /// <param name="filepath">Path of the WAV file to display</param>
        public WAV_Visualization(string filepath)
        {
            InitializeComponent();

            WaveFile wavFile = new WaveFile(filepath);
            Init(wavFile, -1, -1, -1, "");
        }

        /// <summary>
        /// Display the WAV file. The whole file is displayed and no envelope points are saved.
        /// </summary>
        /// <param name="wavFile">WAV file to display</param>
        public WAV_Visualization(WaveFile wavFile)
        {
            InitializeComponent();
            Init(wavFile, -1, -1, -1, "");
        }

        /// <summary>
        /// Display the WAV file in the given path. You can limit the time that is displayed. You can also save the envelope points of the painted curve portion to an XML file.
        /// </summary>
        /// <param name="filepath">Path of the WAV file to display</param>
        /// <param name="beginTime_ms">Start time in ms of the displayed portion</param>
        /// <param name="endTime_ms">End time in ms of the displayed portion</param>
        /// <param name="numberSamplesAfterGrouping">Number of samples after grouping the original samples. Grouping is used to reduce the number of samples that are displayed</param>
        /// <param name="envelopeSaveFilePath">If you want to save the points that describe the envelope of the painted curve portion to a file, use this parameter to specify the file path for that XML file. Otherwise set this to ""</param>
        public WAV_Visualization(string filepath, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping, string envelopeSaveFilePath = "")
        {
            InitializeComponent();
            
            WaveFile wavFile = new WaveFile(filepath);
            Init(wavFile, beginTime_ms, endTime_ms, numberSamplesAfterGrouping, envelopeSaveFilePath);
        }

        /// <summary>
        /// Display the WAV file. You can limit the time that is displayed. You can also save the envelope points of the painted curve portion to an XML file.
        /// </summary>
        /// <param name="wavFile">WAV file to display</param>
        /// <param name="beginTime_ms">Start time in ms of the displayed portion</param>
        /// <param name="endTime_ms">End time in ms of the displayed portion</param>
        /// <param name="numberSamplesAfterGrouping">Number of samples after grouping the original samples. Grouping is used to reduce the number of samples that are displayed</param>
        /// <param name="envelopeSaveFilePath">If you want to save the points that describe the envelope of the painted curve portion to a file, use this parameter to specify the file path for that XML file. Otherwise set this to ""</param>
        public WAV_Visualization(WaveFile wavFile, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping, string envelopeSaveFilePath = "")
        {
            InitializeComponent();
            Init(wavFile, beginTime_ms, endTime_ms, numberSamplesAfterGrouping, envelopeSaveFilePath);
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Init function that is called from all constructors to initialize all parameters.
        /// </summary>
        /// <param name="wavFile">WAV file to display</param>
        /// <param name="beginTime_ms">Start time in ms of the displayed portion</param>
        /// <param name="endTime_ms">End time in ms of the displayed portion</param>
        /// <param name="numberSamplesAfterGrouping">Number of samples after grouping the original samples. Grouping is used to reduce the number of samples that are displayed</param>
        /// <param name="envelopeSaveFilePath">If you want to save the points that describe the envelope of the painted curve portion to a file, use this parameter to specify the file path for that XML file. Otherwise set this to ""</param>
        private void Init(WaveFile wavFile, double beginTime_ms, double endTime_ms, long numberSamplesAfterGrouping, string envelopeSaveFilePath)
        {
            WavFile = wavFile;
            Filepath = (wavFile == null ? "" : wavFile.Filepath);
            BeginTime_ms = beginTime_ms;
            EndTime_ms = endTime_ms;
            NumberOfSamplesAfterGrouping = numberSamplesAfterGrouping;
            EnvelopeSaveFilePath = envelopeSaveFilePath;
        }

        //##########################################################################################################################################################################################################
        
        private void WAV_Visualization_Load(object sender, EventArgs e)
        {
            this.Text = "WAV Visualization: " + Filepath;

            List<AudioSample> samples_L = new List<AudioSample>();
            List<AudioSample> samples_R = new List<AudioSample>();
            List<PointF> samples_envelope_L = new List<PointF>();
            List<PointF> samples_envelope_R = new List<PointF>();

            if (WavFile == null) { return; }

            samples_L = WavFile.GetChannelSamples(AudioChannels.LEFT, BeginTime_ms, EndTime_ms);
            samples_R = WavFile.GetChannelSamples(AudioChannels.RIGHT, BeginTime_ms, EndTime_ms);

            samples_envelope_L = CalculateEnvelope(samples_L, 175);
            samples_envelope_R = CalculateEnvelope(samples_R, 175);

            List<PointF> samples_envelope_L_shifted = samples_envelope_L.Select(s => new PointF(s.X - samples_envelope_L.First().X, s.Y)).ToList();
            List<PointF> samples_envelope_R_shifted = samples_envelope_R.Select(s => new PointF(s.X - samples_envelope_R.First().X, s.Y)).ToList();

            if (EnvelopeSaveFilePath != "")
            { 
                FadeSettings fadeSettings = new FadeSettings(0, FadeTypes.CUSTOM, samples_envelope_L_shifted, AudioChannels.RIGHT_AND_LEFT);
                fadeSettings.SaveFadePoints(EnvelopeSaveFilePath);
            }

            samples_L = GroupSamples(samples_L, NumberOfSamplesAfterGrouping);
            samples_R = GroupSamples(samples_R, NumberOfSamplesAfterGrouping);

            /*chart1.ChartAreas[0].AxisY.IsLogarithmic = true;
            samples_L.ForEach(s => s.Value = (float)(Math.Abs(s.Value) + 0.00001));
            samples_R.ForEach(s => s.Value = (float)(Math.Abs(s.Value) + 0.00001));
            samples_envelope_L.ForEach(p => p.Y = (float)(Math.Abs(p.Y) + 0.00001));
            samples_envelope_R.ForEach(p => p.Y = (float)(Math.Abs(p.Y) + 0.00001));
            */


            foreach (AudioSample s in samples_L)
            {
                chart1.Series["Series_Left"].Points.AddXY(s.Time_ms, s.Value);
            }
            foreach (AudioSample s in samples_R)
            {
                chart1.Series["Series_Right"].Points.AddXY(s.Time_ms, s.Value);
            }

            if (samples_envelope_L.Count < 100) { chart1.Series["Series_Envelope_Left"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line; }     //Change chart type from FastLine to Line to show markers
            if (samples_envelope_R.Count < 100) { chart1.Series["Series_Envelope_Right"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line; }
            foreach (PointF p in samples_envelope_L)
            {
                chart1.Series["Series_Envelope_Left"].Points.AddXY(p.X, p.Y);
            }
            foreach (PointF p in samples_envelope_R)
            {
                chart1.Series["Series_Envelope_Right"].Points.AddXY(p.X, p.Y);
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

        /// <summary>
        /// Calculate the envelope curve for the given samples. The samples are devided in bins. For each bin the maximum absolute value is calculated.
        /// </summary>
        /// <param name="samples">Samples for which the envelope curve is calculated</param>
        /// <param name="binWidth">number of samples per bin</param>
        /// <returns>list with envelope points</returns>
        public static List<PointF> CalculateEnvelope(List<AudioSample> samples, int binWidth)
        {
            List<PointF> envelopePoints = new List<PointF>();
            int numberOfBins = samples.Count / binWidth;
            for(int i = 0; i < numberOfBins; i++)
            {
                List<AudioSample> binSamples = samples.GetRange(i * binWidth, binWidth);
                float binMaxValue = binSamples.Select(s => Math.Abs(s.Value)).Max();
                float binMaxTime = (float)binSamples.Select(s => s.Time_ms).Average();
                envelopePoints.Add(new PointF(binMaxTime, binMaxValue));
            }
            return envelopePoints;
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

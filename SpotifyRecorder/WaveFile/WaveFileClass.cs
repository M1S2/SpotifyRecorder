using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.Streams.SampleConverter;

namespace SpotifyRecorder.WaveFile
{
    /// <summary>
    /// Class that can be used to read samples of a WAV file and split them to the left and right channel
    /// </summary>
    public class WaveFileClass : ICloneable
    {
        /// <summary>
        /// Filepath of the WAV file
        /// </summary>
        public string Filepath { get; private set; }

        /// <summary>
        /// Samples of both channels
        /// </summary>
        public List<AudioSample> Samples { get; set; }

        /// <summary>
        /// Length of the WAV file in ms
        /// </summary>
        public double Length_s { get; private set; }

        private WaveFormat format;

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Init a new WaveFile class
        /// </summary>
        /// <param name="filepath">filepath of the WAV file</param>
        public WaveFileClass(string filepath)
        {
            if (File.Exists(filepath))
            {
                Filepath = filepath;
                Samples = new List<AudioSample>();
                ReadToList();
            }
            else
            {
                Filepath = "";
                System.Windows.Forms.MessageBox.Show("The given file doesn't exist:" + Environment.NewLine + filepath, "File doesn't exist.", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Clone this WaveFile
        /// </summary>
        /// <returns>cloned WaveFile</returns>
        public object Clone()
        {
            WaveFileClass wavFile = new WaveFileClass(this.Filepath);
            wavFile.Samples = this.Samples.Clone();
            wavFile.format = this.format;
            return wavFile;

            //return this.MemberwiseClone();
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Read the WAV file and split the samples to the left and right channel
        /// </summary>
        private void ReadToList()
        {
            if (!File.Exists(Filepath)) { return; }

            WaveFileReader reader = new WaveFileReader(Filepath);
            format = reader.WaveFormat;

            //Length_s = reader.GetLength().TotalSeconds;             // reader.GetLength() contains the FormatChunks too!
            //Length_s = (reader.Length / format.BytesPerSecond);     // reader.Length contains the FormatChunks too!
            List<CSCore.Codecs.WAV.WaveFileChunk> waveFileDataChunks = reader.Chunks.Where(c => c.GetType() == typeof(DataChunk)).ToList();
            long waveFileDataChunksSizeBytes = waveFileDataChunks.Sum(c => c.ChunkDataSize);
            Length_s = (waveFileDataChunksSizeBytes / format.BytesPerSecond);

            //long _sampleCount = reader.Length / (reader.WaveFormat.BitsPerSample / 8);
            //_sampleCount /= reader.WaveFormat.Channels;                                 //Each sample contains the values of the right and left channel for (Waveformat.Channels == 2)

            ISampleSource source = WaveToSampleBase.CreateConverter(reader);

            format = source.WaveFormat;

            float[] sample_buffer = new float[format.Channels];
            while (source.Read(sample_buffer, 0, sample_buffer.Length) > 0 && source.Position < (waveFileDataChunksSizeBytes / format.Channels))          //At least one byte read
            {
                double time_ms = ((1 / (double)source.WaveFormat.BytesPerSecond) * source.WaveFormat.BytesPerSample * source.Position) * 1000;

                AudioSample sample_left = new AudioSample(sample_buffer[0], time_ms, AudioChannels.LEFT);
                Samples.Add(sample_left);

                if (reader.WaveFormat.Channels == 2)
                {
                    AudioSample sample_right = new AudioSample(sample_buffer[1], time_ms, AudioChannels.RIGHT);
                    Samples.Add(sample_right);
                }
            }
            reader.Dispose();
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Save all samples to a new WAV file
        /// </summary>
        /// <param name="filepath">filepath of the new WAV file</param>
        public void SaveFile(string filepath)
        {
            if(Samples == null || format == null) { return; }

            if(!File.Exists(filepath))
            {
                FileStream stream = File.Create(filepath);
                stream.Close();
            }
            else
            {
                File.Delete(filepath);
            }

            WaveWriter writer = new WaveWriter(filepath, format);
            foreach (AudioSample s in Samples)
            {
                writer.WriteSample(s.Value);
            }
            writer.Dispose();
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Only return the samples of the given channel. Only a copy of the samples is returned (no reference).
        /// </summary>
        /// <param name="channel">audio channel</param>
        /// <param name="beginTime_ms">the time of the first sample to get. Samples with times smaller than beginTime aren't returned. Use -1 to ignore the beginTime_ms.</param>
        /// <param name="endTime_ms">the time of the last sample to get. Samples with times greater than endTime aren't returned. Use -1 to ignore the endTime_ms.</param>
        /// <returns>samples of the given channel</returns>
        public List<AudioSample> GetChannelSamples(AudioChannels channel, double beginTime_ms = -1, double endTime_ms = -1)
        {
            List<AudioSample> filtered_samples = new List<AudioSample>();
            if(Samples == null) { return filtered_samples; }

            foreach (AudioSample s in Samples)
            {
                if(s.Channel == channel || (channel == AudioChannels.RIGHT_AND_LEFT && (s.Channel == AudioChannels.LEFT || s.Channel == AudioChannels.RIGHT)))
                {
                    if ((beginTime_ms >= 0 && s.Time_ms < beginTime_ms) || (endTime_ms > 0 && s.Time_ms > endTime_ms)) { continue; }
                    filtered_samples.Add(s);
                }
            }

            return filtered_samples;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Remove all silent parts of the WAV file if they are longer than minDuration_s and shorter than maxDuration_ms
        /// </summary>
        /// <param name="minDuration_ms">minimum duration of silent parts in ms.</param>
        /// <param name="maxDuration_ms">maximum duration of silent parts in ms. Use -1 for infinite maximum duration.</param>
        /// <param name="silence_threshold">threshold of silence. All samples that are under this value are silence.</param>
        public List<SilenceParts> RemoveSilence(double minDuration_ms, double maxDuration_ms, double silence_threshold)
        {
            List<SilenceParts> silenceParts = new List<SilenceParts>();
            List<AudioSample> samples_copy = new List<AudioSample>(Samples);
            if (format == null || Samples == null || minDuration_ms <= 0) { return silenceParts; }

            if (maxDuration_ms > 0 && maxDuration_ms < minDuration_ms) { maxDuration_ms = minDuration_ms; }

            double start_silence_ms = -1;
            int silence_start_index = -1;
            int cnt_removed_samples = 0;
            for (int i = 0; i < Samples.Count; i++)
            {
                if (Samples[i].Value < silence_threshold && silence_start_index == -1 && Samples[i].Time_ms > 500)            //Silence begin (all silent parts at the beginning (t < 500 ms) aren't removed)
                {
                    start_silence_ms = Samples[i].Time_ms;
                    silence_start_index = i;
                }
                else if (Samples[i].Value > silence_threshold && silence_start_index != -1)       //Silence end
                {
                    double silence_length = Samples[i].Time_ms - start_silence_ms;
                    if (silence_length >= minDuration_ms && (maxDuration_ms <= 0 || (maxDuration_ms > 0 && silence_length <= maxDuration_ms)))
                    {
                        samples_copy.RemoveRange(silence_start_index - cnt_removed_samples, i - silence_start_index);
                        silenceParts.Add(new SilenceParts(start_silence_ms, start_silence_ms - ((format.BytesPerSample / (double)format.BytesPerSecond) * cnt_removed_samples) * 1000, Samples[i].Time_ms));
                        cnt_removed_samples += (i - silence_start_index);
                    }
                    silence_start_index = -1;
                }

                samples_copy[i - cnt_removed_samples].Time_ms -= ((format.BytesPerSample / (double)format.BytesPerSecond) * cnt_removed_samples) * 1000;
            }
            Length_s -= ((format.BytesPerSample / (double)format.BytesPerSecond) * cnt_removed_samples) * 1000;
            Samples = new List<AudioSample>(samples_copy);
            return silenceParts;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Apply the given fades to the file
        /// </summary>
        /// <param name="fades">list with fade settings</param>
        public void ApplyFading(List<FadeSettings> fades)
        {
            if (Samples != null)
            {
                for (int i = 0; i < Samples.Count; i++)     //loop through each sample in the WAV file
                {
                    double currentTime_ms = Samples[i].Time_ms;
                    float factor = 1;

                    foreach(FadeSettings fade in fades)
                    {
                        if(fade.FadeStartTime_ms <= currentTime_ms && (fade.FadeStartTime_ms + fade.FadeLength_ms) > currentTime_ms)        //The sample is in the area, where the current fade should be applied
                        {
                            if (Samples[i].Channel == fade.FadeChannels || (fade.FadeChannels == AudioChannels.RIGHT_AND_LEFT && (Samples[i].Channel == AudioChannels.LEFT || Samples[i].Channel == AudioChannels.RIGHT)))      //The sample has the correct channel
                            {
                                factor *= (float)fade.GetFadeFactor(currentTime_ms);
                            }
                        }
                    }
                    Samples[i].Value *= factor;

                    //if(Math.Abs(Samples[i].Value) > 1) { Samples[i].Value = Math.Sign(Samples[i].Value); }
                }
            }
        }

    }
}

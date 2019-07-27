using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyRecorder.WaveFile
{
    /// <summary>
    /// Class containing the value, time and channel of one audio sample
    /// </summary>
    public class AudioSample : ICloneable
    {
        /// <summary>
        /// Audio sample value
        /// </summary>
        public float Value;

        /// <summary>
        /// Audio sample time in ms
        /// </summary>
        public double Time_ms;

        /// <summary>
        /// Audio sample channel
        /// </summary>
        public AudioChannels Channel;


        /// <summary>
        /// Constructor of Sample class
        /// </summary>
        /// <param name="value">Audio sample value</param>
        /// <param name="time_ms">Audio sample time in ms</param>
        /// <param name="channel">Audio sample channel</param>
        public AudioSample(float value, double time_ms, AudioChannels channel)
        {
            Value = value;
            Time_ms = time_ms;
            Channel = channel;
        }

        /// <summary>
        /// Clone this AudioSample
        /// </summary>
        /// <returns>cloned AudioSample</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public static class AudioSampleHelper
    {
        //see: https://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}

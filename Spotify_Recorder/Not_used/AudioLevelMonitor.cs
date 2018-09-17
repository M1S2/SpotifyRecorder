using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;
using System.Drawing;

namespace Spotify_Recorder
{
    /// <summary>
    /// Class to monitor the master volume level of an audio device. The volume is get via polling.
    /// </summary>
    public class AudioLevelMonitor
    {
        private double pollingInterval;
        /// <summary>
        /// Polling intervall in seconds
        /// </summary>
        public double PollingInterval
        {
            get { return pollingInterval; }
            set { pollingInterval = value; pollingTimer.Interval = pollingInterval * 1000; }
        }

        /// <summary>
        /// Monitor the master volume for this device
        /// </summary>
        public MMDevice MonitorDevice { get; set; }

        /// <summary>
        /// List with volume values (x=time of volume change | y=master volume)
        /// </summary>
        public List<PointF> VolumeValues { get; set; } 

        /// <summary>
        /// The number of seconds since the monitor was started
        /// </summary>
        public double MonitorTime { get; set; }

        private System.Timers.Timer pollingTimer = new System.Timers.Timer();

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new AudioLevelMonitor for the given device with the given polling interval
        /// </summary>
        /// <param name="monitorDevice">Monitor the master volume for this device</param>
        /// <param name="pollingInterval">Polling intervall in seconds</param>
        public AudioLevelMonitor(MMDevice monitorDevice, double pollingInterval)
        {
            MonitorDevice = monitorDevice;
            PollingInterval = pollingInterval;
            VolumeValues = new List<PointF>();
        }

        /// <summary>
        /// Create a new AudioLevelMonitor for the default rendering device with a polling interval of 100 ms
        /// </summary>
        public AudioLevelMonitor()
        {
            PollingInterval = 0.1;
            MMDeviceEnumerator devEnumerator = new MMDeviceEnumerator();
            MonitorDevice = devEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            VolumeValues = new List<PointF>();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the monitoring of the master volume.
        /// </summary>
        public void Start()
        {
            pollingTimer.Elapsed += PollingTimer_Elapsed;
            pollingTimer.AutoReset = false;
            MonitorTime = 0;
            pollingTimer.Start();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Stop the monitoring of the master volume.
        /// </summary>
        public void Stop()
        {
            pollingTimer.Stop();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Poll the master volume of the MonitorDevice and add the volume to the list if the volume changed.
        /// </summary>
        private void PollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MonitorTime += PollingInterval;
            float masterVolume = VolumeControl.GetMasterVolumeFromDevice(MonitorDevice);
            if (VolumeValues.Count == 0 || VolumeValues.Last().Y != masterVolume)
            {
                VolumeValues.Add(new PointF((float)MonitorTime, masterVolume));
            }

            pollingTimer.Start(); // trigger next timer
        }

    }
}

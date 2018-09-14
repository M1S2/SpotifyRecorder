using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Codecs.WAV;
using CSCore.Codecs.MP3;
using CSCore.MediaFoundation;
using CSCore.CoreAudioAPI;
using CSCore.Streams;
using CSCore.Codecs;
using System.Runtime.InteropServices;

namespace Spotify_Recorder
{
    //see: https://stackoverflow.com/questions/23126407/trying-to-build-a-simple-c-sharp-app-to-control-my-volume-mixer

    class VolumeControl
    {
        private static Dictionary<string, bool> previous_mute_settings = new Dictionary<string, bool>();        //save all old muting settings <app_name, mute>

        /// <summary>
        /// Mute all applications except the app with the given name
        /// </summary>
        /// <param name="appName">application name</param>
        public static void MuteAllExcept(string appName)
        {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    using (AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device))
                    {
                        using (AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator())
                        {
                            foreach (AudioSessionControl session in sessionEnumerator)
                            {
                                using (SimpleAudioVolume volume = session.QueryInterface<SimpleAudioVolume>())
                                {
                                    AudioSessionControl2 session2 = session.QueryInterface<AudioSessionControl2>();
                                    if (session2.Process != null)
                                    {
                                        string name = session2.Process.ProcessName;
                                        if (previous_mute_settings.ContainsKey(name)) { previous_mute_settings[name] = volume.IsMuted; }        //Save all previous muting settings
                                        else { previous_mute_settings.Add(name, volume.IsMuted); }

                                        if (name.ToLower().Contains(appName.ToLower()))
                                        {
                                            volume.IsMuted = false;
                                        }
                                        else
                                        {
                                            volume.IsMuted = true;
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Restore all previous mute settings
        /// </summary>
        public static void RestoreMuteSettings()
        {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    using (AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device))
                    {
                        using (AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator())
                        {
                            foreach (AudioSessionControl session in sessionEnumerator)
                            {
                                using (SimpleAudioVolume volume = session.QueryInterface<SimpleAudioVolume>())
                                {
                                    AudioSessionControl2 session2 = session.QueryInterface<AudioSessionControl2>();
                                    if (session2.Process != null)
                                    {
                                        string name = session2.Process.ProcessName;
                                        if (previous_mute_settings.ContainsKey(name))
                                        {
                                            volume.IsMuted = previous_mute_settings[name];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Return all sound application names
        /// </summary>
        /// <returns>List of all sound application names</returns>
        public static List<string> GetAllSoundApps()
        {
            List<string> appNames = new List<string>();

            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    using (AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device))
                    {
                        using (AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator())
                        {
                            foreach (AudioSessionControl session in sessionEnumerator)
                            {
                                using (AudioSessionControl2 session2 = session.QueryInterface<AudioSessionControl2>())
                                {
                                    if (session2.Process != null)
                                    {
                                        appNames.Add(session2.Process.ProcessName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return appNames;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the master volume of the given device. The value is between 0 (off) and 1 (maximum)
        /// </summary>
        /// <param name="device">device to get the master volume from</param>
        /// <returns>volume between 0 and 1</returns>
        public static float GetMasterVolumeFromDevice(MMDevice device)
        {
            AudioEndpointVolume endpointVolume = AudioEndpointVolume.FromDevice(device);
            return endpointVolume.GetMasterVolumeLevelScalar();
        }

        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************
        //***********************************************************************************************************************************************************************************************************

        /*
        public static void Test()
        {
            using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
            {
                MMNotificationClient notificationClient = new MMNotificationClient(enumerator);
                notificationClient.DeviceAdded += NotificationClient_DeviceAdded;
                notificationClient.DeviceRemoved += NotificationClient_DeviceRemoved;
                notificationClient.DevicePropertyChanged += NotificationClient_DevicePropertyChanged;
                notificationClient.DeviceStateChanged += NotificationClient_DeviceStateChanged;
                enumerator.RegisterEndpointNotificationCallback(notificationClient);

                using (MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    AudioEndpointVolume audioEndpointVolume = AudioEndpointVolume.FromDevice(device);
                    float volumeLevel = audioEndpointVolume.GetChannelVolumeLevel(0);
                    float volumeLevelScalar = audioEndpointVolume.GetChannelVolumeLevelScalar(0);
                    float masterVolLevel = audioEndpointVolume.GetMasterVolumeLevel();
                    float masterVolLevelScalar = audioEndpointVolume.GetMasterVolumeLevelScalar();
                    bool muted = audioEndpointVolume.GetMute();
                    uint currentStep, stepCount;
                    audioEndpointVolume.GetVolumeStepInfo(out currentStep, out stepCount);
                    AudioEndpointVolumeCallback audioEndpointVolumeCallback = new AudioEndpointVolumeCallback();
                    audioEndpointVolumeCallback.NotifyRecived += AudioEndpointVolumeCallback_NotifyRecived;
                    audioEndpointVolume.RegisterControlChangeNotify(audioEndpointVolumeCallback);

                    using (AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device))
                    {
                        AudioSessionNotification audioSessionNotification = new AudioSessionNotification();
                        audioSessionNotification.SessionCreated += AudioSessionNotification_SessionCreated;
                        RegisterSessionNotification(sessionManager, audioSessionNotification);

                        AudioVolumeDuckNotification audioVolumeDuckNotification = new AudioVolumeDuckNotification();
                        audioVolumeDuckNotification.VolumeDuckNotification += AudioVolumeDuckNotification_VolumeDuckNotification;
                        audioVolumeDuckNotification.VolumeUnDuckNotification += AudioVolumeDuckNotification_VolumeUnDuckNotification;
                        sessionManager.RegisterDuckNotification("", audioVolumeDuckNotification);

                        using (AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator())
                        {
                            foreach (AudioSessionControl session in sessionEnumerator)
                            {
                                AudioSessionEvents audioSessionEvents = new AudioSessionEvents();
                                audioSessionEvents.ChannelVolumeChanged += AudioSessionEvents_ChannelVolumeChanged;
                                audioSessionEvents.DisplayNameChanged += AudioSessionEvents_DisplayNameChanged;
                                audioSessionEvents.SessionDisconnected += AudioSessionEvents_SessionDisconnected;
                                audioSessionEvents.SimpleVolumeChanged += AudioSessionEvents_SimpleVolumeChanged;
                                audioSessionEvents.StateChanged += AudioSessionEvents_StateChanged;
                                session.RegisterAudioSessionNotification(audioSessionEvents);

                                using (AudioSessionControl2 session2 = session.QueryInterface<AudioSessionControl2>())
                                {

                                }
                            }
                        }
                    }
                }
            }
        }


        //see: https://github.com/filoe/cscore/blob/master/CSCore.Test/CoreAudioAPI/AudioSessionTests.cs
        private static void RegisterSessionNotification(AudioSessionManager2 audioSessionManager2, IAudioSessionNotification audioSessionNotification)
        {
            if (System.Threading.Thread.CurrentThread.GetApartmentState() == System.Threading.ApartmentState.MTA)
            {
                audioSessionManager2.RegisterSessionNotification(audioSessionNotification);
            }
            else
            {
                using (System.Threading.ManualResetEvent waitHandle = new System.Threading.ManualResetEvent(false))
                {
                    System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                    {
                        try
                        {
                            audioSessionManager2.RegisterSessionNotification(audioSessionNotification);
                        }
                        finally
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            waitHandle.Set();
                        }
                    });
                    waitHandle.WaitOne();
                }
            }

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            audioSessionManager2.GetSessionEnumerator().ToArray(); //necessary to make it work
        }

        private static void AudioEndpointVolumeCallback_NotifyRecived(object sender, AudioEndpointVolumeCallbackEventArgs e)
        {
        }

        private static void AudioSessionEvents_StateChanged(object sender, AudioSessionStateChangedEventArgs e)
        {
        }

        private static void AudioSessionEvents_SimpleVolumeChanged(object sender, AudioSessionSimpleVolumeChangedEventArgs e)
        {
        }

        private static void AudioSessionEvents_SessionDisconnected(object sender, AudioSessionDisconnectedEventArgs e)
        {
        }

        private static void AudioSessionEvents_DisplayNameChanged(object sender, AudioSessionDisplayNameChangedEventArgs e)
        {
        }

        private static void AudioSessionEvents_ChannelVolumeChanged(object sender, AudioSessionChannelVolumeChangedEventArgs e)
        {
        }

        private static void AudioVolumeDuckNotification_VolumeUnDuckNotification(object sender, VolumeDuckNotificationEventArgs e)
        {
        }

        private static void AudioVolumeDuckNotification_VolumeDuckNotification(object sender, VolumeDuckNotificationEventArgs e)
        {
        }

        private static void AudioSessionNotification_SessionCreated(object sender, SessionCreatedEventArgs e)
        {
            string name = e.NewSession.DisplayName;
        }

        private static void NotificationClient_DeviceStateChanged(object sender, DeviceStateChangedEventArgs e)
        {
        }

        private static void NotificationClient_DevicePropertyChanged(object sender, DevicePropertyChangedEventArgs e)
        {
        }

        private static void NotificationClient_DeviceRemoved(object sender, DeviceNotificationEventArgs e)
        {
        }

        private static void NotificationClient_DeviceAdded(object sender, DeviceNotificationEventArgs e)
        {
        }*/
    }
}

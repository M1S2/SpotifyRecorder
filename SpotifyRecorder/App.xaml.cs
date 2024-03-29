﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using LogBox.LogEvents;

namespace SpotifyRecorder
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    /// see: https://www.c-sharpcorner.com/UploadFile/07c1e7/create-splash-screen-in-wpf/
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 2000; // Milliseconds

        private MainWindow main;
        private SplashScreen splash;

        protected async override void OnStartup(StartupEventArgs e)
        {
            splash = new SplashScreen();
            splash.Show();
            splash.StatusString = "Loading Window ...";

            Stopwatch timer = new Stopwatch();      //Start a stop watch 
            timer.Start();

            base.OnStartup(e);
            main = new MainWindow();     //Load window but don't show it yet

            await StartAndConnectToPlayer(true);

            timer.Stop();
            
            int remainingTimeToShowSplash = MINIMUM_SPLASH_TIME - (int)timer.ElapsedMilliseconds;
            if (remainingTimeToShowSplash > 0) { await Task.Delay(remainingTimeToShowSplash); }

            main.Show();
            splash.Close();
            splash = null;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Start the player application if it's not running and connect to it
        /// </summary>
        /// <param name="startMinimized">true -> start player minimized; false -> start normal</param>
        /// <param name="forceReauthenticate">if true, force the user to reauthenticate to the player application</param>
        public async Task StartAndConnectToPlayer(bool startMinimized = true, bool forceReauthenticate = false)
        {
            bool playerStarted = true;
            if (!main.PlayerApp.IsPlayerApplicationRunning)
            {
                main._logHandle.Report(new LogEventInfo("Starting " + main.PlayerApp.PlayerName + " application."));
                if (splash != null) { splash.StatusString = "Starting player application ..."; }
            }
            playerStarted = await main.PlayerApp.StartPlayerApplication(startMinimized);
                        
            if (playerStarted)
            {
                main._logHandle.Report(new LogEventInfo("Connecting..."));
                if (splash != null) { splash.StatusString = "Connecting to player ..."; }
                await main.PlayerApp.Connect(30000, forceReauthenticate);

                if (main.PlayerApp.IsConnected)
                {
                    main._logHandle.Report(new LogEventInfo("Connected successfully."));
                    if (splash != null) { splash.StatusString = "Connected successfully"; }

                    main.PlayerApp.ListenForEvents = true;
                    main.PlayerApp.UpdateCurrentPlaybackStatus();
                    await Task.Delay(1000);
                }
                else
                {
                    main._logHandle.Report(new LogEventWarning("Connection failed."));
                    if (splash != null) { splash.StatusString = "Connection failed"; }
                }
            }
            else
            {
                main._logHandle.Report(new LogEventWarning("Failed to start player application."));
            }
        }
    }
}

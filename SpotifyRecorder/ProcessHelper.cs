using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;

namespace SpotifyRecorder
{
    static class ProcessHelper
    {
        /// <summary>
        /// Find a process by the given name
        /// </summary>
        /// <param name="processName">name of the process to find</param>
        /// <param name="onlyApplicationProcesses">if true, only application processes are returned; if false all processes (application, background, ...) are returned</param>
        /// <returns>list of Process object or empty list, if the process wasn't found</returns>
        /// see: https://stackoverflow.com/questions/4722198/checking-if-my-windows-application-is-running
        public static List<Process> FindProcess(string processName, bool onlyApplicationProcesses)
        {
            List<Process> processes = Process.GetProcesses().Where(p => p.ProcessName == processName).ToList();
            if (onlyApplicationProcesses) { processes = processes.Where(p => p.MainWindowHandle != IntPtr.Zero).ToList(); }
            return processes;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if a process with the given name is open
        /// </summary>
        /// <param name="processName">name of the process to find</param>
        /// <param name="onlyApplicationProcesses">if true, only application processes are returned; if false all processes (application, background, ...) are returned</param>
        /// <returns>true -> process is open; false -> process is not open</returns>
        /// see: https://stackoverflow.com/questions/4722198/checking-if-my-windows-application-is-running
        public static bool IsProcessOpen(string processName, bool onlyApplicationProcesses)
        {
            return (FindProcess(processName, onlyApplicationProcesses).Count > 0);
        }

        //***********************************************************************************************************************************************************************************************************
        
        /// <summary>
        /// Start a process with the given path
        /// </summary>
        /// <param name="processPath">path of the process to start</param>
        /// <param name="processArguments">process arguments that are used when starting the process</param>
        /// <param name="windowStyle">Start the process in an minimized, maximized or normal window</param>
        /// <returns>true -> starting successful; false -> starting not successful</returns>
        public async static Task<bool> StartProcess(string processPath, string processArguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            try
            {
                Process process = null;
                await Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = processPath,
                        Arguments = processArguments,
                        WindowStyle = windowStyle
                    };

                    process = Process.Start(startInfo);

                });
                return (process != null);
            }
            catch(Exception)
            {
                return false;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Close the main window of the process with the given name
        /// </summary>
        /// <param name="processName">name of the process to close</param>
        /// <param name="onlyApplicationProcesses">if true, only application processes are returned; if false all processes (application, background, ...) are returned</param>
        public static void StopProcess(string processName, bool onlyApplicationProcesses)
        {
            List<Process> processes = FindProcess(processName, onlyApplicationProcesses);
            foreach (Process process in processes)
            {
                try
                {
                    process.CloseMainWindow();
                    process.Kill();
                    //process.WaitForExit();
                }
                catch (Exception) { }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Find all processes with the given name and return the full process path (including the start arguments). If more processes were found, a list with all process paths is returned.
        /// </summary>
        /// <param name="processName">name of the process to find (without ".exe")</param>
        /// <returns>list with full process paths (path including the start arguments)</returns>
        /// see: https://social.msdn.microsoft.com/Forums/en-US/669eeaeb-e6fa-403b-86fd-302b24c569fb/how-to-get-the-command-line-arguments-of-running-processes?forum=netfxbcl
        public static List<string> GetProcessStartArguments(string processName)
        {
            List<string> fullProcessPaths = new List<string>();

            System.Management.ManagementClass managementClass = new System.Management.ManagementClass("Win32_Process");
            foreach(System.Management.ManagementObject managementObject in managementClass.GetInstances())
            {
                string name = (string)managementObject["Name"];
                if (name == processName + ".exe")
                {
                    fullProcessPaths.Add((string)managementObject["CommandLine"]);
                }
            }
            return fullProcessPaths;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the window state for the window of the given process
        /// </summary>
        /// <param name="processName">Name of the process (without .exe)</param>
        /// <returns>window state structure</returns>
        /// see: https://stackoverflow.com/questions/11065026/get-window-state-of-another-process
        public static WindowTheme.WindowPlacement.WINDOWPLACEMENT GetProcessWindowState(string processName)
        {
            List<Process> processes = FindProcess(processName, true);
            if(processes.Count == 0) { return new WindowTheme.WindowPlacement.WINDOWPLACEMENT(); }
            return WindowTheme.WindowPlacement.GetPlacement(processes.First().MainWindowHandle);
        }

    }
}

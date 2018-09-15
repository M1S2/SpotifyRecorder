using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;

namespace Spotify_Recorder
{
    static class ProcessHelper
    {
        /// <summary>
        /// Find a process by the given name
        /// </summary>
        /// <param name="processName">name of the process to find</param>
        /// <returns>list of Process object or empty list, if the process wasn't found</returns>
        /// see: https://stackoverflow.com/questions/4722198/checking-if-my-windows-application-is-running
        public static List<Process> FindProcess(string processName)
        {
            List<Process> processes = new List<Process>();
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == processName)
                {
                    processes.Add(clsProcess);
                }
            }

            return processes;
        }

        /// <summary>
        /// Check if a process with the given name is open
        /// </summary>
        /// <param name="processName">name of the process to find</param>
        /// <returns>true -> process is open; false -> process is not open</returns>
        /// see: https://stackoverflow.com/questions/4722198/checking-if-my-windows-application-is-running
        public static bool IsProcessOpen(string processName)
        {
            return (FindProcess(processName).Count > 0);
        }

        /// <summary>
        /// Start a process with the given path
        /// </summary>
        /// <param name="processPath">path of the process to start</param>
        /// <param name="processArguments">process arguments that are used when starting the process</param>
        /// <returns>true -> starting successful; false -> starting not successful</returns>
        public static bool StartProcess(string processPath, string processArguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            try
            {
                Process process = Process.Start(processPath, processArguments);
                return (process != null);
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Close the main window of the process with the given name
        /// </summary>
        /// <param name="processName">name of the process to close</param>
        public static void StopProcess(string processName)
        {
            List<Process> processes = FindProcess(processName);
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

        ///// <summary>
        ///// Get the path of the application with the given name
        ///// </summary>
        ///// <param name="applicationName">application name</param>
        ///// <returns>path to the application or ""</returns>
        ///// see: https://social.msdn.microsoft.com/Forums/windows/en-US/afb5012a-30f1-4b96-9931-a143fd76bab5/how-to-find-path-of-installed-programs-in-c?forum=winformssetup
        //public static string GetApplicationPathByDisplayName(string applicationName)
        //{
        //    RegistryKey parentKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");

        //    string[] nameList = parentKey.GetSubKeyNames();
        //    List<string> displayNameList = new List<string>();
        //    for (int i = 0; i < nameList.Length; i++)
        //    {
        //        RegistryKey regKey = parentKey.OpenSubKey(nameList[i]);
        //        try
        //        {
        //            displayNameList.Add(regKey.GetValue("DisplayName").ToString());
        //            if (regKey.GetValue("DisplayName").ToString() == applicationName)
        //            {
        //                return regKey.GetValue("InstallLocation").ToString();
        //            }
        //        }
        //        catch { }
        //    }
        //    return "";
        //}
    }
}

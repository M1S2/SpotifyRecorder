using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace SpotifyRecorder.WindowTheme
{
    /// <summary>
    /// Get and set window placement properties
    /// </summary>
    /// see: https://engy.us/blog/2010/03/08/saving-window-size-and-location-in-wpf-and-winforms/
    public static class WindowPlacement
    {
        /// <summary>
        /// Window placement structure containing infos about window state, and position
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowStates showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        /// <summary>
        /// Window state (Normal, Minimized, Maximized)
        /// </summary>
        public enum ShowWindowStates : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        private static Encoding encoding = new UTF8Encoding();
        private static XmlSerializer serializer = new XmlSerializer(typeof(WINDOWPLACEMENT));

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Set the window placement from an xml string
        /// </summary>
        /// <param name="windowHandle">Window handle to set placement for</param>
        /// <param name="placementXml">XML string containing placement infos</param>
        public static void SetPlacementString(IntPtr windowHandle, string placementXml)
        {
            if (string.IsNullOrEmpty(placementXml))
            {
                return;
            }

            WINDOWPLACEMENT placement;
            byte[] xmlBytes = encoding.GetBytes(placementXml);

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(xmlBytes))
                {
                    placement = (WINDOWPLACEMENT)serializer.Deserialize(memoryStream);
                }

                SetPlacement(windowHandle, placement);
            }
            catch (InvalidOperationException)
            {
                // Parsing placement XML failed. Fail silently.
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Set the window placement from an WINDOWPLACEMENT structure
        /// </summary>
        /// <param name="windowHandle">Window handle to set placement for</param>
        /// <param name="placement">Structure containing placement infos</param>
        public static void SetPlacement(IntPtr windowHandle, WINDOWPLACEMENT placement)
        {
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            placement.flags = 0;
            //placement.showCmd = (placement.showCmd == ShowWindowStates.Minimized ? ShowWindowStates.Normal : placement.showCmd);
            SetWindowPlacement(windowHandle, ref placement);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Extension method for setting window placement
        /// </summary>
        /// <param name="window">Window to set placement for</param>
        /// <param name="placementXml">XML string containing placement infos</param>
        public static void SetPlacementString(this System.Windows.Window window, string placementXml)
        {
            SetPlacementString(new System.Windows.Interop.WindowInteropHelper(window).Handle, placementXml);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the window placement as an WINDOWPLACEMENT structure
        /// </summary>
        /// <param name="windowHandle">Window handle to get placement for</param>
        /// <returns>WINDOWPLACEMENT structure</returns>
        public static WINDOWPLACEMENT GetPlacement(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement(windowHandle, out placement);
            return placement;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the window placement as an xml string
        /// </summary>
        /// <param name="window">Window to get placement for</param>
        /// <returns>XML string containing placement infos</returns>
        public static string GetPlacementString(this System.Windows.Window window)
        {
            return GetPlacementString(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Get the window placement as an xml string
        /// </summary>
        /// <param name="windowHandle">Window handle to get placement for</param>
        /// <returns>XML string containing placement infos</returns>
        public static string GetPlacementString(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = GetPlacement(windowHandle);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
                {
                    serializer.Serialize(xmlTextWriter, placement);
                    byte[] xmlBytes = memoryStream.ToArray();
                    return encoding.GetString(xmlBytes);
                }
            }
        }
    }
}

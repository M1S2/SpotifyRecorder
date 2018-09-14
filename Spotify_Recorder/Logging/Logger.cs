using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Spotify_Recorder
{
    /// <summary>
    /// Obsolete! Use LogBox instead!
    /// Class to log times
    /// </summary>
    [Obsolete("Use LogBox instead!")]
    public class Logger : IDisposable
    {
        private StreamWriter writer;
        private DateTime lastLogTime = new DateTime(1, 1, 1);
        
        /// <summary>
        /// Log entries are only created if this property is set to true.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Constructor of the Logger class
        /// </summary>
        /// <param name="filePath">file path of the log file. If the file already exists, it is overwritten!</param>
        public Logger(string filePath)
        {
            if (System.IO.File.Exists(filePath)) { System.IO.File.Delete(filePath); }
            writer = new StreamWriter(filePath, true);
            EnableLogging = true;
        }

        /// <summary>
        /// Only create Logger class without file.
        /// </summary>
        public Logger()
        {
            EnableLogging = false;
        }

        /// <summary>
        /// Write the current time and the time since the last log entry to the log file. Log entries are only created if <see cref="EnableLogging"/> is set to true.
        /// Format: {current time}:{time since last log}:{description text}
        /// </summary>
        /// <param name="description"></param>
        public void LogTime(string description)
        {
            if (EnableLogging)
            {
                DateTime timeNow = DateTime.Now;
                if (lastLogTime.Year == 1) { lastLogTime = timeNow; }
                TimeSpan diff = timeNow.Subtract(lastLogTime);
                if (writer != null)
                {
                    writer.WriteLine(timeNow.ToString("HH:mm:ss.fffffff") + " : " + diff.ToString(@"hh\:mm\:ss\.fffffff") + " : " + description);
                }
                lastLogTime = timeNow;
            }
        }

        /// <summary>
        /// Close the log file
        /// </summary>
        public void Dispose()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify_Recorder
{
    /// <summary>
    /// Log event
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Type of log entry
        /// </summary>
        public LogTypes LogType { get; set; }

        /// <summary>
        /// Time of log entry
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// Text of log entry
        /// </summary>
        public string LogText { get; set; }

        /// <summary>
        /// Constructor of LogEvent
        /// </summary>
        /// <param name="logType">Type of log entry</param>
        /// <param name="logTime">Time of log entry</param>
        /// <param name="logText">Text of log entry</param>
        public LogEvent(LogTypes logType, DateTime logTime, string logText)
        {
            LogType = logType;
            LogTime = logTime;
            LogText = logText;
        }
    }
}

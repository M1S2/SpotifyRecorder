using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Spotify_Recorder
{
    /// <summary>
    /// Logging user control
    /// </summary>
    public partial class LogBox : UserControl
    {
        /// <summary>
        /// List with all log events (unfiltered)
        /// </summary>
        public List<LogEvent> LogEvents { get; private set; }

        private bool _showInfos;
        /// <summary>
        /// Show all INFO events
        /// </summary>
        public bool ShowInfos
        {
            get { return _showInfos; }
            set
            {
                try
                {
                    _showInfos = value;
                    RefreshLogEntries();
                    this.Invoke(new MethodInvoker(delegate () { chk_showInfos.BackColor = ShowInfos ? Color.LightGray : default(Color); }));
                }
                catch (Exception) { }
            }
        }

        private bool _showWarnings;
        /// <summary>
        /// Show all WARNING events
        /// </summary>
        public bool ShowWarnings
        {
            get { return _showWarnings; }
            set
            {
                try
                {
                    _showWarnings = value;
                    RefreshLogEntries();
                    this.Invoke(new MethodInvoker(delegate () { chk_showWarnings.BackColor = ShowWarnings ? Color.LightGray : default(Color); }));
                }
                catch (Exception) { }
            }
        }

        private bool _showErrors;
        /// <summary>
        /// Show all ERROR events
        /// </summary>
        public bool ShowErrors
        {
            get { return _showErrors; }
            set
            {
                try
                {
                    _showErrors = value;
                    RefreshLogEntries();
                    this.Invoke(new MethodInvoker(delegate () { chk_showErrors.BackColor = ShowErrors ? Color.LightGray : default(Color); }));
                }
                catch (Exception) { }
            }
        }

        private ImageList imageListIcons;       //ImageList with Error, Warning and Info icons

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Constructor of LogBox
        /// </summary>
        public LogBox()
        {
            InitializeComponent();
            this.Resize += LogBox_Resize;
            LogEvents = new List<LogEvent>();
        }

        /// <summary>
        /// Init the image list of the list view
        /// </summary>
        private void LogBox_Load(object sender, EventArgs e)
        {
            InitImageList();
            ShowErrors = true;
            ShowWarnings = true;
            ShowInfos = true;

            chk_showErrors.ImageList = imageListIcons;
            chk_showErrors.ImageKey = LogTypes.ERROR.ToString();
            chk_showWarnings.ImageList = imageListIcons;
            chk_showWarnings.ImageKey = LogTypes.WARNING.ToString();
            chk_showInfos.ImageList = imageListIcons;
            chk_showInfos.ImageKey = LogTypes.INFO.ToString();
        }

        /// <summary>
        /// Resize the last column to fill up the remaining space
        /// </summary>
        private void LogBox_Resize(object sender, EventArgs e)
        {
            ResizeLastColumn();
        }

        /// <summary>
        /// Resize the last column to fill up the remaining space
        /// </summary>
        private void ResizeLastColumn()
        {
            listView_log.Columns[listView_log.Columns.Count - 1].Width = -2;
        }

        /// <summary>
        /// Create a new log entry with type, time and text
        /// </summary>
        /// <param name="logType">type of the log entry</param>
        /// <param name="logText">log text</param>
        public void LogEvent(LogTypes logType, string logText)
        {
            LogEvent logEvent = new LogEvent(logType, DateTime.Now, logText);
            LogEvent(logEvent);
        }

        /// <summary>
        /// Create a new log entry with type, time and text
        /// </summary>
        /// <param name="logEvent">log event</param>
        public void LogEvent(LogEvent logEvent)
        {
            LogEvents.Add(logEvent);
            WriteEvent(logEvent);
        }

        /// <summary>
        /// Create a new log entry with type, time and text
        /// </summary>
        /// <param name="logEvent">log event</param>
        public void WriteEvent(LogEvent logEvent)
        {
            this.Invoke(new MethodInvoker(delegate () 
            {
                if ((logEvent.LogType == LogTypes.INFO && ShowInfos) || (logEvent.LogType == LogTypes.WARNING && ShowWarnings) || (logEvent.LogType == LogTypes.ERROR && ShowErrors))
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.ImageKey = logEvent.LogType.ToString();
                    lvi.SubItems.Add(logEvent.LogTime.ToString(@"HH\:mm\:ss.ffff"));
                    lvi.SubItems.Add(logEvent.LogText);
                    listView_log.Items.Add(lvi);
                    lvi.EnsureVisible();

                    chk_showErrors.Text = "      Show Errors (" + LogEvents.Where(log => log.LogType == LogTypes.ERROR)?.Count().ToString() + ")";
                    chk_showWarnings.Text = "      Show Warnings (" + LogEvents.Where(log => log.LogType == LogTypes.WARNING)?.Count().ToString() + ")";
                    chk_showInfos.Text = "      Show Infos (" + LogEvents.Where(log => log.LogType == LogTypes.INFO)?.Count().ToString() + ")";

                    ResizeLastColumn();
                }
            }));
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        public void ClearLog()
        {
            LogEvents.Clear();
            this.Invoke(new MethodInvoker(delegate ()
            {
                listView_log.Items.Clear();

                chk_showErrors.Text = "      Show Errors (0)";
                chk_showWarnings.Text = "      Show Warnings (0)";
                chk_showInfos.Text = "      Show Infos (0)";

                ResizeLastColumn();
            }));
        }

        /// <summary>
        /// Clear all log entries and reload them. This also applies all filters.
        /// </summary>
        private void RefreshLogEntries()
        {
            this.Invoke(new MethodInvoker(delegate () { listView_log.Items.Clear(); }));
            foreach (LogEvent logEvent in LogEvents)
            {
                WriteEvent(logEvent);       //Filtering is done in WriteEvent()
            }
        }

        /// <summary>
        /// Add the info, warning and error icon to the list views image list
        /// </summary>
        private void InitImageList()
        {
            if(imageListIcons == null)
            {
                imageListIcons = new ImageList();
                imageListIcons.ImageSize = new Size(20, 20);
            }
            listView_log.SmallImageList = imageListIcons;

            foreach(string logTypeStr in Enum.GetNames(typeof(LogTypes)))
            {
                if(!imageListIcons.Images.ContainsKey(logTypeStr))     // Check to see if the image collection contains an image for this log type, using the type as a key.
                {
                    Bitmap icon;
                    
                    if(logTypeStr == LogTypes.INFO.ToString())  { icon = SystemIcons.Information.ToBitmap(); }
                    else if (logTypeStr == LogTypes.WARNING.ToString()) { icon = SystemIcons.Warning.ToBitmap(); }
                    else { icon = SystemIcons.Error.ToBitmap(); }

                    imageListIcons.Images.Add(logTypeStr, icon);

                    icon.Dispose();
                }
            }
        }

        /// <summary>
        /// ShowErrors check box toggled
        /// </summary>
        private void chk_showErrors_CheckedChanged(object sender, EventArgs e)
        {
            ShowErrors = chk_showErrors.Checked;
        }

        /// <summary>
        /// ShowWarnings check box toggled
        /// </summary>
        private void chk_showWarnings_CheckedChanged(object sender, EventArgs e)
        {
            ShowWarnings = chk_showWarnings.Checked;
        }

        /// <summary>
        /// ShowInfos check box toggled
        /// </summary>
        private void chk_showInfos_CheckedChanged(object sender, EventArgs e)
        {
            ShowInfos = chk_showInfos.Checked;
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        private void btn_clearLog_Click(object sender, EventArgs e)
        {
            ClearLog();
        }

        /// <summary>
        /// Save all log entries
        /// </summary>
        private void btn_saveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.Title = "Save a location for the log file";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                foreach (LogEvent logEvent in LogEvents)
                {
                    writer.WriteLine(logEvent.LogType.ToString() + "\t" + logEvent.LogTime.ToString(@"dd.MM.yyyy HH\:mm\:ss.ffff") + "\t" + logEvent.LogText);
                }
                writer.Close();
            }
        }
    }
}

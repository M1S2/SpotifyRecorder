using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MahApps.Metro.Controls;
using SpotifyRecorder.GenericRecorder;

namespace SpotifyRecorder
{
    /// <summary>
    /// Interaktionslogik für FileNamePrototypeCreator.xaml
    /// </summary>
    public partial class FileNamePrototypeCreator : MetroWindow, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changed</param>
        /// see: https://docs.microsoft.com/de-de/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        //##############################################################################################################################################################################################

        #region Commands

        private ICommand _insertTitleCommand;
        public ICommand InsertTitleCommand
        {
            get
            {
                if (_insertTitleCommand == null)
                {
                    _insertTitleCommand = new WindowTheme.RelayCommand(param =>
                    {
                        InsertTextAtCursor(txt_Prototype, PROTOTYPSTRING_TITLE);
                    });
                }
                return _insertTitleCommand;
            }
        }

        private ICommand _insertInterpretCommand;
        public ICommand InsertInterpretCommand
        {
            get
            {
                if (_insertInterpretCommand == null)
                {
                    _insertInterpretCommand = new WindowTheme.RelayCommand(param =>
                    {
                        InsertTextAtCursor(txt_Prototype, PROTOTYPSTRING_INTERPRET);
                    });
                }
                return _insertInterpretCommand;
            }
        }

        private ICommand _insertAlbumCommand;
        public ICommand InsertAlbumCommand
        {
            get
            {
                if (_insertAlbumCommand == null)
                {
                    _insertAlbumCommand = new WindowTheme.RelayCommand(param =>
                    {
                        InsertTextAtCursor(txt_Prototype, PROTOTYPSTRING_ALBUM);
                    });
                }
                return _insertAlbumCommand;
            }
        }

        private ICommand _insertPlaylistCommand;
        public ICommand InsertPlaylistCommand
        {
            get
            {
                if (_insertPlaylistCommand == null)
                {
                    _insertPlaylistCommand = new WindowTheme.RelayCommand(param =>
                    {
                        InsertTextAtCursor(txt_Prototype, PROTOTYPSTRING_PLAYLIST);
                    });
                }
                return _insertPlaylistCommand;
            }
        }

        private ICommand _insertDirectorySeparatorCommand;
        public ICommand InsertDirectorySeparatorCommand
        {
            get
            {
                if (_insertDirectorySeparatorCommand == null)
                {
                    _insertDirectorySeparatorCommand = new WindowTheme.RelayCommand(param =>
                    {
                        InsertTextAtCursor(txt_Prototype, new string(System.IO.Path.DirectorySeparatorChar, 1));
                    });
                }
                return _insertDirectorySeparatorCommand;
            }
        }

        private ICommand _oKCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_oKCommand == null)
                {
                    _oKCommand = new WindowTheme.RelayCommand(param =>
                    {
                        DialogResult = true;
                        this.Close();
                    });
                }
                return _oKCommand;
            }
        }

        #endregion

        //##############################################################################################################################################################################################

        public const string PROTOTYPSTRING_RECORDPATH = "{RecordPath}";
        public const string PROTOTYPSTRING_TITLE = "{Title}";
        public const string PROTOTYPSTRING_INTERPRET = "{Interpret}";
        public const string PROTOTYPSTRING_ALBUM = "{Album}";
        public const string PROTOTYPSTRING_PLAYLIST = "{Playlist}";
        public const string PROTOTYPSTRING_DEFAULT = PROTOTYPSTRING_RECORDPATH + @"\" + PROTOTYPSTRING_INTERPRET + " - " + PROTOTYPSTRING_TITLE;

        //***********************************************************************************************************************************************************************************************************

        private string _recordPath = "";
        private string _title = "";
        private string _interpret = "";
        private string _album = "";
        private string _playlist = "";
        private RecorderFileExistModes _fileExistMode = RecorderFileExistModes.SKIP;

        //***********************************************************************************************************************************************************************************************************

        private string _fileNamePrototype;
        /// <summary>
        /// String that represents a filename with placeholders and without extension (for example "{RecordPath}\{Album}\{Interpret} - {Title}")
        /// </summary>
        public string FileNamePrototype
        {
            get { return _fileNamePrototype; }
            set { _fileNamePrototype = GetValidFileNamePrototyp(value); OnPropertyChanged(); OnPropertyChanged("SampleFileName"); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Formated sample file name string
        /// </summary>
        public string SampleFileName
        {
            get { return GetCompleteFileNameWithoutExtension(FileNamePrototype, _recordPath, _title, _interpret, _album, _playlist, _fileExistMode); }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Check if the given file name prototyp string is valid.
        /// </summary>
        /// <param name="fileNamePrototyp">file name prototyp to check</param>
        /// <returns>true -> valid; false -> not valid</returns>
        public static bool IsFileNamePrototypValid(string fileNamePrototyp)
        {
            string validFileNamePrototyp = GetValidFileNamePrototyp(fileNamePrototyp);
            return (fileNamePrototyp == validFileNamePrototyp);
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Remove all invalid signs from the file name prototyp string. If the string doesn't start with the PROTOTYPSTRING_RECORDPATH it is added.
        /// </summary>
        /// <param name="fileNamePrototyp">file name prototyp to check</param>
        /// <returns>valid file name prototyp</returns>
        public static string GetValidFileNamePrototyp(string fileNamePrototyp)
        {
            string validFileNamePrototyp = "";
            if (string.IsNullOrEmpty(fileNamePrototyp) || fileNamePrototyp == PROTOTYPSTRING_RECORDPATH + @"\")
            {
                validFileNamePrototyp = PROTOTYPSTRING_DEFAULT;
            }
            else
            {
                int firstIndexRecordPath = fileNamePrototyp.IndexOf(PROTOTYPSTRING_RECORDPATH);
                if (firstIndexRecordPath >= 0)
                {
                    fileNamePrototyp = fileNamePrototyp.Remove(0, firstIndexRecordPath + PROTOTYPSTRING_RECORDPATH.Length);
                }
                validFileNamePrototyp = PROTOTYPSTRING_RECORDPATH + @"\" + fileNamePrototyp;
            }

            validFileNamePrototyp = validFileNamePrototyp.Replace(@"\\", @"\");
            return validFileNamePrototyp;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Remove all illegal characters from the given file name
        /// </summary>
        /// <param name="filepath">complete file path</param>
        /// <returns>file name without illegal characters</returns>
        /// see: https://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
        public static string RemoveIllegalFileNamePathCharacters(string filepath)
        {
            string corrected_file_name = filepath;

            string[] filename_split = filepath.Split('\\', '/');

            string file_name = (filename_split.Length > 0 ? filename_split.Last() : System.IO.Path.GetFileName(filepath));
            string directory_name = System.IO.Path.GetDirectoryName(filepath.Remove(filepath.LastIndexOf(file_name))); //filepath.Replace(file_name, ""));

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                file_name = file_name.Replace(c.ToString(), "");
            }
            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                directory_name = directory_name.Replace(c.ToString(), "");
            }
            corrected_file_name = System.IO.Path.Combine(directory_name, file_name);
            return corrected_file_name;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Replace the placeholders in the file name prototyp string with the given attributes
        /// </summary>
        /// <param name="filenamePrototype">file name prototyp with placeholders</param>
        /// <param name="recordPath">text for the PROTOTYPSTRING_RECORDPATH placeholder</param>
        /// <param name="title">text for the PROTOTYPSTRING_TITLE placheolder</param>
        /// <param name="interpret">text for the PROTOTYPSTRING_INTERPRET placeholder</param>
        /// <param name="album">text for the PROTOTYPSTRING_ALBUM placeholder</param>
        /// <param name="playlist">text for the PROTOTYPSTRING_PLAYLIST placeholder</param>
        /// <param name="fileExistMode">What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)</param>
        /// <returns>file name string</returns>
        public static string GetCompleteFileNameWithoutExtension(string filenamePrototype, string recordPath, string title, string interpret, string album, string playlist, RecorderFileExistModes fileExistMode)
        {
            if(filenamePrototype == null) { return ""; }
            string filename = filenamePrototype;
            if (recordPath != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_RECORDPATH, recordPath, RegexOptions.IgnoreCase); }
            if (title != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_TITLE, title, RegexOptions.IgnoreCase); }
            if (interpret != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_INTERPRET, interpret, RegexOptions.IgnoreCase); }
            if (album != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_ALBUM, album, RegexOptions.IgnoreCase); }
            if (playlist != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_PLAYLIST, playlist, RegexOptions.IgnoreCase); }

            try
            {
                filename = RemoveIllegalFileNamePathCharacters(filename);
                if (fileExistMode == RecorderFileExistModes.CREATENEW && System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(filename)))
                {
                    string temp_filename = filename;
                    int cnt = -1;
                    do
                    {
                        cnt++;
                        if (cnt > 0)
                        {
                            temp_filename = filename + "_" + cnt.ToString("000");
                        }
                    } while (System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(temp_filename), System.IO.Path.GetFileNameWithoutExtension(temp_filename) + ".*").Length > 0);

                    filename = temp_filename;
                }
            }
            catch (Exception)
            {
                filename = GetCompleteFileNameWithoutExtension(PROTOTYPSTRING_DEFAULT, recordPath, title, interpret, album, playlist, fileExistMode);
            }

            filename = RemoveIllegalFileNamePathCharacters(filename);
            return filename;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Set all attributes to default values
        /// </summary>
        public FileNamePrototypeCreator()
        {
            InitializeComponent();
            _recordPath = "";
            _title = "No Title";
            _interpret = "No Interpret";
            _album = "No Album";
            _playlist = "No Playlist";
            _fileExistMode = RecorderFileExistModes.SKIP;
            FileNamePrototype = PROTOTYPSTRING_DEFAULT;
        }

        /// <summary>
        /// Set all attributes to the given values
        /// </summary>
        /// <param name="fileNamePrototype">file name prototyp with placeholders</param>
        /// <param name="recordPath">record path</param>
        /// <param name="title">title</param>
        /// <param name="interpret">interpret</param>
        /// <param name="album">album name</param>
        /// <param name="playlist">playlist name</param>
        /// <param name="fileExistMode">What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)</param>
        public FileNamePrototypeCreator(string fileNamePrototype, string recordPath, string title, string interpret, string album, string playlist, RecorderFileExistModes fileExistMode)
        {
            InitializeComponent();
            _recordPath = recordPath;
            _title = title;
            _interpret = interpret;
            _album = album;
            _playlist = playlist;
            _fileExistMode = fileExistMode;
            FileNamePrototype = fileNamePrototype;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Insert the given text at the current position of the cursor in the given TextBox
        /// </summary>
        /// <param name="txtBox">TextBox to insert the text into</param>
        /// <param name="text">text to insert in the TextBox</param>
        /// see: https://stackoverflow.com/questions/1416454/how-to-paste-text-in-textbox-current-cursor
        private void InsertTextAtCursor(TextBox txtBox, string text)
        {
            int selectionIndex = txtBox.SelectionStart;
            FileNamePrototype = FileNamePrototype.Insert(selectionIndex, text);
            txtBox.SelectionStart = selectionIndex + text.Length;
        }
    }
}

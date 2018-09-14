using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace Spotify_Recorder
{
    public partial class FileNameSubFolderCreator : Form
    {
        public const string PROTOTYPSTRING_RECORDPATH = "{RecordPath}";
        public const string PROTOTYPSTRING_TITLE = "{Title}";
        public const string PROTOTYPSTRING_INTERPRET = "{Interpret}";
        public const string PROTOTYPSTRING_ALBUM = "{Album}";
        public const string PROTOTYPSTRING_DEFAULT = PROTOTYPSTRING_RECORDPATH + @"\" + PROTOTYPSTRING_INTERPRET + " - " + PROTOTYPSTRING_TITLE;

        //***********************************************************************************************************************************************************************************************************

        private string _recordPath = "";
        private string _title = "";
        private string _interpret = "";
        private string _album = "";
        private FileExistModes _fileExistMode = FileExistModes.SKIP;

        //***********************************************************************************************************************************************************************************************************

        private string _fileNamePrototype;
        /// <summary>
        /// String that represents a filename with placeholders and without extension (for example "{RecordPath}\{Album}\{Interpret} - {Title}")
        /// </summary>
        public string FileNamePrototype 
        {
            get { return _fileNamePrototype; }
            set { _fileNamePrototype = GetValidFileNamePrototyp(value);  }
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
                if(firstIndexRecordPath >= 0)
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

            string file_name = (filename_split.Length > 0 ? filename_split.Last() : Path.GetFileName(filepath));
            string directory_name = Path.GetDirectoryName(filepath.Replace(file_name, ""));
            
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                file_name = file_name.Replace(c.ToString(), "");
            }
            foreach (char c in Path.GetInvalidPathChars())
            {
                directory_name = directory_name.Replace(c.ToString(), "");
            }
            corrected_file_name = Path.Combine(directory_name, file_name);
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
        /// <param name="fileExistMode">What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)</param>
        /// <returns>file name string</returns>
        public static string GetCompleteFileNameWithoutExtension(string filenamePrototype, string recordPath, string title, string interpret, string album, FileExistModes fileExistMode)
        {
            string filename = filenamePrototype;
            if (recordPath != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_RECORDPATH, recordPath, RegexOptions.IgnoreCase); }
            if (title != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_TITLE, title, RegexOptions.IgnoreCase); }
            if (interpret != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_INTERPRET, interpret, RegexOptions.IgnoreCase); }
            if (album != null) { filename = Regex.Replace(filename, PROTOTYPSTRING_ALBUM, album, RegexOptions.IgnoreCase); }

            try
            {
                filename = RemoveIllegalFileNamePathCharacters(filename);
                if (fileExistMode == FileExistModes.CREATENEW && Directory.Exists(Path.GetDirectoryName(filename)))
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
                    } while (Directory.GetFiles(Path.GetDirectoryName(temp_filename), Path.GetFileNameWithoutExtension(temp_filename) + ".*").Length > 0);

                    filename = temp_filename;
                }
            }
            catch(Exception)
            {
                filename = GetCompleteFileNameWithoutExtension(PROTOTYPSTRING_DEFAULT, recordPath, title, interpret, album, fileExistMode);
            }

            filename = RemoveIllegalFileNamePathCharacters(filename);
            return filename;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Set all attributes to default values
        /// </summary>
        public FileNameSubFolderCreator()
        {
            InitializeComponent();
            FileNamePrototype = PROTOTYPSTRING_DEFAULT;
            _recordPath = "";
            _title = "No Title";
            _interpret = "No Interpret";
            _album = "No Album";
            _fileExistMode = FileExistModes.SKIP;

            txt_filename_prototype.Text = FileNamePrototype;
            btn_directory_separator.Text = new string(Path.DirectorySeparatorChar, 1);
            SetLblFileName();
        }

        /// <summary>
        /// Set all attributes to the given values
        /// </summary>
        /// <param name="fileNamePrototype">file name prototyp with placeholders</param>
        /// <param name="recordPath">record path</param>
        /// <param name="title">title</param>
        /// <param name="interpret">interpret</param>
        /// <param name="album">album name</param>
        /// <param name="fileExistMode">What should be done if the file already exists. Skip and don't record anything, override the existing file, create a new file with another filename (number appended)</param>
        public FileNameSubFolderCreator(string fileNamePrototype, string recordPath, string title, string interpret, string album, FileExistModes fileExistMode)
        {
            InitializeComponent();
            FileNamePrototype = fileNamePrototype;
            _recordPath = recordPath;
            _title = title;
            _interpret = interpret;
            _album = album;
            _fileExistMode = fileExistMode;

            txt_filename_prototype.Text = FileNamePrototype;
            btn_directory_separator.Text = new string(Path.DirectorySeparatorChar, 1);
            SetLblFileName();
        }

        //***********************************************************************************************************************************************************************************************************

        private void SetLblFileName()
        {
            lbl_sample_file_name.Text = GetCompleteFileNameWithoutExtension(FileNamePrototype, _recordPath, _title, _interpret, _album, _fileExistMode);
        }

        //***********************************************************************************************************************************************************************************************************

        private void txt_filename_prototype_TextChanged(object sender, EventArgs e)
        {
            FileNamePrototype = txt_filename_prototype.Text;
            SetLblFileName();
        }

        //***********************************************************************************************************************************************************************************************************

        private void btn_title_Click(object sender, EventArgs e)
        {
            InsertTextAtCursor(txt_filename_prototype, PROTOTYPSTRING_TITLE);
        }

        private void btn_interpret_Click(object sender, EventArgs e)
        {
            InsertTextAtCursor(txt_filename_prototype, PROTOTYPSTRING_INTERPRET);
        }

        private void btn_album_Click(object sender, EventArgs e)
        {
            InsertTextAtCursor(txt_filename_prototype, PROTOTYPSTRING_ALBUM);
        }

        private void btn_directory_separator_Click(object sender, EventArgs e)
        {
            InsertTextAtCursor(txt_filename_prototype, new string(Path.DirectorySeparatorChar, 1));
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
            txtBox.Text = txtBox.Text.Insert(selectionIndex, text);
            txtBox.SelectionStart = selectionIndex + text.Length;
        }

    }
}

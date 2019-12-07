using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpotifyRecorder.GenericRecorder;

namespace SpotifyRecorder
{
    public class RecorderSettings : ICloneable
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

        private string _basePath;
        public string BasePath
        {
            get { return _basePath; }
            set { _basePath = value; OnPropertyChanged(); }
        }

        private string _fileNamePrototype;
        public string FileNamePrototype
        {
            get { return _fileNamePrototype; }
            set { _fileNamePrototype = value; OnPropertyChanged(); }
        }

        private RecorderFileExistModes _fileExistMode;
        public RecorderFileExistModes FileExistMode
        {
            get { return _fileExistMode; }
            set { _fileExistMode = value; OnPropertyChanged(); }
        }

        private RecordFormats _recordFormat;
        public RecordFormats RecordFormat
        {
            get { return _recordFormat; }
            set { _recordFormat = value; OnPropertyChanged(); }
        }

        private string _recorderDeviceName;
        public string RecorderDeviceName
        {
            get { return _recorderDeviceName; }
            set { _recorderDeviceName = value; OnPropertyChanged(); }
        }

        //##############################################################################################################################################################################################

        public RecorderSettings()
        {
            BasePath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            FileNamePrototype = FileNamePrototypeCreator.PROTOTYPSTRING_DEFAULT;
            FileExistMode = RecorderFileExistModes.SKIP;
            RecordFormat = RecordFormats.MP3;
            RecorderDeviceName = "CABLE Input (VB-Audio Virtual Cable)";
        }

        public object Clone() => this.MemberwiseClone();

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj)
        {
            RecorderSettings settings1 = (RecorderSettings)obj;
            return this.BasePath == settings1.BasePath &&
                this.FileNamePrototype == settings1.FileNamePrototype &&
                this.FileExistMode == settings1.FileExistMode &&
                this.RecordFormat == settings1.RecordFormat &&
                this.RecorderDeviceName == settings1.RecorderDeviceName;
        }
    }
}

using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyRecorder.WindowTheme
{
    /// <summary>
    /// Interaction logic for DarkThemeControl.xaml
    /// </summary>
    public partial class DarkThemeControl : UserControl, INotifyPropertyChanged
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

        private bool _isDarkTheme;
        /// <summary>
        /// Is the used theme the dark theme
        /// </summary>
        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged();

                ThemeManager.Current.ChangeThemeBaseColor(Application.Current, (IsDarkTheme ? "Dark" : "Light"));
            }
        }

        //##############################################################################################################################################################################################

        public DarkThemeControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += DarkThemeControl_Loaded;
        }

        private void DarkThemeControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += Window_Closing;

            // Add new User scope property "DarkTheme"
            SettingsProperty darkThemeProp = new SettingsProperty("DarkTheme")
            {
                DefaultValue = false,
                IsReadOnly = false,
                PropertyType = typeof(bool),
                Provider = Properties.Settings.Default.Providers["LocalFileSettingsProvider"],
            };
            darkThemeProp.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            Properties.Settings.Default.Properties.Add(darkThemeProp);

            // Reload the setting to initialize the new property with the saved value
            Properties.Settings.Default.Reload();

            IsDarkTheme = (bool)Properties.Settings.Default["DarkTheme"];
        }
        
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default["DarkTheme"] = IsDarkTheme;
            Properties.Settings.Default.Save();
        }
    }
}

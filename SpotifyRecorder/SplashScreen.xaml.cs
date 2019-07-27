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

namespace SpotifyRecorder
{
    /// <summary>
    /// Interaktionslogik für SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public static readonly DependencyProperty StatusStringProperty = DependencyProperty.Register("StatusString", typeof(string), typeof(SplashScreen), new PropertyMetadata("Loading..."));

        public string StatusString
        {
            get { return (string)this.GetValue(StatusStringProperty); }
            set { this.SetValue(StatusStringProperty, value); }
        }

        public SplashScreen()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}

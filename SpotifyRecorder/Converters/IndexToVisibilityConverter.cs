using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SpotifyRecorder.Converters
{
    /// <summary>
    /// Converter that can be used to hide the last element in an list view
    /// </summary>
    /// see: https://social.msdn.microsoft.com/Forums/vstudio/en-US/ede0480b-ad56-4a02-8213-9ff9e1d2570a/itemscontrol-separators?forum=wpf
    public class IndexToVisibilityConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Return Visibility.Collapsed if value[1] is the last element in value[0]
        /// </summary>
        /// <param name="values">Converter input: value[0] is a list of type T, value[1] is a object of the same type T</param>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ICollection list = values[0] as ICollection;
            if (list == null) { return Visibility.Visible; }

            int index = 0;
            foreach(object obj in list)
            {
                if(obj.Equals(values[1])) { break; }
                index++;
            }
            return index == list.Count - 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null; //Not needed
        }

        #endregion
    }
}

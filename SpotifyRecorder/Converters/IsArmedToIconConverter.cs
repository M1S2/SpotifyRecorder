﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace SpotifyRecorder.Converters
{
    /// <summary>
    /// Convert IsArmed property to GeometryDrawing representation of the armed state icon
    /// </summary>
    [ValueConversion(typeof(bool), typeof(GeometryDrawing))]
    public class IsArmedToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Geometry iconGeometry = null;
            Brush foregroundBrush = App.Current.TryFindResource("GrayBrush3") as Brush;

            if ((bool)value) { iconGeometry = Geometry.Parse((new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.MicrophoneAltSolid }).Data); }
            else { iconGeometry = Geometry.Parse((new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.MicrophoneSlashSolid }).Data); }

            GeometryDrawing iconGeometryDrawing = new GeometryDrawing(foregroundBrush, new Pen(foregroundBrush, 1), iconGeometry);
            return iconGeometryDrawing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

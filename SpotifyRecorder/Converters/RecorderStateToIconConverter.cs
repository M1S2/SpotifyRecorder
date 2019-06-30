using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.IconPacks;
using SpotifyRecorder.GenericRecorder;

namespace SpotifyRecorder.Converters
{
    /// <summary>
    /// Convert recorder state to GeometryDrawing representation of the recorder state icon
    /// </summary>
    [ValueConversion(typeof(RecordStates), typeof(GeometryDrawing))]
    public class RecorderStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Geometry iconGeometry = null;
            Brush foregroundBrush = App.Current.TryFindResource("AccentBaseColorBrush") as Brush;

            switch((RecordStates)value)
            {
                case RecordStates.STOPPED: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.Stop }).Data); break;
                case RecordStates.PAUSED: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.Pause }).Data); break;
                case RecordStates.RECORDING: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.Record }).Data); break;
                case RecordStates.NORMALIZING: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.TuneVertical }).Data); break;
                case RecordStates.WAV_TO_MP3: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.Sync }).Data); break;
                case RecordStates.REMOVING_FADES: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.CloseCircle }).Data); break;
                case RecordStates.ADDING_TAGS: iconGeometry = Geometry.Parse((new PackIconMaterial() { Kind = PackIconMaterialKind.Tag }).Data); break;
            }

            GeometryDrawing iconGeometryDrawing = new GeometryDrawing(foregroundBrush, new Pen(foregroundBrush, 1), iconGeometry);
            return iconGeometryDrawing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

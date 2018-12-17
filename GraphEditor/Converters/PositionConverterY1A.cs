using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace GraphEditor.Converters
{
    class PositionConverterY1A : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Edge edge = value as Edge;
            double dx = edge.FirstVertex.Position.X - edge.SecondVertex.Position.X;
            double dy = edge.FirstVertex.Position.Y - edge.SecondVertex.Position.Y;
            double norm = Math.Sqrt(dx * dx + dy * dy);
            double udy = dy / norm;

            return edge.SecondVertex.Position.Y + udy * 25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}

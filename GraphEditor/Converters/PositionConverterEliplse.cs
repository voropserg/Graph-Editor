using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace GraphEditor.Converters
{
    class PositionConverterEliplse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

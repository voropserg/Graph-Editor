﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace GraphEditor.Converters
{
    class PositionConverterY2A : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Edge edge = value as Edge;
            double dx = edge.FirstVertex.Position.X - edge.SecondVertex.Position.X;
            double dy = edge.FirstVertex.Position.Y - edge.SecondVertex.Position.Y;
            double norm = Math.Sqrt(dx * dx + dy * dy);
            double udx = dx / norm;
            double udy = dy / norm;
            double ay = udx * 1 / 2 + udy * Math.Sqrt(3) / 2;
            double by = -udx * 1 / 2 + udy * Math.Sqrt(3) / 2;

            return edge.SecondVertex.Position.X + (udy * 25) + by * 25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
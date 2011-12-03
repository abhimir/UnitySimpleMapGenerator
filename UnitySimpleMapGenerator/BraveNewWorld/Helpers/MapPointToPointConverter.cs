using System;
using System.Windows.Data;
using System.Windows.Media;
using BraveNewWorld.Models;

namespace BraveNewWorld.Helpers
{
    public class MapPointToPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ps = (Center)value;

            PointCollection polygonPoints = new PointCollection();

            foreach (Corner mp in ps.Corners)
            {
                polygonPoints.Add(new System.Windows.Point(mp.Point.X - ps.Point.X, mp.Point.Y - ps.Point.Y));
            }

            return polygonPoints;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
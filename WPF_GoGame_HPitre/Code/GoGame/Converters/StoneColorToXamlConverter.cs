using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GoGame.Model.Enums;

namespace GoGame.Converters
{
    public class StoneColorToXamlConverter : IValueConverter
    {
        public Brush BlackBrush { get; set; } = Brushes.Black;
        public Brush WhiteBrush { get; set; } = Brushes.White;
        public Brush NoneBrush { get; set; } = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not StoneColor color)
                return NoneBrush;

            return color switch
            {
                StoneColor.Black => BlackBrush,
                StoneColor.White => WhiteBrush,
                _ => NoneBrush
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GoGame.Model.Enums;

namespace GoGame.Converters
{
    public class SecondsToTextTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int seconds)
                return "00:00";

            //Format : secondes / 60 = minutes, restant quand secondes/60 = secondes restantes

            int minutes = seconds / 60;
            int restant = seconds % 60;

            if (minutes != 0)
                return $"{minutes:00}:{restant:00} minutes";
            else
                return $"{restant:00} secondes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
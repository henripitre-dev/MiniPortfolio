using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GoGame.Model;
using GoGame.Model.Enums;

namespace GoGame.Converters
{
    public class MoveToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Move m && m != null)
            {
                string color;
                switch (m.Player.Color)
                {
                    case StoneColor.Black:
                        color = "Noir";
                        break;
                    case StoneColor.White:
                        color = "Blanc";
                        break;
                    default:
                        color = "Inconnu";
                        break;
                }
                if (m.IsPass) return $"{color} passe son tour";

                return $"" +
                    $"{color} joue en {m.Coords.ToString()}";

            }
            else return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

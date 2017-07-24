using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Screentroll.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Inverted { get;  set; }
        public BoolToVisibilityConverter()
        {
            Inverted = false;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
            {
                var val = (bool)value;
                
                if (Inverted) val = !val;
                return (val ? Visibility.Visible : Visibility.Hidden);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                var val = (Visibility)value;
                var ret =  (val == Visibility.Hidden ? false:true);
                if (Inverted) ret = !ret;
            }
            return null;
        }
    }
}

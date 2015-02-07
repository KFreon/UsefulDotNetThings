using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UsefulThings.WPF
{
    [ValueConversion(typeof(bool?), typeof(Visibility), ParameterType=typeof(bool))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            bool? val = null;

            // KFreon: Cast from correct type. I'm sure there's a better way to do this, but I can't figure it out.
            Type type = value.GetType();
            if (type == typeof(bool))
                val = (bool)value;
            else if (type == typeof(bool?))
                val = (bool?)value;

            // KFreon: Invert if required
            if (parameter != null && (bool)parameter)
                val = !val;


            // KFreon: Convert to Visibility
            if (val == true)
                visibility = Visibility.Visible;
            else if (val == null)
                visibility = Visibility.Hidden;
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

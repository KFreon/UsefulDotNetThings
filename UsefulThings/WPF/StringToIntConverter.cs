using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF
{
    [ValueConversion(typeof(string), typeof(int))]
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(string))
                throw new InvalidOperationException("Value must be a single character string");

            string val = (string)value;
            if (val.Length > 1)
                throw new InvalidOperationException("Value must be a single character string");

            int res = -1;
            if (!Int32.TryParse(val, out res))
                throw new InvalidOperationException("Conversion failed.");

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

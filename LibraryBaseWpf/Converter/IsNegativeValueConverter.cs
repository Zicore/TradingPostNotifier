using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LibraryBase.Wpf.Converter
{
    public class IsNegativeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Double doubleValue = 0.0;
            if (value != null)
            {
                try
                {
                    Double.TryParse(value.ToString(), out doubleValue);
                }
                catch { }
            }

            return doubleValue < 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

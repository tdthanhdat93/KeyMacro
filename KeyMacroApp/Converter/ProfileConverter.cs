using KeyMacroApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KeyMacroApp.Converter
{
    [ValueConversion(typeof(ProfileBindKey), typeof(ProfileBindKey))]
    public class SelectProfileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProfileBindKey)
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}

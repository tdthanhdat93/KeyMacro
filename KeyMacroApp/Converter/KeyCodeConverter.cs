using KeyMacroApp.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace KeyMacroApp.Converter
{
    [ValueConversion(typeof(uint), typeof(string))]  // Key code <-> Key name
    class KeyCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value?.ToString() ?? string.Empty;
            uint keyCode = ConvertHelper.HexStringToUInt(strValue);

            return ConvertHelper.GetKeyName(keyCode);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

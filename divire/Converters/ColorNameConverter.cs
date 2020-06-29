//
//  divire
//
//  Copyright (C) 2020 Aru Nanika
//
//  This program is released under the MIT License.
//  https://opensource.org/licenses/MIT
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace divire.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class ColorNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorName = "#00000000";
            var numericValue = System.Convert.ToInt32(value);

            if (0 <= numericValue)
            {
                colorName = $"#{((int)value & 0xFFFFFF):X6}";
            }

            return colorName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorName = (string)value;
            var numericPart = colorName.Replace("#", string.Empty);
            var result = System.Convert.ToInt32(numericPart, 16);

            return result;
        }
    }
}

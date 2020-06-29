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
using divire.Protocols;

namespace divire.Converters
{
    [ValueConversion(typeof(OperationProtocol.BlendMode), typeof(string))]
    public class BlendModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OperationProtocol.GetBlendModeDetail((OperationProtocol.BlendMode)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var detail = (null != value) ? (string)value : string.Empty;
            return OperationProtocol.GetBlendMode(detail);
        }
    }
}

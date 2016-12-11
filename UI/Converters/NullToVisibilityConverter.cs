using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace LegoTuringMachine.UI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; }
        public Visibility NotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result = value == null ? NullValue : NotNullValue;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

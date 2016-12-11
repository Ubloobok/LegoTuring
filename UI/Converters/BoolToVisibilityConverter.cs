using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace LegoTuringMachine.UI.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public Visibility FalseValue { get; set; }
		public Visibility TrueValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility result = FalseValue;
			if (value is bool)
			{
				result = (bool)value ? TrueValue : FalseValue;
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

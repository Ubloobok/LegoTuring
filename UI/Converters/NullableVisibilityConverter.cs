using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace LegoTuringMachine.UI.Converters
{
	public class NullableVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility result = Visibility.Visible;
			if (value == null)
			{
				result = Visibility.Hidden;
			}
			if (value is int?)
			{
				result = ((int?)value).HasValue ? Visibility.Visible : Visibility.Hidden;
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

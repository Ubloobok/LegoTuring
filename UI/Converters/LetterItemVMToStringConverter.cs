using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LegoTuringMachine.UI.Converters
{
    public class LetterItemVMToStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets string for empty letter value.
        /// </summary>
        public string EmptyLetterValueString { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = null;
            var letter = value as LetterItemVM;
            if (letter != null)
            {
                result = letter.IsEmptyValue ? EmptyLetterValueString : letter.DisplayValue;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

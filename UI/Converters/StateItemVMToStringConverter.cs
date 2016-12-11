using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LegoTuringMachine.UI.Converters
{
    public class StateItemVMToStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets string for final state.
        /// </summary>
        public string FinalStateString { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = null;
            var state = value as StateItemVM;
            if (state != null)
            {
                result = state.IsFinalState ? FinalStateString : state.Name;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

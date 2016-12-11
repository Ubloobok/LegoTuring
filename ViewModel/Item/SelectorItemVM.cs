using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;

namespace LegoTuringMachine.ViewModel.Item
{
	public class SelectorItemVM : ViewModelBase
	{
		public SelectorItemVM()
		{
		}

		public SelectorItemVM(string displayValue, object value)
		{
			DisplayValue = displayValue;
			Value = value;
		}

		private string _displayValue;

		public string DisplayValue
		{
			get { return _displayValue; }
			set
			{
				if (value != _displayValue)
				{
					RaisePropertyChanging<string>(() => DisplayValue);
					_displayValue = value;
					RaisePropertyChanged<string>(() => DisplayValue);
				}
			}
		}

		private object _value;

		public object Value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					RaisePropertyChanging<object>(() => Value);
					_value = value;
					RaisePropertyChanged<object>(() => Value);
				}
			}
		}
	}
}

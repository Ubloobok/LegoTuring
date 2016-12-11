using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;

namespace LegoTuringMachine.ViewModel.Item
{
	public class StateItemVM : ViewModelBase
	{
		private bool _isInitialState;
		private bool _isFinalState;
		private string _name;

		public bool IsInitialState
		{
			get { return _isInitialState; }
			set
			{
				if (value != _isInitialState)
				{
					RaisePropertyChanging<bool>(() => IsInitialState);
					_isInitialState = value;
					RaisePropertyChanged<bool>(() => IsInitialState);
				}
			}
		}

		public bool IsFinalState
		{
			get { return _isFinalState; }
			set
			{
				if (value != _isFinalState)
				{
					RaisePropertyChanging<bool>(() => IsFinalState);
					_isFinalState = value;
					RaisePropertyChanged<bool>(() => IsFinalState);
				}
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if (value != _name)
				{
					RaisePropertyChanging<string>(() => Name);
					_name = value;
					RaisePropertyChanged<string>(() => Name);
				}
			}
		}

        public override string ToString()
        {
            string str = string.Format("Name=\"{0}\", IsInitialState={1}, IsFinalState={2}", Name, IsInitialState, IsFinalState);
            return str;
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.Device;

namespace LegoTuringMachine.ViewModel.Item
{
	public class RuleItemVM : ViewModelBase
	{
        private bool _hasError;
        private string _error;
		private StateItemVM _conditionState;
		private LetterItemVM _conditionLetter;
		private StateItemVM _newState;
		private LetterItemVM _newLetter;
		private Direction _newPosition;

        public bool HasError
        {
            get { return _hasError; }
            set
            {
                if (value != _hasError)
                {
                    RaisePropertyChanging<bool>(() => HasError);
                    _hasError = value;
                    RaisePropertyChanged<bool>(() => HasError);
                }
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                if (value != _error)
                {
                    RaisePropertyChanging<string>(() => Error);
                    _error = value;
                    RaisePropertyChanged<string>(() => Error);
                }
            }
        }

		public StateItemVM ConditionState
		{
			get { return _conditionState; }
			set
			{
				if (value != _conditionState)
				{
					RaisePropertyChanging<StateItemVM>(() => ConditionState);
					_conditionState = value;
					RaisePropertyChanged<StateItemVM>(() => ConditionState);
				}
			}
		}

		public LetterItemVM ConditionLetter
		{
			get { return _conditionLetter; }
			set
			{
				if (value != _conditionLetter)
				{
					RaisePropertyChanging<LetterItemVM>(() => ConditionLetter);
					_conditionLetter = value;
					RaisePropertyChanged<LetterItemVM>(() => ConditionLetter);
				}
			}
		}

		public StateItemVM NewState
		{
			get { return _newState; }
			set
			{
				if (value != _newState)
				{
					RaisePropertyChanging<StateItemVM>(() => NewState);
					_newState = value;
					RaisePropertyChanged<StateItemVM>(() => NewState);
				}
			}
		}

		public LetterItemVM NewLetter
		{
			get { return _newLetter; }
			set
			{
				if (value != _newLetter)
				{
					RaisePropertyChanging<LetterItemVM>(() => NewLetter);
					_newLetter = value;
					RaisePropertyChanged<LetterItemVM>(() => NewLetter);
				}
			}
		}

		public Direction NewPosition
		{
			get { return _newPosition; }
			set
			{
				if (value != _newPosition)
				{
					RaisePropertyChanging<Direction>(() => NewPosition);
					_newPosition = value;
					RaisePropertyChanged<Direction>(() => NewPosition);
				}
			}
		}
	}
}

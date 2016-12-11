using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LegoTuringMachine.ViewModel.Item
{
	public enum CellActivationState
	{
        Deactivated = 0,
		Activated = 1,
	}

	public enum CellValueState
	{
		Known,
		Unknown,
		Unused
	}

	public class CellItemVM : ViewModelBase
	{
		private LetterItemVM _oldValue;
		private LetterItemVM _currentValue;
		private LetterItemVM _newValue;
		private CellValueState _valueState;
        private bool _isActive;

		private BasicConfigItemVM _attachedBasicConfig;

		public BasicConfigItemVM AttachedBasicConfig
		{
			get { return _attachedBasicConfig; }
			set
			{
				if (value != _attachedBasicConfig)
				{
					var oldValue = value;
					RaisePropertyChanging<BasicConfigItemVM>(() => AttachedBasicConfig);
					_attachedBasicConfig = value;
					RaisePropertyChanged<BasicConfigItemVM>(() => AttachedBasicConfig);
					OnAttachedBasicConfigChanged(oldValue, value);
				}
			}
		}

		private void OnAttachedBasicConfigChanged(BasicConfigItemVM oldValue, BasicConfigItemVM newValue)
		{
			if (OldValue != null)
			{
				OldValue.AttachedBasicConfig = newValue;
			}
			if (CurrentValue != null)
			{
				CurrentValue.AttachedBasicConfig = newValue;
			}
			if (NewValue != null)
			{
				NewValue.AttachedBasicConfig = newValue;
			}
		}

		public LetterItemVM OldValue
		{
			get { return _oldValue; }
			set
			{
				if (value != _oldValue)
				{
					RaisePropertyChanging<LetterItemVM>(() => OldValue);
					_oldValue = value;
					RaisePropertyChanged<LetterItemVM>(() => OldValue);
				}
			}
		}

		public LetterItemVM CurrentValue
		{
			get { return _currentValue; }
			set
			{
				if (value != _currentValue)
				{
					if (_currentValue != null)
					{
						_currentValue.PropertyChanged -= OnCurrentValuePropertyChanged;
					}
					RaisePropertyChanging(() => CurrentValue);
					_currentValue = value;
					if (value != null)
					{
						_currentValue.PropertyChanged -= OnCurrentValuePropertyChanged;
						_currentValue.PropertyChanged += OnCurrentValuePropertyChanged;
					}
					RaisePropertyChanged(() => CurrentValue);
				}
			}
		}

		private void OnCurrentValuePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanging(() => CurrentValue);
			RaisePropertyChanged(() => CurrentValue);
		}

		public LetterItemVM NewValue
		{
			get { return _newValue; }
			set
			{
				if (value != _newValue)
				{
					if (_newValue != null)
					{
						_newValue.PropertyChanged -= OnNewValuePropertyChanged;
					}
					RaisePropertyChanging(() => NewValue);
					_newValue = value;
					if (value != null)
					{
						_newValue.PropertyChanged -= OnNewValuePropertyChanged;
						_newValue.PropertyChanged += OnNewValuePropertyChanged;
					}
					RaisePropertyChanged(() => NewValue);
				}
			}
		}

		private void OnNewValuePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanging(() => NewValue);
			RaisePropertyChanged(() => NewValue);
		}

        public CellValueState ValueState
        {
            get { return _valueState; }
            set
            {
                RaisePropertyChanging<CellValueState>(() => ValueState);
                _valueState = value;
                RaisePropertyChanged<CellValueState>(() => ValueState);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                RaisePropertyChanging<bool>(() => IsActive);
                _isActive = value;
                RaisePropertyChanged<bool>(() => IsActive);
            }
        }
	}
}

using LegoTuringMachine.Device;
using LegoTuringMachine.Model;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LegoTuringMachine.Extensions;
using LegoTuringMachine.Implements;

namespace LegoTuringMachine.ViewModel
{
	public class StudentTuringMachineVM : BaseTuringMachineVM
	{
		public const string EmptyLetterValueSynonym = " ";

		private bool _isSelectCurrentConfigMode;
		private string _inputString;
		private string _inputMessage;

		public StudentTuringMachineVM(IControlDevice controlDevice, ISettingsFacade settingsFacade, SharedValue<int?> sharedActiveCellIndex)
			: base(controlDevice, settingsFacade, sharedActiveCellIndex)
		{
			Configs = new SelectableCollection<ComplexConfigItemVM>();
			Configs.SelectedItemChanged += (o, e) => SetCurrentConfigCommand.RaiseCanExecuteChanged();
			SetCurrentConfigCommand = new DelegateCommand<ComplexConfigItemVM>(ExecuteSetCurrentConfigCommand, CanExecuteCurrentConfigCommand);
			EnableSelectCurrentConfigModeCommand = new ActionCommand(() => IsSelectCurrentConfigMode = true);
			DisableSelectCurrentConfigModeCommand = new ActionCommand(() => IsSelectCurrentConfigMode = false);
			Initialize();
		}

		private void Initialize()
		{
			var commonConfig = SettingsFacade.LoadCommonSettings();
			for (int i = 0; i < commonConfig.BlocksCount; i++)
			{
				Cells.Add(new CellItemVM()
				{
					IsActive = false,
					ValueState = CellValueState.Unused,
				});
			}

			var preSets = SettingsFacade.LoadPreSetConfigs();
			Configs.AddRange(preSets);

			App.GlobalDispatcher.BeginInvoke(new Action(() =>
			{
				Configs.SelectedItem = preSets.FirstOrDefault();
			}));
			InputMessage = null;
		}

		/// <summary>
		/// Gets or sets input string.
		/// </summary>
		public string InputString
		{
			get { return _inputString; }
			set
			{
				RaisePropertyChanging<string>(() => InputString);
				_inputString = value;
				RaisePropertyChanged<string>(() => InputString);
				OnInputStringChanged();
			}
		}

		/// <summary>
		/// Gets or sets message for input string with some errors, information and etc.
		/// </summary>
		public string InputMessage
		{
			get { return _inputMessage; }
			private set
			{
				if (value != _inputMessage)
				{
					RaisePropertyChanging<string>(() => InputMessage);
					_inputMessage = value;
					RaisePropertyChanged<string>(() => InputMessage);
				}
			}
		}

		/// <summary>
		/// Gets collection of all available configurations.
		/// </summary>
		public SelectableCollection<ComplexConfigItemVM> Configs { get; private set; }

		/// <summary>
		/// Gets command for set current configuration.
		/// </summary>
		public DelegateCommand<ComplexConfigItemVM> SetCurrentConfigCommand { get; private set; }

		/// <summary>
		/// Gets command for enable current config selection mode.
		/// </summary>
		public ActionCommand EnableSelectCurrentConfigModeCommand { get; private set; }

		/// <summary>
		/// Gets command for disable current config selection mode.
		/// </summary>
		public ActionCommand DisableSelectCurrentConfigModeCommand { get; private set; }

		/// <summary>
		/// Gets flag which indicates that now is current config selection mode enabled or disabled.
		/// </summary>
		public bool IsSelectCurrentConfigMode
		{
			get { return _isSelectCurrentConfigMode; }
			private set
			{
				_isSelectCurrentConfigMode = value;
				RaisePropertyChanged(() => IsSelectCurrentConfigMode);
			}
		}

		private void ExecuteSetCurrentConfigCommand(ComplexConfigItemVM newConfig)
		{
			var oldConfig = CurrentConfig;
			CurrentConfig = newConfig;
			foreach (var cell in Cells)
			{
				if (newConfig != null)
				{
					cell.AttachedBasicConfig = newConfig.BasicConfig;
					if (cell.CurrentValue == null)
					{
						cell.CurrentValue = newConfig.BasicConfig.Alphabet.FirstOrDefault(a => a.IsEmptyValue);
						cell.ValueState = CellValueState.Unknown;
					}
				}
				if ((oldConfig == null)
					|| (newConfig == null)
					|| (newConfig.BasicConfig.Alphabet.Count != oldConfig.BasicConfig.Alphabet.Count))
				{
					cell.ValueState = CellValueState.Unknown;
				}
			}

			InputString = InputString;
			IsSelectCurrentConfigMode = false;
		}

		private bool CanExecuteCurrentConfigCommand(ComplexConfigItemVM newConfig)
		{
			return newConfig != null;
		}

		private void OnInputStringChanged()
		{
			string newInputMessageValue = null;
			if (!string.IsNullOrEmpty(InputString))
			{
				if (CurrentConfig == null)
				{
					newInputMessageValue = "Выберите Алгоритм";
				}
				else
				{
					var notExistsLetters = InputString
						.Select(c => c.ToString())
						.Where(c => CurrentConfig.BasicConfig.Alphabet
							.FirstOrDefault(l =>
								((c == EmptyLetterValueSynonym) && l.IsEmptyValue)
								|| (l.Value == c)) == null)
						.Distinct()
						.ToArray();
					if ((notExistsLetters != null) && (notExistsLetters.Length > 0))
					{
						string notExistsLettersString = string.Join(", ", notExistsLetters);
						newInputMessageValue = string.Format("Во входной строке есть символы отсутствующие в алфавите: '{0}'.", notExistsLettersString);
					}
					if (InputString.Length > Cells.Count)
					{
						newInputMessageValue = "Входная строка слишком длинная.";
					}
				}
			}
			InputMessage = newInputMessageValue;
		}

		protected override bool CanExecuteStartWritingCommand()
		{
			bool canExecute = base.CanExecuteStartWritingCommand()
				&& string.IsNullOrEmpty(InputMessage);
			return canExecute;
		}

		protected override void OnBeforeWriting()
		{
			if ((CurrentConfig != null) && !string.IsNullOrEmpty(InputString))
			{
				for (int i = 0; i < InputString.Length; i++)
				{
					string c = InputString[i].ToString();
					Cells[i].NewValue = CurrentConfig.BasicConfig.Alphabet
						.FirstOrDefault(l =>
							((c == EmptyLetterValueSynonym) && l.IsEmptyValue)
							|| (l.Value == c));
				}
			}
		}
	}
}

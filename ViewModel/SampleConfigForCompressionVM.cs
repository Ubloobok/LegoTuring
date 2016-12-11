using LegoTuringMachine.Device;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Model;
using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.ViewModel
{
	public class SampleConfigForCompressionVM : BaseTuringMachineVM
	{
		public const string EmptyLetterValueSynonym = " ";

		private string _inputString;
		private string _inputMessage;

		public SampleConfigForCompressionVM(IControlDevice controlDevice, ISettingsFacade settingsFacade, SharedValue<int?> sharedActiveCellIndex)
			: base(controlDevice, settingsFacade, sharedActiveCellIndex)
		{
			var commonSettings = SettingsFacade.LoadCommonSettings();
			for (int i = 0; i < commonSettings.BlocksCount; i++)
			{
				Cells.Add(new CellItemVM()
				{
					IsActive = false,
					ValueState = CellValueState.Unknown,
				});
			}
			CurrentConfig = SettingsFacade.LoadSampleConfig("ScCompression");
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

		private void OnInputStringChanged()
		{
			string newInputMessageValue = null;
			if (!string.IsNullOrEmpty(InputString))
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
					string notExistsLettersString = string.Join(",", notExistsLetters);
					newInputMessageValue = string.Format("Во входной строке есть значения которых нет в Алфавите: '{0}'.", notExistsLettersString);
				}
				if (InputString.Length > Cells.Count)
				{
					newInputMessageValue = "Входная строка слишком длинная.";
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

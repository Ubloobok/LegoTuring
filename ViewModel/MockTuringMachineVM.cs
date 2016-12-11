using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using LegoTuringMachine.ViewModel.Item;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.Device;
using LegoTuringMachine.Model;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Extensions;
using System.Threading.Tasks;

namespace LegoTuringMachine.ViewModel
{
    public class MockTuringMachineVM : ViewModelBase, ITuringMachineVM
    {
        public const string EmptyLetterValueSynonym = " ";

        private ISettingsFacade _settingsFacade;
        private IControlDevice _controlDevice;
        private ComplexConfigItemVM _currentConfig;
        private StateItemVM _currentState;
        private RuleItemVM _currentRule;
        private string _inputString;
        private string _inputMessage;

        public MockTuringMachineVM(IControlDevice controlDevice, ISettingsFacade settingsFacade)
        {
            _settingsFacade = settingsFacade;
            _controlDevice = controlDevice;
            _controlDevice.WriteCompleted +=_controlDevice_WriteCompleted;
            _controlDevice.MoveCompleted += _controlDevice_MoveCompleted;
            WriteInputStringCommand = new ActionCommand(ExecuteWriteInputStringCommand, CanExecuteWriteInputStringCommand);
            StartCommand = new ActionCommand(ExecuteStartCommand, CanExecuteStartCommand);
            DoPauseCommand = new ActionCommand(ExecutePauseCommand, CanExecutePauseCommand);
            DoStopCommand = new ActionCommand(ExecuteStopCommand, CanExecuteStopCommand);

            Cells = new ObservableCollection<CellItemVM>();
            Configs = new ObservableCollection<ComplexConfigItemVM>();
            SetCurrentConfigCommand = new DelegateCommand<ComplexConfigItemVM>(c =>
            {
                CurrentConfig = c;
                Cells.Clear();
                var commonConfig = _settingsFacade.LoadCommonSettings();
                for (int i = 0; i < commonConfig.BlocksCount; i++)
                {
                    Cells.Add(new CellItemVM()
                    {
                        IsActive = false,
                        CurrentValue = CurrentConfig.BasicConfig.Alphabet.FirstOrDefault(a => a.IsEmptyValue),
                        ValueState = CellValueState.Unused,
                        AttachedBasicConfig = CurrentConfig.BasicConfig,
                    });
                }
                InputString = InputString;
            });
            LoadPreSetConfigs();
            ActiveCellIndex = 0;
        }

        /// <summary>
        /// Gets or sets current machine state.
        /// </summary>
        public StateItemVM CurrentState
        {
            get { return _currentState; }
            private set
            {
                if (value != _currentState)
                {
                    RaisePropertyChanging<StateItemVM>(() => CurrentState);
                    _currentState = value;
                    RaisePropertyChanged<StateItemVM>(() => CurrentState);
                }
            }
        }

        /// <summary>
        /// Gets or sets current processing rule.
        /// </summary>
        public RuleItemVM CurrentRule
        {
            get { return _currentRule; }
            private set
            {
                if (value != _currentRule)
                {
                    RaisePropertyChanging<RuleItemVM>(() => CurrentRule);
                    _currentRule = value;
                    RaisePropertyChanged<RuleItemVM>(() => CurrentRule);
                }
            }
        }

        /// <summary>
        /// Gets collection of the current cells.
        /// </summary>
        public ObservableCollection<CellItemVM> Cells { get; private set; }

        /// <summary>
        /// Gets collection of all available configurations.
        /// </summary>
        public ObservableCollection<ComplexConfigItemVM> Configs { get; private set; }

        /// <summary>
        /// Gets current configuration.
        /// </summary>
        public ComplexConfigItemVM CurrentConfig
        {
            get { return _currentConfig; }
            set
            {
                if (value != _currentConfig)
                {
                    RaisePropertyChanging<ComplexConfigItemVM>(() => CurrentConfig);
                    _currentConfig = value;
                    RaisePropertyChanged<ComplexConfigItemVM>(() => CurrentConfig);
                }
            }
        }

        /// <summary>
        /// Gets command for set current configuration.
        /// </summary>
        public DelegateCommand<ComplexConfigItemVM> SetCurrentConfigCommand { get; set; }

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
                if (CurrentConfig == null)
                {
                    newInputMessageValue = "Выберите конфигурацию";
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

        public ActionCommand WriteInputStringCommand { get; private set; }
        public ActionCommand StartCommand { get; private set; }
        public ActionCommand DoPauseCommand { get; private set; }
        public ActionCommand DoStopCommand { get; private set; }

        public ActionCommand WriteInputValueCommand { get; set; }

        private int _activeCellIndex;
        private int ActiveCellIndex
        {
            get { return _activeCellIndex; }
            set
            {
                if (_activeCellIndex < Cells.Count)
                {
                    Cells[_activeCellIndex].IsActive = false;
                }
                if (value < Cells.Count)
                {
                    Cells[value].IsActive = true;
                }
                _activeCellIndex = value;
            }
        }
        private int NewCellIndex { get; set; }

        private void LoadPreSetConfigs()
        {
            var preSets = _settingsFacade.LoadPreSetConfigs();
            Configs.AddRange(preSets);

			//CurrentAction = "Ожидание";
			//CurrentRule = null;
			//InputMessage = null;
        }

        void _controlDevice_WriteCompleted(object sender, WriteCompletedEventArgs e)
        {
            var activeCell = Cells[ActiveCellIndex];
            activeCell.CurrentValue = activeCell.NewValue;
            activeCell.NewValue = null;
            activeCell.ValueState = CellValueState.Known;
            NewCellIndex = ActiveCellIndex + 1;
            _controlDevice.MoveAsync(Direction.Right, 1);
        }

        void _controlDevice_MoveCompleted(object sender, MoveCompletedEventArgs e)
        {
            ActiveCellIndex = NewCellIndex;
            WriteActiveCellValue();
        }

        private void ExecuteWriteInputStringCommand()
        {
            CurrentState = null;
            CurrentRule = null;
            for (int i = 0; i < InputString.Length; i++)
            {
                string c = InputString[i].ToString();
                Cells[i].NewValue = CurrentConfig.BasicConfig.Alphabet
                    .FirstOrDefault(l =>
                        ((c == EmptyLetterValueSynonym) && l.IsEmptyValue)
                        || (l.Value == c));
            }
            WriteActiveCellValue();

			////// 2. Записать данные
			// Заполняем везде NewValue, потом команда их переносит на ленту
			// 

			////// 3. Выполнить алгоритм
			// 
        }

        private void WriteActiveCellValue()
        {
            var inputCell = Cells[ActiveCellIndex];
            if (inputCell.NewValue != null)
            {
                double blockStep = 1 / ((double)CurrentConfig.BasicConfig.Alphabet.Count - 1);
                var inputValueIndex = CurrentConfig.BasicConfig.Alphabet.IndexOf(inputCell.NewValue);
                double inputValue = inputValueIndex * blockStep;
				//_controlDevice.WriteAsync(inputValue);
            }
        }

        private bool CanExecuteWriteInputStringCommand()
        {
            return true;
        }

        private void ExecuteStartCommand()
        {
            var currentCell = Cells[ActiveCellIndex];
            if (currentCell.CurrentValue != null)
            {
                if (CurrentState == null)
                {
                    CurrentState = CurrentConfig.BasicConfig.States
                        .FirstOrDefault(s => s.IsInitialState);
                }
                CurrentRule = CurrentConfig.RulesConfig.Rules
                    .FirstOrDefault(r => r.ConditionLetter.Value == currentCell.CurrentValue.Value && r.ConditionState.Name == CurrentState.Name);
                if (CurrentRule != null)
                {
                    currentCell.CurrentValue = CurrentRule.NewLetter;
                    //currentCell.NewValue = CurrentRule.NewLetter;
                    CurrentState = CurrentRule.NewState;

                    if (CurrentRule.NewPosition == Direction.Left)
                    {
                        ActiveCellIndex = Math.Max(0, ActiveCellIndex - 1);
                    }
                    else if (CurrentRule.NewPosition == Direction.Right)
                    {
                        ActiveCellIndex = ActiveCellIndex + 1;
                    }
                    var newCurrentCell = Cells[ActiveCellIndex];
                    //CurrentState = CurrentConfig.BasicConfig.States
                    //    .FirstOrDefault(s => s.IsInitialState);
                    CurrentRule = CurrentConfig.RulesConfig.Rules
                        .FirstOrDefault(r => r.ConditionLetter.Value == newCurrentCell.CurrentValue.Value && r.ConditionState.Name == CurrentState.Name);
                }
            }
        }

        private bool CanExecuteStartCommand()
        {
            return true;
        }

        private void ExecutePauseCommand()
        {
        }

        private bool CanExecutePauseCommand()
        {
            return true;
        }

        private void ExecuteStopCommand()
        {
            ActiveCellIndex = 0;
            CurrentState = null;
            CurrentRule = null;
            var newCurrentCell = Cells[ActiveCellIndex];
            CurrentState = CurrentConfig.BasicConfig.States
                .FirstOrDefault(s => s.IsInitialState);
            CurrentRule = CurrentConfig.RulesConfig.Rules
                .FirstOrDefault(r => r.ConditionLetter.Value == newCurrentCell.CurrentValue.Value && r.ConditionState.Name == CurrentState.Name);
        }

        private bool CanExecuteStopCommand()
        {
            return true;
        }


		public ActionCommand DoStartProcessingCommand
		{
			get { throw new NotImplementedException(); }
		}

		public ActionCommand DoStartWritingCommand
		{
			get { throw new NotImplementedException(); }
		}


		public TuringMachineAction CurrentAction
		{
			get { throw new NotImplementedException(); }
		}
	}
}

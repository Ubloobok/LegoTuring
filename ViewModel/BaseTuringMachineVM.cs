using LegoTuringMachine.Device;
using LegoTuringMachine.Extensions;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Model;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LegoTuringMachine.ViewModel
{
	public class BaseTuringMachineVM : ViewModelBase, ITuringMachineVM
	{
		protected const int DefaultActiveCellIndex = 0;

		private ComplexConfigItemVM _currentConfig;
		private StateItemVM _currentState;
		private TuringMachineAction _currentAction;
		private TuringMachineAction _requestedAction;
		private RuleItemVM _currentRule;
		private int? _activeCellIndex;
		private bool _isHelpModeEnabled;
		private SharedValue<int?> _sharedActiveCellIndex;

		public BaseTuringMachineVM(IControlDevice controlDevice, ISettingsFacade settingsFacade)
			: this(controlDevice, settingsFacade, null)
		{
		}

		public BaseTuringMachineVM(IControlDevice controlDevice, ISettingsFacade settingsFacade, SharedValue<int?> sharedActiveCellIndex)
		{
			SettingsFacade = settingsFacade;
			ControlDevice = controlDevice;
			ExecutionThreadToken = new ThreadToken();

			DoStartWritingCommand = new ActionCommand(ExecuteStartWritingCommand, CanExecuteStartWritingCommand);
			DoStartProcessingCommand = new ActionCommand(ExecuteStartProcessingCommand, CanExecuteStartProcessingCommand);
			DoPauseCommand = new ActionCommand(ExecutePauseCommand, CanExecutePauseCommand);
			DoStopCommand = new ActionCommand(ExecuteStopCommand, CanExecuteStopCommand);
			DoResetCommand = new ActionCommand(ExecuteResetCommand, CanExecuteResetCommand);

			Cells = new ObservableCollection<CellItemVM>();
			_sharedActiveCellIndex = sharedActiveCellIndex;
			if (_sharedActiveCellIndex == null)
			{
				ActiveCellIndex = null;
			}
			else
			{
				ActiveCellIndex = _sharedActiveCellIndex.Value;
				_sharedActiveCellIndex.ValueChanged += (o, e) => ActiveCellIndex = _sharedActiveCellIndex.Value;
			}
		}

		#region Properties

		/// <summary>
		/// Gets writing/reading thread token for waiting and cancellation.
		/// </summary>
		protected ThreadToken ExecutionThreadToken { get; private set; }

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
		/// Gets or sets current machine action.
		/// </summary>
		public TuringMachineAction CurrentAction
		{
			get { return _currentAction; }
			private set
			{
				_currentAction = value;
				RaisePropertyChanged(() => CurrentAction);
				RaisePropertyChanged(() => IsBusy);
				RaisePropertyChanged(() => IsWriting);
				RaisePropertyChanged(() => IsProcessing);
			}
		}

		/// <summary>
		/// Gets flags which indicates that not machine is busy.
		/// </summary>
		public bool IsBusy
		{
			get { return CurrentAction != TuringMachineAction.None; }
		}

		/// <summary>
		/// Gets flag which indicates that current action is writing.
		/// </summary>
		public bool IsWriting
		{
			get { return CurrentAction == TuringMachineAction.Writing; }
		}

		/// <summary>
		/// Gets flag which indicates that current action is processing.
		/// </summary>
		public bool IsProcessing
		{
			get { return CurrentAction == TuringMachineAction.Processing; }
		}

		/// <summary>
		/// Gets or sets request machine action.
		/// </summary>
		public TuringMachineAction RequestedAction
		{
			get { return _requestedAction; }
			protected set
			{
				if (value != _requestedAction)
				{
					RaisePropertyChanging(() => RequestedAction);
					_requestedAction = value;
					RaisePropertyChanged(() => RequestedAction);
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
		/// Gets or sets current configuration.
		/// </summary>
		public ComplexConfigItemVM CurrentConfig
		{
			get { return _currentConfig; }
			set
			{
				if (value != _currentConfig)
				{
					var oldValue = _currentConfig;
					RaisePropertyChanging<ComplexConfigItemVM>(() => CurrentConfig);
					_currentConfig = value;
					RaisePropertyChanged<ComplexConfigItemVM>(() => CurrentConfig);
					OnCurrentConfigChanged(oldValue, value);
				}
			}
		}

		/// <summary>
		/// Gets command for starting or continuation the processing.
		/// </summary>
		public ActionCommand DoStartProcessingCommand { get; set; }

		/// <summary>
		/// Gets command for starting or continuation the writing.
		/// </summary>
		public ActionCommand DoStartWritingCommand { get; set; }

		/// <summary>
		/// Gets command for common pause the work.
		/// </summary>
		public ActionCommand DoPauseCommand { get; private set; }

		/// <summary>
		/// Gets command for common stop the work.
		/// </summary>
		public ActionCommand DoStopCommand { get; private set; }

		/// <summary>
		/// Gets command for clearing the cells and reset states.
		/// </summary>
		public ActionCommand DoResetCommand { get; private set; }

		/// <summary>
		/// Gets current settings facade.
		/// </summary>
		public ISettingsFacade SettingsFacade { get; private set; }

		/// <summary>
		/// Gets current control device.
		/// </summary>
		public IControlDevice ControlDevice { get; private set; }

		/// <summary>
		/// Gets current active cell index, or null, if value has not specified.
		/// </summary>
		public int? ActiveCellIndex
		{
			get { return _activeCellIndex; }
			private set
			{
				if (_activeCellIndex != value)
				{
					int? newActiveCellIndex = value.HasValue ? Math.Min(Math.Max(value.Value, 0), Cells.Count) : (int?)null;
					if (_activeCellIndex.HasValue)
					{
						Cells[_activeCellIndex.Value].IsActive = false;
					}
					if (newActiveCellIndex.HasValue)
					{
						Cells[newActiveCellIndex.Value].IsActive = true;
					}
					_activeCellIndex = newActiveCellIndex;
					if (_sharedActiveCellIndex != null)
					{
						_sharedActiveCellIndex.Value = newActiveCellIndex;
					}
					RaisePropertyChanged(() => ActiveCellIndex);
					RaisePropertyChanged(() => ActiveCell);
				}
			}
		}

		/// <summary>
		/// Gets current active cell, or null, if value has not specified.
		/// </summary>
		public CellItemVM ActiveCell
		{
			get
			{
				var cell = ((Cells.Count == 0) || !ActiveCellIndex.HasValue) ? null : Cells[ActiveCellIndex.Value];
				return cell;
			}
		}

		/// <summary>
		/// Gets or sets flag which indicates that help mode is enabled.
		/// </summary>
		public bool IsHelpModeEnabled
		{
			get { return _isHelpModeEnabled; }
			set
			{
				if (value != _isHelpModeEnabled)
				{
					_isHelpModeEnabled = value;
					RaisePropertyChanged(() => IsHelpModeEnabled);
				}
			}
		}

		#endregion // Properties

		#region Commands

		/// <summary>
		/// Gets value which indicates that writing command can be executed.
		/// </summary>
		protected virtual bool CanExecuteStartWritingCommand()
		{
			bool canExecute =
				(CurrentAction == TuringMachineAction.None && RequestedAction == TuringMachineAction.None)
				|| (CurrentAction == TuringMachineAction.Pausing && RequestedAction == TuringMachineAction.Writing);
			return canExecute;
		}

		/// <summary>
		/// Executes writing command.
		/// </summary>
		private void ExecuteStartWritingCommand()
		{
			if (CurrentAction == TuringMachineAction.Pausing)
			{
				CurrentAction = TuringMachineAction.Writing;
				RequestedAction = TuringMachineAction.None;
				ExecutionThreadToken.Continue();
			}
			else
			{
				ThreadPool.QueueUserWorkItem(o =>
				{
					try
					{
						if (ControlDevice.ConnectionState == DeviceConnectionState.Disconnected)
						{
							var settings = SettingsFacade.LoadDeviceSettings();
							ControlDevice.ApplySettings(settings);
							ControlDevice.Connect();
						}
						RequestedAction = TuringMachineAction.Writing;
						if (!ActiveCellIndex.HasValue || (ActiveCellIndex != DefaultActiveCellIndex))
						{
							Move(DefaultActiveCellIndex);
						}
						OnBeforeWriting();
						CurrentAction = TuringMachineAction.Writing;
						RequestedAction = TuringMachineAction.None;
						WriteCellsNewValue();
					}
					catch (OperationCanceledException ocEx)
					{
						ExecutionThreadToken.Reset();
					}
					catch (Exception e)
					{
						ExecutionThreadToken.Reset();
						MessageBox.Show(e.Message, "Печалька!");
					}
					finally
					{
						if (ControlDevice.ConnectionState == DeviceConnectionState.Connected)
						{
							Move(DefaultActiveCellIndex);
							ControlDevice.Disconnect();
						}
						CurrentAction = TuringMachineAction.None;
						RequestedAction = TuringMachineAction.None;
					}
				});
			}
		}

		/// <summary>
		/// Gets value which indicates that processing command can be executed.
		/// </summary>
		private bool CanExecuteStartProcessingCommand()
		{
			bool canExecute =
				(CurrentAction == TuringMachineAction.None && RequestedAction == TuringMachineAction.None)
				|| (CurrentAction == TuringMachineAction.Pausing && RequestedAction == TuringMachineAction.Processing);
			return canExecute;
		}

		/// <summary>
		/// Executes processing command.
		/// </summary>
		private void ExecuteStartProcessingCommand()
		{
			if (CurrentAction == TuringMachineAction.Pausing)
			{
				CurrentAction = TuringMachineAction.Processing;
				RequestedAction = TuringMachineAction.None;
				ExecutionThreadToken.Continue();
			}
			else
			{
				ThreadPool.QueueUserWorkItem(o =>
				{
					try
					{
						if (ControlDevice.ConnectionState == DeviceConnectionState.Disconnected)
						{
							var settings = SettingsFacade.LoadDeviceSettings();
							ControlDevice.ApplySettings(settings);
							ControlDevice.Connect();
						}
						RequestedAction = TuringMachineAction.Processing;
						if (!ActiveCellIndex.HasValue || (ActiveCellIndex != DefaultActiveCellIndex))
						{
							Move(DefaultActiveCellIndex);
						}
						CurrentState = null;
						CurrentRule = null;
						CurrentAction = TuringMachineAction.Processing;
						RequestedAction = TuringMachineAction.None;
						Process();
					}
					catch (OperationCanceledException ocEx)
					{
						ExecutionThreadToken.Reset();
					}
					catch (Exception e)
					{
						ExecutionThreadToken.Reset();
						MessageBox.Show(e.Message, "Печалька!");
					}
					finally
					{
						if (ControlDevice.ConnectionState == DeviceConnectionState.Connected)
						{
							Move(DefaultActiveCellIndex);
							ControlDevice.Disconnect();
						}
						CurrentAction = TuringMachineAction.None;
						RequestedAction = TuringMachineAction.None;
					}
				});
			}
		}

		/// <summary>
		/// Gets value which indicates that pause command can be executed.
		/// </summary>
		/// <returns></returns>
		private bool CanExecutePauseCommand()
		{
			bool canExecute =
				(CurrentAction == TuringMachineAction.Writing || CurrentAction == TuringMachineAction.Processing)
				&& (RequestedAction == TuringMachineAction.None);
			return canExecute;
		}

		/// <summary>
		/// Executes pause command.
		/// </summary>
		private void ExecutePauseCommand()
		{
			if (RequestedAction != TuringMachineAction.Pausing)
			{
				RequestedAction = TuringMachineAction.Pausing;
				ExecutionThreadToken.Pause();
			}
		}

		/// <summary>
		/// Gets value which indicates that stop command can be executed.
		/// </summary>
		private bool CanExecuteStopCommand()
		{
			bool canExecute =
				(CurrentAction == TuringMachineAction.Writing || CurrentAction == TuringMachineAction.Processing)
				&& (RequestedAction == TuringMachineAction.None);
			return canExecute;
		}

		/// <summary>
		/// Executes stop command;
		/// </summary>
		private void ExecuteStopCommand()
		{
			RequestedAction = TuringMachineAction.Stopping;
			ExecutionThreadToken.Cancel();
			ControlDevice.StopMoveAsync();
			ControlDevice.StopWriteAsync();
		}

		/// <summary>
		/// Gets value which indicates that reset command can be executed.
		/// </summary>
		private bool CanExecuteResetCommand()
		{
			bool canExecute = CurrentAction == TuringMachineAction.None;
			return canExecute;
		}

		/// <summary>
		/// Executes reset command;
		/// </summary>
		private void ExecuteResetCommand()
		{
			try
			{
				CurrentState = null;
				CurrentRule = null;
				var emptyLetter = CurrentConfig.BasicConfig.Alphabet.FirstOrDefault(l => l.IsEmptyValue);
				foreach (var cell in Cells)
				{
					cell.CurrentValue = emptyLetter;
					cell.NewValue = null;
				}
				RequestedAction = TuringMachineAction.None;
				if (!ActiveCellIndex.HasValue || (ActiveCellIndex != DefaultActiveCellIndex))
				{
					if (ControlDevice.ConnectionState != DeviceConnectionState.Connected)
					{
						var settings = SettingsFacade.LoadDeviceSettings();
						ControlDevice.ApplySettings(settings);
						ControlDevice.Connect();
					}
					Move(DefaultActiveCellIndex);
				}
				if (ControlDevice.ConnectionState == DeviceConnectionState.Connected)
				{
					ControlDevice.Disconnect();
				}
			}
			catch
			{
			}
		}

		#endregion // Commands

		/// <summary>
		/// Method called when current config has been changed.
		/// </summary>
		/// <param name="oldValue">Old value.</param>
		/// <param name="newValue">New value.</param>
		protected virtual void OnCurrentConfigChanged(ComplexConfigItemVM oldValue, ComplexConfigItemVM newValue)
		{
			var commonSettings = SettingsFacade.LoadCommonSettings();
			Cells.Clear();
			for (int i = 0; i < commonSettings.BlocksCount; i++)
			{
				Cells.Add(new CellItemVM()
				{
					IsActive = false,
					CurrentValue = CurrentConfig.BasicConfig.Alphabet.FirstOrDefault(a => a.IsEmptyValue),
					ValueState = CellValueState.Unused,
					AttachedBasicConfig = CurrentConfig.BasicConfig,
				});
			}
		}

		/// <summary>
		/// Writes new values of all cells in machine, beggining from the active cell index or the default position.
		/// </summary>
		private void WriteCellsNewValue()
		{
			if (!ActiveCellIndex.HasValue)
			{
				Move(DefaultActiveCellIndex);
			}
			for (int inputCellIndex = ActiveCellIndex.Value; inputCellIndex < Cells.Count; inputCellIndex++)
			{
				WaitAndThrowIfRequested();
				var inputCell = Cells[inputCellIndex];
				if (inputCell.NewValue != null)
				{
					WaitAndThrowIfRequested();
					if (ActiveCellIndex != inputCellIndex)
					{
						Move(inputCellIndex);
					}
					WaitAndThrowIfRequested();
					WriteActiveCellNewValue();
				}
			}
		}

		/// <summary>
		/// Writes active cell new value.
		/// </summary>
		private void WriteActiveCellNewValue()
		{
			var ac = ActiveCell;

			ReadCompletedEventArgs readArgs = null;
			bool isReaded = false;
			do
			{
				readArgs = ControlDevice.ReadIntervalAsync(CurrentConfig.BasicConfig.Alphabet.Count).Await();
				if (readArgs.Interval.HasValue)
				{
					isReaded = true;
				}
				else
				{
					var result = MessageBox.Show("Не обнаружен кубик в ячейке. Выставите, пожалуйста, вручную?", "Печалька!", MessageBoxButton.YesNo);
					if (result == MessageBoxResult.No)
					{
						throw new OperationCanceledException();
					}
					isReaded = false;
				}
			} while (!isReaded);
			int? acCurrentValueIndex =
				readArgs.Interval.HasValue ?
				readArgs.Interval.Value - 1 :
				(int?)null;
			var acCurrentValue =
				acCurrentValueIndex.HasValue ?
				CurrentConfig.BasicConfig.Alphabet.Reverse().ElementAt(acCurrentValueIndex.Value) :
				CurrentConfig.BasicConfig.EmptyAlphabetLetter;
			ac.CurrentValue = acCurrentValue;

			if (ac.CurrentValue == ac.NewValue)
			{
				ac.NewValue = null;
				ac.ValueState = CellValueState.Known;
			}
			else
			{
				int inputValueIndex = CurrentConfig.BasicConfig.Alphabet.IndexOf(ac.NewValue);
				int newInterval = (CurrentConfig.BasicConfig.Alphabet.Count - inputValueIndex);

				int oldValueIndex = CurrentConfig.BasicConfig.Alphabet.IndexOf(ac.CurrentValue);
				int oldInterval = (CurrentConfig.BasicConfig.Alphabet.Count - oldValueIndex);

				WaitAndThrowIfRequested();
				var wTask = ControlDevice.WriteIntervalAsync(newInterval, oldInterval, CurrentConfig.BasicConfig.Alphabet.Count);
				var wTaskResult = wTask.Await();
				WaitAndThrowIfRequested();

				ac.OldValue = ac.CurrentValue;
				ac.CurrentValue = ac.NewValue;
				ac.NewValue = null;
				ac.ValueState = CellValueState.Known;
			}
		}

		/// <summary>
		/// Calls before all writing actions.
		/// </summary>
		protected virtual void OnBeforeWriting()
		{
		}

		/// <summary>
		/// Sleep current thread, if pausing is requested, or break current thread, if stopping is requested.
		/// </summary>
		protected void WaitAndThrowIfRequested()
		{
			if (ExecutionThreadToken.IsWaitingRequested)
			{
				var lastAction = CurrentAction;
				CurrentAction = TuringMachineAction.Pausing;
				RequestedAction = lastAction;
				ExecutionThreadToken.WaitIfWaitingRequested();
				CurrentAction = lastAction;
				RequestedAction = TuringMachineAction.None;
			}
			if (ExecutionThreadToken.IsCancellationRequested)
			{
				var lastAction = CurrentAction;
				CurrentAction = TuringMachineAction.Stopping;
				RequestedAction = lastAction;
				ExecutionThreadToken.ThrowIfCancellationRequested();
				CurrentAction = lastAction;
				RequestedAction = TuringMachineAction.None;
			}
		}

		/// <summary>
		/// Moves to the concrete cell index.
		/// </summary>
		private void Move(int cellIndex)
		{
			if (ActiveCellIndex != cellIndex)
			{
				Task<MoveCompletedEventArgs> mTask = null;
				Direction direction = ActiveCellIndex < cellIndex ? Direction.Right : Direction.Left;
				int distance = Math.Abs(ActiveCellIndex.GetValueOrDefault() - cellIndex);
				if (distance != 0)
				{
					mTask = ControlDevice.MoveAsync(direction, distance);
					var mTaskResult = mTask.Await();
				}
			}
			ActiveCellIndex = cellIndex;
		}

		/// <summary>
		/// Process all cells in machine from the active cell index, or from the default cell index.
		/// Processing will stop when machine would go the final state, or at the empty cell.
		/// </summary>
		private void Process()
		{
			while (true)
			{
				WaitAndThrowIfRequested();
				var activeCell = ActiveCell;
				if (activeCell == null)
				{
					Move(DefaultActiveCellIndex);
					activeCell = ActiveCell;
				}
				if (activeCell.CurrentValue == null)
				{
					break;
				}
				else
				{
					WaitAndThrowIfRequested();
					if (CurrentState == null)
					{
						CurrentState = CurrentConfig.BasicConfig.States
							.FirstOrDefault(s => s.IsInitialState);
						if (CurrentState == null)
						{
							break;
						}
					}
					CurrentRule = CurrentConfig.RulesConfig.Rules
						.FirstOrDefault(r => r.ConditionLetter.Value == activeCell.CurrentValue.Value && r.ConditionState.Name == CurrentState.Name);
					if (CurrentRule == null)
					{
						CurrentState = CurrentConfig.BasicConfig.States
							.FirstOrDefault(s => s.IsFinalState);
						break;
					}
					else
					{
						WaitAndThrowIfRequested();
						activeCell.NewValue = CurrentRule.NewLetter;
						WriteActiveCellNewValue();
						CurrentState = CurrentRule.NewState;

						WaitAndThrowIfRequested();
						int activeCellIndex = ActiveCellIndex.Value;
						int newActiveCellIndex = activeCellIndex;
						if (CurrentRule.NewPosition == Direction.Left)
						{
							newActiveCellIndex = Math.Max(0, activeCellIndex - 1);
						}
						else if (CurrentRule.NewPosition == Direction.Right)
						{
							newActiveCellIndex = Math.Min(Cells.Count, activeCellIndex + 1);
						}

						WaitAndThrowIfRequested();
						if ((newActiveCellIndex == ActiveCellIndex)
							|| CurrentRule.NewState.IsFinalState)
						{
							break;
						}
						else
						{
							Move(newActiveCellIndex);
						}
					}
				}
			}
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			DoStartWritingCommand.RaiseCanExecuteChanged();
			DoStartProcessingCommand.RaiseCanExecuteChanged();
			DoPauseCommand.RaiseCanExecuteChanged();
			DoStopCommand.RaiseCanExecuteChanged();
			DoResetCommand.RaiseCanExecuteChanged();
		}
	}
}

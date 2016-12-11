using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.ViewModel.Item;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LegoTuringMachine.ViewModel
{
	public interface ITuringMachineVM : INotifyPropertyChanging, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets current machine state.
		/// </summary>
		StateItemVM CurrentState { get; }

		/// <summary>
		/// Gets current machine action.
		/// </summary>
		TuringMachineAction CurrentAction { get; }

		/// <summary>
		/// Gets current processing rule.
		/// </summary>
		RuleItemVM CurrentRule { get; }

		/// <summary>
		/// Gets collection of the current cells.
		/// </summary>
		ObservableCollection<CellItemVM> Cells { get; }

		/// <summary>
		/// Gets current configuration.
		/// </summary>
		ComplexConfigItemVM CurrentConfig { get; set; }

		/// <summary>
		/// Gets command for starting or continuation the processing.
		/// </summary>
		ActionCommand DoStartProcessingCommand { get; }

		/// <summary>
		/// Gets command for starting or continuation the writing.
		/// </summary>
		ActionCommand DoStartWritingCommand { get; }

		/// <summary>
		/// Gets command for common pause the work.
		/// </summary>
		ActionCommand DoPauseCommand { get; }

		/// <summary>
		/// Gets command for common stop the work.
		/// </summary>
		ActionCommand DoStopCommand { get; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;

namespace LegoTuringMachine.ViewModel.Core
{
	public class ActionCommand : ICommand
	{
		readonly Action _execute;
		readonly Func<bool> _canExecute;

		public ActionCommand(Action execute)
			: this(execute, null)
		{
		}

		public ActionCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		#region ICommand

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute();
		}

		public event EventHandler CanExecuteChanged;

		[DebuggerStepThrough]
		public void Execute(object parameter)
		{
			_execute();
		}

		#endregion // ICommand

		[DebuggerStepThrough]
		public void RaiseCanExecuteChanged()
		{
			if (App.GlobalDispatcher.CheckAccess())
			{
				if (CanExecuteChanged != null)
				{
					CanExecuteChanged(this, new EventArgs());
				}
			}
			else
			{
				App.GlobalDispatcher.BeginInvoke(new Action(RaiseCanExecuteChanged));
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;

namespace LegoTuringMachine.ViewModel.Core
{
	public class DelegateCommand<T> : ICommand
	{
		readonly Action<T> _execute;
		readonly Predicate<T> _canExecute;

		public DelegateCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
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
			return _canExecute == null ? true : _canExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged;

        [DebuggerStepThrough]
		public void Execute(object parameter)
		{
			_execute((T)parameter);
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

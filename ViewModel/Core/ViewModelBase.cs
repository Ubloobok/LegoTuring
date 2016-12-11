using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using LegoTuringMachine.Utilities;
using System.Windows.Threading;

namespace LegoTuringMachine.ViewModel.Core
{
	public abstract class ViewModelBase : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private Dispatcher _dispatcher;

		protected ViewModelBase()
			: this(App.GlobalDispatcher)
		{
		}

		protected ViewModelBase(Dispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		protected void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
		{
			if (_dispatcher.CheckAccess())
			{
				string propertyName = PropertyHelper.ExtractPropertyName<T>(propertyExpression);
				if (PropertyChanging != null)
				{
					PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
				}
			}
			else
			{
				_dispatcher.Invoke(new Action<Expression<Func<T>>>(RaisePropertyChanging<T>), propertyExpression);
			}
		}

		protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			if (_dispatcher.CheckAccess())
			{
				string propertyName = PropertyHelper.ExtractPropertyName<T>(propertyExpression);
				var args = new PropertyChangedEventArgs(propertyName);
				if (PropertyChanged != null)
				{
					PropertyChanged(this, args);
				}
				OnPropertyChanged(args);
			}
			else
			{
				_dispatcher.Invoke(new Action<Expression<Func<T>>>(RaisePropertyChanged<T>), propertyExpression);
			}
		}

		public event PropertyChangingEventHandler PropertyChanging;

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Implements
{
	public class SharedValue<T>
	{
		private T _value;

		public T Value
		{
			get { return _value; }
			set
			{
				_value = value;
				var valueChanged = ValueChanged;
				if (valueChanged != null)
				{
					valueChanged(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ValueChanged;
	}
}

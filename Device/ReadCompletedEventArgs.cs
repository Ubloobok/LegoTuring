using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Device
{
	public class ReadCompletedEventArgs : EventArgs
	{
		public ReadCompletedEventArgs(int? interval, double? translatedValue)
		{
			Interval = interval;
			TranslatedValue = translatedValue;
		}

		/// <summary>
		/// Gets interval number.
		/// </summary>
		public int? Interval { get; private set; }

		/// <summary>
		/// Gets translated value in [0, 1] range.
		/// </summary>
		public double? TranslatedValue { get; private set; }
	}
}

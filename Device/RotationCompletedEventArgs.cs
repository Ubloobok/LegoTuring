using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Device
{
	public class RotationCompletedEventArgs : EventArgs
	{
		public bool IsFinalPosition { get; set; }
		public int InitialTachoCount { get; set; }
		public int TargetTachoCount { get; set; }
		public int FinalTachoCount { get; set; }
		public int ErrorTachoCount { get; set; }
	}
}

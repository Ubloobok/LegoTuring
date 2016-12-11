using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.ViewModel
{
	/// <summary>
	/// Turing machine action.
	/// </summary>
	public enum TuringMachineAction
	{
		/// <summary>
		/// None, nothing, waiting for user command.
		/// </summary>
		None,

		/// <summary>
		/// Writing cells new values.
		/// </summary>
		Writing,

		/// <summary>
		/// Processing rules of the current configuration.
		/// </summary>
		Processing,

		/// <summary>
		/// Pausing, requested by user or system.
		/// </summary>
		Pausing,

		/// <summary>
		/// Stopping, requested by user or system.
		/// </summary>
		Stopping,
	}
}

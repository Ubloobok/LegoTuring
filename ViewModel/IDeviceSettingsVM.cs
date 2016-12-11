using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Item;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Model;

namespace LegoTuringMachine.ViewModel
{
	public interface IDeviceSettingsVM
	{
		string DeviceComPort { get; set; }

		SelectableCollection<SelectorItemVM> EnginePortsSelector { get; }
		int EngineStepPower { get; set; }
		int EngineStepAngle { get; set; }

		SelectableCollection<SelectorItemVM> ManipulatorPortSelector { get; }
		int ManipulatorPower { get; set; }
		int ManipulatorMaxAngle { get; set; }

		SelectableCollection<SelectorItemVM> SonarPortSelector { get; }

		void InitializeSettings();
	}
}

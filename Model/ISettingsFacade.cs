using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Model
{
	public interface ISettingsFacade
	{
		/// <summary>
		/// Loads saved or default device settings.
		/// </summary>
		DeviceSettings LoadDeviceSettings();

		/// <summary>
		/// Saves device settings.
		/// </summary>
		void SaveDeviceSettings(DeviceSettings deviceSettings);

		/// <summary>
		/// Loads saved or default common settings.
		/// </summary>
		CommonSettings LoadCommonSettings();

		/// <summary>
		/// Saves common settings.
		/// </summary>
		void SaveCommonSettings(CommonSettings commonSettings);

		/// <summary>
		/// Loads pre-set (default) configurations.
		/// </summary>
		IEnumerable<ComplexConfigItemVM> LoadPreSetConfigs();

		/// <summary>
		/// Loads sample config.
		/// </summary>
		/// <param name="sampleConfigName">Sample config name.</param>
		ComplexConfigItemVM LoadSampleConfig(string sampleConfigName);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Contract
{
	public interface INavigationInteraction
	{
		void OpenSettingsAsync();
		event EventHandler SettingsOpened;

		void CloseSettingsAsync();
		event EventHandler SettingsClosed;

		void DisplayMessageAsync(string message);
		void DisplayMessageAsync(string caption, string message);
		void DisplayErrorAsync(string message, Exception error = null);
		void DisplayErrorAsync(string caption, string message, Exception error = null);
	}
}

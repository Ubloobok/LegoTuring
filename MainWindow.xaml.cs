using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LegoTuringMachine.ViewModel;
using LegoTuringMachine.Device;
using LegoTuringMachine.Model;
using Xceed.Wpf.Toolkit;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.View;
using LegoTuringMachine.UI.Controls;
using MahApps.Metro.Controls;
using LegoTuringMachine.Implements;

namespace LegoTuringMachine
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			SmmView = (SelectModeMenuView)Resources["SmmView"];
			SscmView = (SelectSampleConfigMenuView)Resources["SscmView"];
			TmView = (TuringMachineView)Resources["TmView"];
			ScAdditionView = (SampleConfigForAdditionView)Resources["ScAdditionView"];
			ScCompressionView = (SampleConfigForCompressionView)Resources["ScCompressionView"];
			ScInversionView = (SampleConfigForInversionView)Resources["ScInversionView"];

			MainContentRegion.Content = SmmView;

			SmmView.SelectSimpleModeCommand = new ActionCommand(ExecuteSelectSimpleModeCommand);
			SmmView.SelectExtendedModeCommand = new ActionCommand(ExecuteSelectExtendedModeCommand);
			SscmView.BackCommand = new ActionCommand(ExecuteSscmBackCommand);
			SscmView.SelectAdditionCommand = new ActionCommand(() => ExecuteSelectSampleConfig(ScAdditionView));
			SscmView.SelectCompressionCommand = new ActionCommand(() => ExecuteSelectSampleConfig(ScCompressionView));
			SscmView.SelectInversionCommand = new ActionCommand(() => ExecuteSelectSampleConfig(ScInversionView));

			InitializeViewModels();

			DsWindow.CloseCommand = new ActionCommand(() =>
			{
				DsWindow.IsOpen = false;
				BackVideoMediaElement.Play();
			});
		}

		private void InitializeViewModels()
		{
			// DEV: Concrete control device may be changed in developing progress.
			// Mock are using for developing and testing.
			var mockControlDevice = new MockControlDevice();
			//var minqSquallsControlDevice = new MindSquallsControlDevice();
			//var aForgeControlDevice = new AForgeControlDevice();

			IControlDevice currentControlDevice = mockControlDevice;
			//if (EnvironmentChecker.IsChecked == true)
			//{
			//	currentControlDevice = mockControlDevice;
			//}
			//else
			//{
			//	currentControlDevice = aForgeControlDevice;
			//}

			var settingsFacade = new SettingsFacade();
			var sharedActiveCellIndex = new SharedValue<int?>() { Value = null };
			var studentTmVM = new StudentTuringMachineVM(currentControlDevice, settingsFacade, sharedActiveCellIndex);
			var scAdditionVM = new SampleConfigForAdditionVM(currentControlDevice, settingsFacade, sharedActiveCellIndex);
			var scCompressionVM = new SampleConfigForCompressionVM(currentControlDevice, settingsFacade, sharedActiveCellIndex);
			var scInversionVM = new SampleConfigForInversionVM(currentControlDevice, settingsFacade, sharedActiveCellIndex);

			//TmView.DataContext = new MockTuringMachineVM(mockControlDevice, settingsFacade);
			TmView.DataContext = studentTmVM;
			ScAdditionView.DataContext = scAdditionVM;
			ScCompressionView.DataContext = scCompressionVM;
			ScInversionView.DataContext = scInversionVM;
			SscWindow.DataContext = studentTmVM;
			SscWindow.CloseCommand = new ActionCommand(() => studentTmVM.DisableSelectCurrentConfigModeCommand.Execute(null));

			var deviceSettingsVM = new DeviceSettingsVM(currentControlDevice, settingsFacade);
			deviceSettingsVM.InitializeSettings();

			DsView.DataContext = deviceSettingsVM;
		}

		public SelectModeMenuView SmmView { get; set; }
		public SelectSampleConfigMenuView SscmView { get; set; }
		public TuringMachineView TmView { get; set; }
		public SampleConfigForAdditionView ScAdditionView { get; set; }
		public SampleConfigForCompressionView ScCompressionView { get; set; }
		public SampleConfigForInversionView ScInversionView { get; set; }

		private void ExecuteSelectSimpleModeCommand()
		{
			MainContentRegion.Content = SscmView;
		}

		private void ExecuteSelectExtendedModeCommand()
		{
			BackVideoMediaElement.Stop();
			BackVideoMediaElement.Visibility = Visibility.Hidden;
			MainContentRegion.Content = TmView;
		}

		private void ExecuteSscmBackCommand()
		{
			BackVideoMediaElement.Play();
			BackVideoMediaElement.Visibility = Visibility.Visible;
			MainContentRegion.Content = SmmView;
		}

		private void ExecuteSelectSampleConfig(object sampleConfigView)
		{
			BackVideoMediaElement.Stop();
			BackVideoMediaElement.Visibility = Visibility.Hidden;
			MainContentRegion.Content = sampleConfigView;
		}

		private void MainMenuItemClick(object sender, RoutedEventArgs e)
		{
			BackVideoMediaElement.Play();
			BackVideoMediaElement.Visibility = Visibility.Visible;
			MainContentRegion.Content = SmmView;
		}

		private void SettingsMenuItemClick(object sender, RoutedEventArgs e)
		{
			BackVideoMediaElement.Pause();
			DsWindow.IsOpen = true;
		}

		private void OnBackVideoLoaded(object sender, RoutedEventArgs e)
		{
			BackVideoMediaElement.Play();
		}

		private void OnBackVideoMediaFailed(object sender, ExceptionRoutedEventArgs e)
		{

		}

		private void OnBackVideoMediaEnded(object sender, RoutedEventArgs e)
		{
			BackVideoMediaElement.Stop();
			BackVideoMediaElement.Play();
		}

		private void OnEnvironmentCheckerChecked(object sender, RoutedEventArgs e)
		{
			MainMenuItemClick(null, null);
			InitializeViewModels();
		}
	}
}

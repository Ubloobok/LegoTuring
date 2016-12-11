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

namespace LegoTuringMachine.View
{
	/// <summary>
	/// Логика взаимодействия для TuringMachineView.xaml
	/// </summary>
	public partial class TuringMachineView : UserControl
	{
		public TuringMachineView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			DataContextChanged += OnDataContextChanged;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext != null)
			{
				((StudentTuringMachineVM)DataContext).SetCurrentConfigCommand.Execute(((StudentTuringMachineVM)DataContext).Configs.FirstOrDefault());
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (DataContext != null)
			{
				((StudentTuringMachineVM)DataContext).SetCurrentConfigCommand.Execute(((StudentTuringMachineVM)DataContext).Configs.FirstOrDefault());
			}
		}
	}
}

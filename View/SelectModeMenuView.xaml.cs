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

namespace LegoTuringMachine.View
{
	/// <summary>
	/// Interaction logic for SelectModeMenuView.xaml
	/// </summary>
	public partial class SelectModeMenuView : UserControl
	{
		public SelectModeMenuView()
		{
			DataContext = this;
			InitializeComponent();
		}

		public ICommand SelectSimpleModeCommand
		{
			get { return (ICommand)GetValue(SelectSimpleModeCommandProperty); }
			set { SetValue(SelectSimpleModeCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectSimpleModeCommandProperty =
			DependencyProperty.Register("SelectSimpleModeCommand", typeof(ICommand), typeof(SelectModeMenuView));

		public ICommand SelectExtendedModeCommand
		{
			get { return (ICommand)GetValue(SelectExtendedModeCommandProperty); }
			set { SetValue(SelectExtendedModeCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectExtendedModeCommandProperty =
			DependencyProperty.Register("SelectExtendedModeCommand", typeof(ICommand), typeof(SelectModeMenuView));

		private void OnSimpleModeButtonClick(object sender, RoutedEventArgs e)
		{
		}

		private void OnExtendedModeButtonClick(object sender, RoutedEventArgs e)
		{
		}
	}
}

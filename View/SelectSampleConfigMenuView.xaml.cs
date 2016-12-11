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
	/// Interaction logic for SelectSimpleConfigMenuView.xaml
	/// </summary>
	public partial class SelectSampleConfigMenuView : UserControl
	{
		public SelectSampleConfigMenuView()
		{
			DataContext = this;
			InitializeComponent();
		}

		public ICommand BackCommand
		{
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}

		public static readonly DependencyProperty BackCommandProperty =
			DependencyProperty.Register("BackCommand", typeof(ICommand), typeof(SelectSampleConfigMenuView));

		public ICommand SelectAdditionCommand
		{
			get { return (ICommand)GetValue(SelectAdditionCommandProperty); }
			set { SetValue(SelectAdditionCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectAdditionCommandProperty =
			DependencyProperty.Register("SelectAdditionCommand", typeof(ICommand), typeof(SelectSampleConfigMenuView));

		public ICommand SelectCompressionCommand
		{
			get { return (ICommand)GetValue(SelectCompressionCommandProperty); }
			set { SetValue(SelectCompressionCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectCompressionCommandProperty =
			DependencyProperty.Register("SelectCompressionCommand", typeof(ICommand), typeof(SelectSampleConfigMenuView));

		public ICommand SelectInversionCommand
		{
			get { return (ICommand)GetValue(SelectInversionCommandProperty); }
			set { SetValue(SelectInversionCommandProperty, value); }
		}

		public static readonly DependencyProperty SelectInversionCommandProperty =
			DependencyProperty.Register("SelectInversionCommand", typeof(ICommand), typeof(SelectSampleConfigMenuView));
	}
}

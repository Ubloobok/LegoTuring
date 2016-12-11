using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LegoTuringMachine.ViewModel.Item;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;

namespace LegoTuringMachine.UI.Controls
{
	[TemplatePart(Name = TemplatePartNameCellLayout, Type = typeof(Grid))]
	[TemplatePart(Name = TemplatePartNameCurrentValueControl, Type = typeof(ContentControl))]
	public class CellControl : Control
	{
		private const string TemplatePartNameCellLayout = "PART_CellLayout";
		private const string TemplatePartNameCurrentValueControl="PART_CurrentValueControl";

		private static Random BrushesRandom = new Random();

		private static readonly Brush[] BackgroundBrushes = new Brush[]
		{
			Brushes.Chocolate,
			Brushes.Coral,
			Brushes.CornflowerBlue,
			Brushes.DarkOrange,
			Brushes.DeepSkyBlue,
			Brushes.DodgerBlue,
			Brushes.Gold,
			Brushes.GreenYellow,
			Brushes.Lime,
			Brushes.LawnGreen,
			Brushes.Khaki,
			Brushes.Ivory,
			Brushes.MediumSpringGreen,
			Brushes.Violet,
			Brushes.SpringGreen,
			Brushes.Salmon,
			Brushes.Plum,
		};

		public CellControl()
		{
            Background = BackgroundBrushes[BrushesRandom.Next(0, BackgroundBrushes.Length - 1)];
		}

		public CellItemVM Cell
		{
			get { return (CellItemVM)GetValue(CellProperty); }
			set { SetValue(CellProperty, value); }
		}

		public static readonly DependencyProperty CellProperty =
			DependencyProperty.Register("Cell", typeof(CellItemVM), typeof(CellControl));

		public DataTemplate CurrentValueTemplate
		{
			get { return (DataTemplate)GetValue(CurrentValueTemplateProperty); }
			set { SetValue(CurrentValueTemplateProperty, value); }
		}

		public static readonly DependencyProperty CurrentValueTemplateProperty =
			DependencyProperty.Register("CurrentValueTemplate", typeof(DataTemplate), typeof(CellControl));
	}
}

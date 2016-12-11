using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LegoTuringMachine.UI.Behavior
{
	public class GridBehavior
	{
		public static int GetRowsCount(DependencyObject obj)
		{
			return (int)obj.GetValue(RowsCountProperty);
		}

		public static void SetRowsCount(DependencyObject obj, int value)
		{
			obj.SetValue(RowsCountProperty, value);
		}

		public static readonly DependencyProperty RowsCountProperty =
			DependencyProperty.RegisterAttached("RowsCount", typeof(int), typeof(GridBehavior), new PropertyMetadata(OnRowsCountChanged));

		private static void OnRowsCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var grid = sender as Grid;
			if (grid != null)
			{
				int gridRowsCount =grid.RowDefinitions.Count;
				int newValue = (int)e.NewValue;
				if (newValue > gridRowsCount)
				{
					for (int i = 0; i < (newValue - gridRowsCount); i++)
					{
						grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
					}
				}
				else
				{
					for (int i = 0; i < (gridRowsCount - newValue); i++)
					{
						grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
					}
				}
			}
		}
	}
}

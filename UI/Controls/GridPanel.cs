using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace LegoTuringMachine.UI.Controls
{
	public class GridPanel : Grid
	{
		public GridPanel()
			: base()
		{
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			if (visualAdded != null)
			{
				App.GlobalDispatcher.BeginInvoke(new Action(() =>
					RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) })));
			}
			if (visualRemoved != null)
			{
				App.GlobalDispatcher.BeginInvoke(new Action(() =>
					RowDefinitions.RemoveAt(0)));
			}
			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i] != null)
				{
					Grid.SetRow(Children[i], i);
				}
			}
		}
	}
}

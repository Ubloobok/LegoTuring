using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LegoTuringMachine.ViewModel.Item;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace LegoTuringMachine.UI.Controls
{
	[TemplatePart(Name = TemplatePartAlphabetLayout, Type = typeof(Grid))]
	public class TapeControl : Control
	{
		public const string TemplatePartAlphabetLayout = "AlphabetLayout_PART";

		private Grid AlphabetLayout { get; set; }

		public TapeControl()
		{
		}

		public IEnumerable<LetterItemVM> Alphabet
		{
			get { return (IEnumerable<LetterItemVM>)GetValue(AlphabetProperty); }
			set { SetValue(AlphabetProperty, value); }
		}

		public static readonly DependencyProperty AlphabetProperty =
			DependencyProperty.Register("Alphabet", typeof(IEnumerable<LetterItemVM>), typeof(TapeControl));

		public IEnumerable<CellItemVM> Cells
		{
			get { return (IEnumerable<CellItemVM>)GetValue(CellsProperty); }
			set { SetValue(CellsProperty, value); }
		}

		public static readonly DependencyProperty CellsProperty =
			DependencyProperty.Register("Cells", typeof(IEnumerable<CellItemVM>), typeof(TapeControl));

		public DataTemplate AlphabetItemTemplate
		{
			get { return (DataTemplate)GetValue(AlphabetItemTemplateProperty); }
			set { SetValue(AlphabetItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty AlphabetItemTemplateProperty =
			DependencyProperty.Register("AlphabetItemTemplate", typeof(DataTemplate), typeof(TapeControl));
	}
}

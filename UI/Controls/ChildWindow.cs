using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Input;
using LegoTuringMachine.ViewModel.Core;

namespace LegoTuringMachine.UI.Controls
{
	[ContentProperty("Content")]
	[TemplatePart(Name = TemplatePartNameCloseButton, Type = typeof(Button))]
	public class ChildWindow : Control
	{
		public const string TemplatePartNameCloseButton = "PART_CloseButton";

		public object HeaderContent
		{
			get { return (object)GetValue(HeaderContentProperty); }
			set { SetValue(HeaderContentProperty, value); }
		}

		public static readonly DependencyProperty HeaderContentProperty =
			DependencyProperty.Register("HeaderContent", typeof(object), typeof(ChildWindow));

		public object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(object), typeof(ChildWindow));

		public object FooterContent
		{
			get { return (object)GetValue(FooterContentProperty); }
			set { SetValue(FooterContentProperty, value); }
		}

		public static readonly DependencyProperty FooterContentProperty =
			DependencyProperty.Register("FooterContent", typeof(object), typeof(ChildWindow));

		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}

		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.Register("IsOpen", typeof(bool), typeof(ChildWindow));

		public ICommand CloseCommand
		{
			get { return (ICommand)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}

		public static readonly DependencyProperty CloseCommandProperty = 
			DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(ChildWindow), new PropertyMetadata(OnCloseCommandChanged));

		private Button CloseButton { get; set; }

		public override void OnApplyTemplate()
		{
			CloseButton = GetTemplateChild(TemplatePartNameCloseButton) as Button;
			if (CloseButton != null)
			{
				CloseButton.Command =
					CloseCommand == null ?
					new ActionCommand(Close) :
					CloseCommand;
			}
		}

		private static void OnCloseCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((ChildWindow)sender).OnCloseCommandChanged(e);
		}

		private void OnCloseCommandChanged(DependencyPropertyChangedEventArgs e)
		{
			if (CloseButton != null)
			{
				CloseButton.Command =
					e.NewValue == null ?
					new ActionCommand(Close) :
					(ICommand)e.NewValue;
			}
		}

		private void Close()
		{
			SetCurrentValue(IsOpenProperty, false);
		}
	}
}

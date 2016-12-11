using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LegoTuringMachine.UI.Controls
{
	public class Transition : FrameworkElement
	{
		public enum TransitionState
		{
			A,
			B
		}

		public object Source
		{
			get { return this.GetValue(SourceProperty); }
			set { this.SetValue(SourceProperty, value); }
		}

		public object DisplayA
		{
			get { return this.GetValue(DisplayAProperty); }
			set { this.SetValue(DisplayAProperty, value); }
		}

		public object DisplayB
		{
			get { return this.GetValue(DisplayBProperty); }
			set { this.SetValue(DisplayBProperty, value); }
		}

		public TransitionState State
		{
			get { return (TransitionState)this.GetValue(StateProperty); }
			set { this.SetValue(StateProperty, value); }
		}

		public static DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(object), typeof(Transition), new PropertyMetadata(
				(obj, args) =>
				{
					((Transition)obj).Swap();
				}));

		public static DependencyProperty DisplayAProperty =
			DependencyProperty.Register("DisplayA", typeof(object), typeof(Transition));

		public static DependencyProperty DisplayBProperty =
			DependencyProperty.Register("DisplayB", typeof(object), typeof(Transition));

		public static DependencyProperty StateProperty =
			DependencyProperty.Register("State", typeof(TransitionState), typeof(Transition), new PropertyMetadata(TransitionState.A));

		private void Swap()
		{
			if (this.State == TransitionState.A)
			{
				this.DisplayB = this.Source;
				this.State = TransitionState.B;
			}
			else
			{
				this.DisplayA = this.Source;
				this.State = TransitionState.A;
			}
		}
	}
}

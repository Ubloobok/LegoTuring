using LegoTuringMachine.Device;
using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LegoTuringMachine.UI.Converters
{
    public class RuleItemVMToStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets string for empty letter value.
        /// </summary>
        public string EmptyLetterValueString { get; set; }

        /// <summary>
        /// Gets or sets string for final state.
        /// </summary>
        public string FinalStateString { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = null;
            RuleItemVM rule = value as RuleItemVM;
            // HACK: Пока для отображения просто будем превращать в строку.
            if (rule != null)
            {
                string newPositionString = GetDirectionString(rule.NewPosition);
                result = string.Format(
                    "{0} {1} → {2} {3} {4}",
                    rule.ConditionLetter.IsEmptyValue ? EmptyLetterValueString ?? string.Empty : rule.ConditionLetter.Value ?? string.Empty,
                    rule.ConditionState.IsFinalState ? FinalStateString ?? string.Empty : rule.ConditionState.Name ?? string.Empty,
                    rule.NewLetter.IsEmptyValue ? EmptyLetterValueString ?? string.Empty : rule.NewLetter.Value ?? string.Empty,
                    rule.NewState.IsFinalState ? FinalStateString ?? string.Empty : rule.NewState.Name ?? string.Empty,
                    newPositionString);
            }
            return result;
        }

        /// <summary>
        /// Converts direction enum value to string.
        /// </summary>
        private string GetDirectionString(Direction direction)
        {
            switch (direction)
            {
                case Direction.Hold:
                    return "▼";
                case Direction.Left:
                    return "Л";
                case Direction.Right:
                    return "П";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

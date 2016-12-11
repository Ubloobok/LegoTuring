using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LegoTuringMachine.Device;
using LegoTuringMachine.ViewModel.Item;
using System.Text.RegularExpressions;

namespace LegoTuringMachine.Implements
{
	public class RuleParsingResult
	{
		/// <summary>
		/// Gets or sets flag which indicates that parsing completed succesfully.
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Gets or sets parsing error message.
		/// </summary>
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets rule (parsing result).
		/// </summary>
		public RuleItemVM Rule { get; set; }
	}

	public class RulesParser
	{
		/// <summary>
		/// Gets or sets string associated with empty letter.
		/// </summary>
		public string EmptyLetterString { get; set; }

		/// <summary>
		/// Gets or sets string associated with initial (start) state.
		/// </summary>
		public string InitialStateString { get; set; }

		/// <summary>
		/// Gets or sets string associated with final (stop) state.
		/// </summary>
		public string FinalStateString { get; set; }

		/// <summary>
		/// Gets or sets string associated with 'Right' direction.
		/// </summary>
		public string RightDirectionString { get; set; }

		/// <summary>
		/// Gets or sets string associated with 'Left' direction.
		/// </summary>
		public string LeftDirectionString { get; set; }

		/// <summary>
		/// Gets or sets string associated with 'Hold' direction.
		/// </summary>
		public string HoldDirectionString { get; set; }

		/// <summary>
		/// Gets or sets string value for empty letter.
		/// </summary>
		public string EmptyLetterValue { get; set; }

		public IEnumerable<RuleParsingResult> ParseRulesList(string[] list)
		{
			foreach (string item in list)
			{
				yield return Parse(item);
			}
		}

		public RuleParsingResult Parse(string ruleString)
		{
			string normalizedRuleString = ruleString.Replace(" ", string.Empty);
			var ruleResult = new RuleParsingResult();
			ruleResult.Rule = new RuleItemVM();
			ruleResult.Rule.ConditionLetter = new LetterItemVM();
			ruleResult.Rule.ConditionState = new StateItemVM();
			ruleResult.Rule.NewLetter = new LetterItemVM();
			ruleResult.Rule.NewState = new StateItemVM();

			string dir_mas = RightDirectionString + LeftDirectionString + HoldDirectionString;
			string pattern = @"(.{1})q(\d+)\-\>(.{1})q(\d+)[" + dir_mas + "]";
			string finish_pattern = @"(.{1})q(\d+)\-\>(.{1})" + FinalStateString + "[" + dir_mas + "]";
			try
			{
				if (!(Regex.IsMatch(normalizedRuleString, pattern) || Regex.IsMatch(normalizedRuleString, finish_pattern)))
				{
					throw new Exception("Правило не соответствует формату.");
				}

				string[] list = normalizedRuleString.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
				string first = list[0];
				string second = list[1];

				ruleResult.Rule.ConditionLetter.Value = (first[0].ToString() == EmptyLetterString) ? EmptyLetterValue : first[0].ToString();
				string conditionStateString = first.Substring(1);
				ruleResult.Rule.ConditionState.Name = conditionStateString;
				if (conditionStateString == InitialStateString)
				{
					ruleResult.Rule.ConditionState.IsInitialState = true;
				}

				ruleResult.Rule.NewLetter.Value = (second[0].ToString() == EmptyLetterString) ? EmptyLetterValue : second[0].ToString();
				string newStateString = second.Substring(1, second.Length - 2);
				ruleResult.Rule.NewState.Name = newStateString;
				if (newStateString == InitialStateString)
				{
					ruleResult.Rule.NewState.IsInitialState = true;
				}
				else if (newStateString == FinalStateString)
				{
					ruleResult.Rule.NewState.IsFinalState = true;
				}
				string directionString = second[second.Length - 1].ToString();
				ruleResult.Rule.NewPosition = this.GetDirection(directionString);
			}
			catch (Exception e)
			{
				ruleResult.IsSuccess = false;
				ruleResult.Error = e.Message;
			}

			return ruleResult;
		}

		private Direction GetDirection(string directionString)
		{
			Direction direction =
				directionString == HoldDirectionString ?
				Direction.Hold :
					directionString == LeftDirectionString ?
					Direction.Left :
						directionString == RightDirectionString ?
						Direction.Right :
						Direction.Unknown;
			return direction;
		}
	}
}
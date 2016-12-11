using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using LegoTuringMachine.ViewModel.Item;
using LegoTuringMachine.Implements;
using System.Xml.Linq;
using System.Xml.XPath;
using LegoTuringMachine.Extensions;

namespace LegoTuringMachine.Model
{
	public class SettingsFacade : ISettingsFacade
	{
		/// <summary>
		/// Loads saved or default device settings.
		/// </summary>
		public DeviceSettings LoadDeviceSettings()
		{
			var settings = new DeviceSettings();
			var appSettings = ConfigurationManager.AppSettings;
			settings.DeviceComPort = appSettings["DefaultDS_DeviceComPort"];
			settings.EnginePorts = (DeviceMotorPort)int.Parse(appSettings["DefaultDS_EnginePorts"]);
			settings.EngineStepAngle = int.Parse(appSettings["DefaultDS_EngineStepAngle"]);
			settings.EngineStepPower = int.Parse(appSettings["DefaultDS_EngineStepPower"]);
			settings.EngineStepSonarPartAngle = int.Parse(appSettings["DefaultDS_EngineStepSonarPartAngle"]);
			settings.ManipulatorMaxAngle = int.Parse(appSettings["DefaultDS_ManipulatorMaxAngle"]);
			settings.ManipulatorPort = (DeviceMotorPort)int.Parse(appSettings["DefaultDS_ManipulatorPort"]);
			settings.ManipulatorPower = int.Parse(appSettings["DefaultDS_ManipulatorPower"]);
			settings.SonarPort = (DeviceSensorPort)int.Parse(appSettings["DefaultDS_SonarPort"]);
			settings.MaxBlockDistance = double.Parse(appSettings["DefaultDS_MaxBlockDistance"]);
			settings.MinBlockDistance = double.Parse(appSettings["DefaultDS_MinBlockDistance"]);
			settings.BlockWidth = double.Parse(appSettings["DefaultDS_BlockWidth"]);
			settings.BlockWidthAngle = double.Parse(appSettings["DefaultDS_BlockWidthAngle"]);
			settings.LeftTouchPort = (DeviceSensorPort)int.Parse(appSettings["DefaultDS_LeftTouchPort"]);
			settings.RightTouchPort = (DeviceSensorPort)int.Parse(appSettings["DefaultDS_RightTouchPort"]);
			settings.LeftManipulatorDirectionKoef = double.Parse(appSettings["DefaultDS_LeftManipulatorDirectionKoef"]);
			settings.RightManipulatorDirectionKoef = double.Parse(appSettings["DefaultDS_RightManipulatorDirectionKoef"]);
			return settings;
		}

		/// <summary>
		/// Saves device settings.
		/// </summary>
		public void SaveDeviceSettings(DeviceSettings deviceSettings)
		{
			var appSettings = ConfigurationManager.AppSettings;
			appSettings["DefaultDS_DeviceComPort"] = deviceSettings.DeviceComPort;
			appSettings["DefaultDS_EnginePorts"] = ((int)deviceSettings.EnginePorts).ToString();
			appSettings["DefaultDS_EngineStepAngle"] = deviceSettings.EngineStepAngle.ToString();
			appSettings["DefaultDS_EngineStepPower"] = deviceSettings.EngineStepPower.ToString();
			appSettings["DefaultDS_EngineStepSonarPartAngle"] = deviceSettings.EngineStepSonarPartAngle.ToString();
			appSettings["DefaultDS_ManipulatorMaxAngle"] = deviceSettings.ManipulatorMaxAngle.ToString();
			appSettings["DefaultDS_ManipulatorPort"] = ((int)deviceSettings.ManipulatorPort).ToString();
			appSettings["DefaultDS_ManipulatorPower"] = deviceSettings.ManipulatorPower.ToString();
			appSettings["DefaultDS_SonarPort"] = ((int)deviceSettings.SonarPort).ToString();
			appSettings["DefaultDS_MaxBlockDistance"] = deviceSettings.MaxBlockDistance.ToString();
			appSettings["DefaultDS_MinBlockDistance"] = deviceSettings.MinBlockDistance.ToString();
			appSettings["DefaultDS_BlockWidth"] = deviceSettings.BlockWidth.ToString();
			appSettings["DefaultDS_BlockWidthAngle"] = deviceSettings.BlockWidthAngle.ToString();
			appSettings["DefaultDS_LeftTouchPort"] = ((int)deviceSettings.LeftTouchPort).ToString();
			appSettings["DefaultDS_RightTouchPort"] = ((int)deviceSettings.RightTouchPort).ToString();
			appSettings["DefaultDS_LeftManipulatorDirectionKoef"] = deviceSettings.LeftManipulatorDirectionKoef.ToString();
			appSettings["DefaultDS_RightManipulatorDirectionKoef"] = deviceSettings.RightManipulatorDirectionKoef.ToString();
		}

		/// <summary>
		/// Loads saved or default common settings.
		/// </summary>
		public CommonSettings LoadCommonSettings()
		{
			var settings = new CommonSettings();
			var appSettings = ConfigurationManager.AppSettings;
			settings.BlocksCount = int.Parse(appSettings["DefaultCS_BlocksCount"]);
			return settings;
		}

		/// <summary>
		/// Saves common settings.
		/// </summary>
		public void SaveCommonSettings(CommonSettings commonSettings)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Loads pre-set (default) configurations.
		/// </summary>
		public IEnumerable<ComplexConfigItemVM> LoadPreSetConfigs()
		{
			string pathToFile = "Assets\\PreSets\\DefaultPreSets.xml";
			List<ComplexConfigItemVM> preSets = LoadPreSetConfigs(pathToFile);
			return preSets;
		}

		private static List<ComplexConfigItemVM> LoadPreSetConfigs(string pathToFile)
		{
			List<ComplexConfigItemVM> preSets = new List<ComplexConfigItemVM>();
			var parser = new RulesParser()
			{
				EmptyLetterString = "B",
				EmptyLetterValue = LetterItemVM.EmptyValue,
				InitialStateString = "q1",
				FinalStateString = "STOP",
				LeftDirectionString = "L",
				HoldDirectionString = "H",
				RightDirectionString = "R",
			};

			var preSetsDocument = XDocument.Load(pathToFile);
			var preSetElements = preSetsDocument.Root.XPathSelectElements("//PreSetConfigItem");
			foreach (var preSetElement in preSetElements)
			{
				var basicConfig = new BasicConfigItemVM();
				var rulesConfig = new RulesConfigItemVM();
				var plainTextElement = preSetElement.Element("PlainText");
				string plainText = plainTextElement == null ? string.Empty : plainTextElement.Value;

				string[] ruleStrings = plainText
					.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
					.Select(s => s.Trim())
					.Where(s => !string.IsNullOrEmpty(s))
					.ToArray();

				var parsedRules = ruleStrings.Select(s => parser.Parse(s));
				foreach (var parsedRule in parsedRules)
				{
					var conditionLetter =
						parsedRule.Rule.ConditionLetter == null ?
						null :
						basicConfig.Alphabet.FirstOrDefault(l => l.Value == parsedRule.Rule.ConditionLetter.Value)
							?? basicConfig.Alphabet.AddItem(parsedRule.Rule.ConditionLetter);
					var conditionState =
						parsedRule.Rule.ConditionState == null ?
						null :
						basicConfig.States.FirstOrDefault(s => s.Name == parsedRule.Rule.ConditionState.Name)
							?? basicConfig.States.AddItem(parsedRule.Rule.ConditionState);
					var newLetter =
						parsedRule.Rule.NewLetter == null ?
						null :
						basicConfig.Alphabet.FirstOrDefault(l => l.Value == parsedRule.Rule.NewLetter.Value)
							?? basicConfig.Alphabet.AddItem(parsedRule.Rule.NewLetter);
					var newState =
						parsedRule.Rule.NewState == null ?
						null :
						basicConfig.States.FirstOrDefault(s => s.Name == parsedRule.Rule.NewState.Name)
							?? basicConfig.States.AddItem(parsedRule.Rule.NewState);

					rulesConfig.Rules.Add(new RuleItemVM()
					{
						HasError = parsedRule.IsSuccess,
						Error = parsedRule.Error,
						ConditionLetter = conditionLetter,
						ConditionState = conditionState,
						NewLetter = newLetter,
						NewState = newState,
						NewPosition = parsedRule.Rule.NewPosition,
					});
				}

				var sortedAlphabet = basicConfig.Alphabet.OrderBy(l => l.Value).ToArray();
				basicConfig.Alphabet.Clear();
				basicConfig.Alphabet.AddRange(sortedAlphabet.Reverse());
				var sortedStates = basicConfig.States.OrderBy(s => s.Name).ToArray();
				basicConfig.States.Clear();
				basicConfig.States.AddRange(sortedStates);
				foreach (var rule in rulesConfig.Rules)
				{
					if (rule.ConditionLetter != null)
					{
						rule.ConditionLetter.AttachedBasicConfig = basicConfig;
					}
					if (rule.NewLetter != null)
					{
						rule.NewLetter.AttachedBasicConfig = basicConfig;
					}
				}

				var nameElement = preSetElement.Element("Name");
				string name = nameElement == null ? null : nameElement.Value;
				var preSet = new ComplexConfigItemVM()
				{
					Name = name,
					BasicConfig = basicConfig,
					RulesConfig = rulesConfig,
				};
				preSets.Add(preSet);
			}
			return preSets;
		}

		/// <summary>
		/// Loads sample config.
		/// </summary>
		/// <param name="sampleConfigName">Sample config name.</param>
		public ComplexConfigItemVM LoadSampleConfig(string sampleConfigName)
		{
			string pathToFile = "Assets\\PreSets\\" + sampleConfigName + ".xml";
			var config = LoadPreSetConfigs(pathToFile).First();
			return config;
		}
	}
}

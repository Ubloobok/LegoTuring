using LegoTuringMachine.Device;
using LegoTuringMachine.Implements;
using LegoTuringMachine.Model;
using LegoTuringMachine.ViewModel.Core;
using LegoTuringMachine.ViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.ViewModel
{
	public class SampleConfigForAdditionVM : BaseTuringMachineVM
	{
		public const string EmptyLetterValueSynonym = " ";

		public SampleConfigForAdditionVM(IControlDevice controlDevice, ISettingsFacade settingsFacade, SharedValue<int?> sharedActiveCellIndex)
			: base(controlDevice, settingsFacade, sharedActiveCellIndex)
		{
			FirstValueSelector = new SelectableCollection<CollectionItem<int>>()
			{
				new CollectionItem<int>() { DisplayValue = "1", Value = 1 },
				new CollectionItem<int>() { DisplayValue = "2", Value = 2 },
			};
			FirstValueSelector.SelectedItem = FirstValueSelector.First();
			SecondValueSelector = new SelectableCollection<CollectionItem<int>>()
			{
				new CollectionItem<int>() { DisplayValue = "1", Value = 1 },
				new CollectionItem<int>() { DisplayValue = "2", Value = 2 },
			};
			SecondValueSelector.SelectedItem = SecondValueSelector.First();

			var commonSettings = SettingsFacade.LoadCommonSettings();
			for (int i = 0; i < commonSettings.BlocksCount; i++)
			{
				Cells.Add(new CellItemVM()
				{
					IsActive = false,
					ValueState = CellValueState.Unknown,
				});
			}
			CurrentConfig = SettingsFacade.LoadSampleConfig("ScAddition");
		}

		public SelectableCollection<CollectionItem<int>> FirstValueSelector { get; private set; }
		public SelectableCollection<CollectionItem<int>> SecondValueSelector { get; private set; }

		protected override void OnBeforeWriting()
		{
			string inputString =
				(FirstValueSelector.SelectedItem.Value == 1 ? "1" : "11") +
				"+" +
				(SecondValueSelector.SelectedItem.Value == 1 ? "1" : "11") +
				EmptyLetterValueSynonym;
			for (int i = 0; i < inputString.Length; i++)
			{
				string c = inputString[i].ToString();
				Cells[i].NewValue = CurrentConfig.BasicConfig.Alphabet
					.FirstOrDefault(l =>
						((c == EmptyLetterValueSynonym) && l.IsEmptyValue)
						|| (l.Value == c));
			}
		}
	}
}

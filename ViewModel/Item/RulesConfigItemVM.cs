using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;
using System.Collections.ObjectModel;

namespace LegoTuringMachine.ViewModel.Item
{
	public class RulesConfigItemVM : ViewModelBase
	{
        public RulesConfigItemVM()
        {
            Rules = new ObservableCollection<RuleItemVM>();
        }

		public ObservableCollection<RuleItemVM> Rules { get; private set; }
	}
}

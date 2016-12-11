using LegoTuringMachine.ViewModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.ViewModel.Item
{
    public class ComplexConfigItemVM : ViewModelBase
    {
        private string _name;
        private string _description;
        private BasicConfigItemVM _basicConfig;
        private RulesConfigItemVM _rulesConfig;

        public ComplexConfigItemVM()
        {
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    RaisePropertyChanging<string>(() => Name);
                    _name = value;
                    RaisePropertyChanged<string>(() => Name);
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    RaisePropertyChanging<string>(() => Description);
                    _description = value;
                    RaisePropertyChanged<string>(() => Description);
                }
            }
        }

        public BasicConfigItemVM BasicConfig
        {
            get { return _basicConfig; }
            set
            {
                if (value != _basicConfig)
                {
                    RaisePropertyChanging<BasicConfigItemVM>(() => BasicConfig);
                    _basicConfig = value;
                    RaisePropertyChanged<BasicConfigItemVM>(() => BasicConfig);
                }
            }
        }

        public RulesConfigItemVM RulesConfig
        {
            get { return _rulesConfig; }
            set
            {
                if (value != _rulesConfig)
                {
                    RaisePropertyChanging<RulesConfigItemVM>(() => RulesConfig);
                    _rulesConfig = value;
                    RaisePropertyChanged<RulesConfigItemVM>(() => RulesConfig);
                }
            }
        }
    }
}

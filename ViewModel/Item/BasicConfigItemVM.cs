using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegoTuringMachine.ViewModel.Core;
using System.Collections.ObjectModel;

namespace LegoTuringMachine.ViewModel.Item
{
	public class BasicConfigItemVM : ViewModelBase
	{
		private string _name;
		private string _description;

		public BasicConfigItemVM()
		{
			States = new ObservableCollection<StateItemVM>();
			Alphabet = new ObservableCollection<LetterItemVM>();
		}

		/// <summary>
		/// Gets or sets configuration name.
		/// </summary>
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

		/// <summary>
		/// Gets or sets configuration description.
		/// </summary>
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

		/// <summary>
		/// Gets collection with defined configuration states.
		/// </summary>
		public ObservableCollection<StateItemVM> States
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets collection with defined configuration letters.
		/// </summary>
		public ObservableCollection<LetterItemVM> Alphabet { get; private set; }

		/// <summary>
		/// Gets empty letter from the alphabet or null, if it doesn't exist.
		/// </summary>
		public LetterItemVM EmptyAlphabetLetter
		{
			get
			{
				var letter = Alphabet.FirstOrDefault(l => l.IsEmptyValue);
				return letter;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

using LegoTuringMachine.ViewModel.Core;

namespace LegoTuringMachine.ViewModel.Item
{
	public class LetterItemVM : ViewModelBase
	{
		private string _value = string.Empty;
		private string _displayValueOverriden = null;
		private BasicConfigItemVM _attachedBasicConfig;

		public static string EmptyValue
		{
			get { return string.Empty; }
		}

		public LetterItemVM()
			: this(null)
		{
		}

		public LetterItemVM(string value)
		{
			Value = value;
		}

		/// <summary>
		/// Gets flag which indicates that it's empty value (symbol, string).
		/// </summary>
		public bool IsEmptyValue
		{
			get
			{
				bool isEmptyValue = Value == EmptyValue;
				return isEmptyValue;
			}
		}

		/// <summary>
		/// Gets or sets value of this letter.
		/// </summary>
		public string Value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					_value = value ?? EmptyValue;
					RaisePropertyChanged(() => Value);
					RaisePropertyChanged(() => ValueIndex);
					RaisePropertyChanged(() => IsEmptyValue);
				}
			}
		}

		/// <summary>
		/// Gets or sets value index in attached alphabet.
		/// </summary>
		public int? ValueIndex
		{
			get
			{
				int? result = null;
				if (AttachedBasicConfig != null)
				{
					var alphabetItem = AttachedBasicConfig.Alphabet.FirstOrDefault(l => l.Value == Value);
					if (alphabetItem != null)
					{
						result = AttachedBasicConfig.Alphabet.IndexOf(alphabetItem);
					}
					if (result < 0)
					{
						result = null;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Gets display value for this letter.
		/// </summary>
		public string DisplayValue
		{
			get { return _displayValueOverriden ?? _value; }
		}

		/// <summary>
		/// Gets overriden display value for this letter, instead of original value.
		/// </summary>
		public string DisplayValueOverriden
		{
			get { return _displayValueOverriden; }
			set
			{
				_displayValueOverriden = value;
				RaisePropertyChanged(() => DisplayValue);
				RaisePropertyChanged(() => DisplayValueOverriden);
				RaisePropertyChanged(() => IsDisplayValueOverriden);
			}
		}

		/// <summary>
		/// Gets flag which indicates that display value for this letter is overriden.
		/// </summary>
		public bool IsDisplayValueOverriden
		{
			get { return _displayValueOverriden != null; }
		}

		/// <summary>
		/// Gets or set attached basic config for this letter.
		/// </summary>
		public BasicConfigItemVM AttachedBasicConfig
		{
			get { return _attachedBasicConfig; }
			set
			{
				if (value != _attachedBasicConfig)
				{
					if (_attachedBasicConfig != null)
					{
						_attachedBasicConfig.Alphabet.CollectionChanged -= OnAlphabetCollectionChanged;
					}
					if (value != null)
					{
						value.Alphabet.CollectionChanged -= OnAlphabetCollectionChanged;
						value.Alphabet.CollectionChanged += OnAlphabetCollectionChanged;
					}
					_attachedBasicConfig = value;
					RaisePropertyChanged<BasicConfigItemVM>(() => AttachedBasicConfig);
				}
			}
		}

		/// <summary>
		/// Method called when alphabet of the attached basic config will change.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnAlphabetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanging<int?>(() => ValueIndex);
			RaisePropertyChanged<int?>(() => ValueIndex);
		}

		/// <summary>
		/// Determines whether the specified letter is equal to the current letter.
		/// </summary>
		//public override bool Equals(object obj)
		//{
		//	bool result = Equals(obj as LetterItemVM);
		//	return result;
		//}

		/// <summary>
		/// Determines whether the specified letter is equal to the current letter.
		/// </summary>
		//public bool Equals(LetterItemVM other)
		//{
		//	if (other == null)
		//	{
		//		return false;
		//	}
		//	bool result = this.Value == other.Value;
		//	return result;
		//}

		/// <summary>
		/// Convert letter to string, with debug information.
		/// </summary>
		public override string ToString()
		{
			string str = string.Format("Value=\"{0}\", ValueIndex={1}, IsEmptyValue={2}", Value, ValueIndex, IsEmptyValue);
			return str;
		}
	}
}

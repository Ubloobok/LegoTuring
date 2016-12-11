using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Extensions
{
	public static class DictionaryExtensions
	{
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
			where TKey : class
			where TValue : class
		{
			if (dictionary == null)
			{
				return null;
			}
			TValue value = null;
			dictionary.TryGetValue(key, out value);
			return value;
		}
	}
}

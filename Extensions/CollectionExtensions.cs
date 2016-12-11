using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegoTuringMachine.Extensions
{
	public static class CollectionExtensions
	{
        public static T AddItem<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return item;
        }

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
		{
			if (range != null)
			{
				foreach (var item in range)
				{
					collection.Add(item);
				}
			}
		}
	}
}

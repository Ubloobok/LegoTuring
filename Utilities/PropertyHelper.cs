using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LegoTuringMachine.Utilities
{
	public class PropertyHelper
	{
		public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			string propertyName = ((MemberExpression)propertyExpression.Body).Member.Name;
			return propertyName;
		}
	}
}

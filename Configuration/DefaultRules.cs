using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public class DefaultRules : IRules
	{
		public string GetIdName(Type type)
		{
			Type bType = type.BaseType;
			return "Id";
			//if (type.BaseType == typeof(object))
			//{
			//	return "Id";
			//}
			//else
			//{
			//	return string.Concat(bType.Name, "Id");
			//}

		}

		public string GetTableName(Type type)
		{
			return type.Name;
		}
	}
}

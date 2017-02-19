using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public class Util
	{
		public static SqlDbType GetSqlDbType(Type type, string propName)
		{
			var prop = type.GetProperty(propName);
			SqlDbType sqlType = SqlDbType.BigInt;

			var ts = new TypeSwitch()
				.Case<string>(() => sqlType = SqlDbType.VarChar)
				.Case<int>(() => sqlType = SqlDbType.Int)
				.Case<DateTime>(() => sqlType = SqlDbType.DateTime2)
				.Case<bool>(() => sqlType = SqlDbType.Bit)
				.Case<decimal>(() => sqlType = SqlDbType.Decimal);

			ts.Switch(prop.PropertyType);
			return sqlType;
		}

		public static object GetModelColumnValue(Type type, object model, string propName)
		{
			var prop = type.GetProperty(propName);
			if (prop != null)
			{
				var value = prop.GetValue(model, null);
				return value;
			}

			return null;
		}
	}
}

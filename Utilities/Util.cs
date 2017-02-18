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
		public static SqlDbType GetSqlDbType(Type type, string column)
		{
			var prop = type.GetProperty(column);
			SqlDbType sqlType = SqlDbType.BigInt;

			var ts = new TypeSwitch()
				.Case<string>(() => sqlType = SqlDbType.VarChar)
				.Case<int>(() => sqlType = SqlDbType.Int)
				.Case<DateTime>(() => sqlType = SqlDbType.DateTime2)
				.Case<bool>(() => sqlType = SqlDbType.Bit);

			ts.Switch(prop.PropertyType);
			return sqlType;
		}

		public static object GetModelColumnValue(Type type, object model, string column)
		{
			var prop = type.GetProperty(column);
			if (prop != null)
			{
				var value = prop.GetValue(model, null);
				return value;
			}

			return null;
		}
	}
}

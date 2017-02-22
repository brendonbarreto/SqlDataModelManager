using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public interface IRules
	{
		string GetModelIdName(Type type);

		string GetTableIdName(Type type);

		string ToTableName(Type type);

		string ToPropertyName(Type type, string columnName);

		string ToColumnName(Type type, string propertyName);

		string ComplexPropertyName(Type type, string propertyName);
	}
}

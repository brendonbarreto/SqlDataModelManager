using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbTransaction
{
	public class DbPropertyColumn
	{
		public string ColumnName { get; set; }

		public string PropertyName { get; set; }

		public object Value { get; set; }

		public bool IsComplexProperty { get; set; }
	}
}

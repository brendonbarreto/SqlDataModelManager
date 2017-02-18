using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
	public interface IRules
	{
		string GetIdName(Type type);

		string GetTableName(Type type);
	}
}

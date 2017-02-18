using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public class TypeSwitch
	{
		Dictionary<Type, Action> Matches = new Dictionary<Type, Action>();

		public TypeSwitch Case<T>(Action action)
		{
			Matches.Add(typeof(T), action);
			return this;
		}

		public void Switch(Type type)
		{
			Matches[type]();
		}
	}
}

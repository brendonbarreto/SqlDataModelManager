using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModels
{
	public class Job
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual string Type { get; set; }

		public virtual List<Employee> Employees { get; set; }
	}
}

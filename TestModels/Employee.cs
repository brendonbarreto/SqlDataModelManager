using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModels
{
	public class Employee
	{
		public virtual int Id { get; set; }

		public virtual int Age { get; set; }

		public virtual decimal Wage { get; set; }

		public virtual List<Job> Jobs { get; set; }
	}
}

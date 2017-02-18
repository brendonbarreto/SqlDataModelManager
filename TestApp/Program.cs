using Configuration;
using DbTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var model = new PhysicalPerson();
			model.Id = 0;
			model.Name = "Brendon ijef";
			model.BornDate = DateTime.Now;
			var dbModel = new DbModel<PhysicalPerson>(model);
			Connection.ConnectionString = "Server=localhost;Database=hue;Trusted_Connection=True;";
			dbModel.Save();
		}
	}

	public class Person
	{
		public int Id { get; set; }

		public string Name { get; set; }

	}

	public class PhysicalPerson : Person
	{
		public DateTime BornDate { get; set; }
	}
}

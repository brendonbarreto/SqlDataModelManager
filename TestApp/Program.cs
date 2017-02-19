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
			Connection.ConnectionString = "Server=localhost;Database=hue;Trusted_Connection=True;";
			var model = new Part();
			model.Code = "123";
			model.Description = "Descrição";
			model.Id = 1;
			model.Quantity = 23;
			model.StockLocationId = 7;
			model.Weight = (decimal)2.76;
			var dbModel = new DbModel<Part>(model);
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


	public class Item
	{
		public int Id { get; set; }

		public string Code { get; set; }

		public string Description { get; set; }
	}

	public class Material : Item
	{
		public decimal Weight { get; set; }

		public int Quantity { get; set; }
	}

	public class Part : Material
	{
		public int StockLocationId { get; set; }
	}
}

using DbTransaction;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestModels;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			ConfigureAutoMapping();

			Configuration.Connection.ConnectionString = "Server=localhost;Database=hue;Trusted_Connection=True;";
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

		public static void ConfigureAutoMapping()
		{
			NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
			config.SetNamingStrategy(DefaultNamingStrategy.Instance);

			FluentNHibernate.Cfg.FluentConfiguration fConfig = FluentNHibernate.Cfg.Fluently.Configure(config);
			var sessionFactory = Fluently.Configure(config)
			  .Database(MsSqlConfiguration.MsSql2012.ConnectionString(new Action<ConnectionStringBuilder>(GenerateConnStr)))
			  .ExposeConfiguration(BuildSchema)
			  .Mappings(m =>
				m.AutoMappings
				  .Add(AutoMap.AssemblyOf<Product>()))
			  .BuildSessionFactory();

			var k = sessionFactory.OpenSession();
		}

		private static void BuildSchema(NHibernate.Cfg.Configuration config)
		{
			try
			{
				new SchemaValidator(config).Validate();
			}
			catch
			{
				new SchemaUpdate(config).Execute(true, true);
			}
		}

		private static void GenerateConnStr(ConnectionStringBuilder conn)
		{
			conn.Is("Data Source=SERVIDORTESTE\\SQLEXPRESS2014;Initial Catalog=potato;Persist Security Info=True;User ID=producao;password=producao;MultipleActiveResultSets=True;Max Pool Size = 10000");
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

		public ItemStatus Status { get; set; }
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

	public class ItemStatus
	{
		public bool Active { get; set; }

		[Configuration.DbIgnore]
		public bool Deleted { get; set; }

		public int StatusId { get; set; }
	}
}

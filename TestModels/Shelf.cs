using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModels
{
	public class Shelf
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IList<Product> Products { get; set; }

		public Shelf()
		{
			Products = new List<Product>();
		}
	}
}

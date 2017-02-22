using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Configuration
{
	public class DefaultRules : IRules
	{
		private static CultureInfo culture = new CultureInfo("en-US");

		private static PluralizationService pluralizationService = PluralizationService.CreateService(culture);

		public string GetModelIdName(Type type)
		{
			return "Id";
		}

		public string GetTableIdName(Type type)
		{
			Type bType = type.BaseType;
			if (bType == typeof(object))
			{
				return "Id";
			}
			else
			{
				return string.Concat(bType.Name, "Id");
			}
		}

		public string ToPropertyName(Type type, string columnName)
		{
			var prop = type.GetProperty(columnName);
			if (prop == null)
			{
				if (columnName.EndsWith("Id"))
				{
					return "Id";
				}

				throw new NullReferenceException("Invalid column name");
			}

			return prop.Name;
		}

		public string ToColumnName(Type type, string propertyName)
		{
			if (GetModelIdName(type) == propertyName)
			{
				return GetTableIdName(type);
			}
			else
			{
				return propertyName;
			}
		}

		public string ToTableName(Type type)
		{
			string name = PluralizeCamelCase(type.Name);
			return name;
		}

		private string PluralizeCamelCase(string text)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				if (i > 0 && char.IsUpper(text[i]))
				{
					builder.Append(" ");
				}

				builder.Append(text[i]);
			}

			string name = pluralizationService.Pluralize(builder.ToString());
			name = culture.TextInfo.ToTitleCase(name).Replace(" ", string.Empty);
			return name;
		}

		public string ComplexPropertyName(Type type, string propertyName)
		{
			return string.Join("_", propertyName.Split('.'));
		}
	}
}

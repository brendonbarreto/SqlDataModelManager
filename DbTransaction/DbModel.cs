using Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DbTransaction
{
	public class DbModel<T> : DbModel where T : class, new()
	{
		public DbModel(T model) : base(typeof(T), model)
		{
		}
	}

	public class DbModel
	{
		private object Model { get; set; }

		private Type ModelType { get; set; }

		private IRules Rules { get; set; }

		private bool IsNew { get; set; }

		protected DbModel(Type type, object model)
		{
			ModelType = type;
			Model = model;
			Rules = new DefaultRules();
			IsNew = GetIdValue() == 0;
		}

		private PropertyInfo GetIdProperty()
		{
			return ModelType.GetProperty(Rules.GetModelIdName(ModelType));
		}

		private int GetIdValue()
		{
			var prop = GetIdProperty();
			return prop != null ? (int)prop.GetValue(Model, null) : 0;
		}

		public void Save(bool saveInheritance = true)
		{
			GetTypePersistantProperties();
			var idProp = GetIdProperty();
			if (Model != null && idProp != null)
			{
				if (ModelType.BaseType == typeof(object) || !saveInheritance)
				{
					if (IsNew)
					{
						idProp.SetValue(Model, Insert());
					}
					else
					{
						Update();
					}
				}
				else
				{
					var inhModel = new DbModel(ModelType.BaseType, Model);
					inhModel.Save();
					Save(false);
				}
			}
		}

		private List<SqlParameter> GetSaveParameters(List<string> columns)
		{
			List<SqlParameter> pars = new List<SqlParameter>();
			columns.ForEach(column =>
			{
				var propName = Rules.ToPropertyName(ModelType, column);
				var value = Util.GetModelColumnValue(ModelType, Model, propName);
				pars.Add(new SqlParameter()
				{
					ParameterName = string.Concat("@", column),
					SqlDbType = Util.GetSqlDbType(ModelType, propName),
					Value = value
				});
			});

			return pars;
		}

		public void GetTypePersistantProperties()
		{
			List<PropertyInfo> dbProps = new List<PropertyInfo>();
			foreach (var prop in ModelType.GetProperties())
			{
				if(!Rules.IgnoreProperty(ModelType, prop.Name))
				{
					dbProps.Add(prop);
				}
			}
		}

		private int Insert()
		{
			var columns = SchemaInfo.GetAllColumns(Rules.ToTableName(ModelType));
			if (ModelType.BaseType == typeof(object))
			{
				columns.Remove(Rules.GetTableIdName(ModelType));
			}

			List<SqlParameter> pars = GetSaveParameters(columns);

			string sql = string.Format(@"INSERT INTO {0}({1}) OUTPUT inserted.{2} VALUES({3})",
				Rules.ToTableName(ModelType),
				string.Join(", ", columns),
				Rules.GetTableIdName(ModelType),
				string.Join(", ", columns.Select(m => string.Concat("@", m))));
			int id = 0;
			using (SqlConnection conn = new SqlConnection(Connection.ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand(sql, conn))
				{
					command.Parameters.AddRange(pars.ToArray());
					id = (int)command.ExecuteScalar();

				}
			}

			return id;
		}

		private string GetUpdateFieldsString(List<SqlParameter> pars)
		{
			StringBuilder builder = new StringBuilder();
			var uPars = pars.Where(m => m.ParameterName != string.Concat("@", Rules.GetTableIdName(ModelType)));
			if (uPars != null && uPars.Count() > 0)
			{
				builder.Append("SET ");
				for (int i = 0; i < uPars.Count(); i++)
				{
					var par = uPars.ElementAt(i);
					var columnName = par.ParameterName.Remove(0, 1);
					builder.AppendFormat("{0} = {1}",
						columnName,
						par.ParameterName);
					if (i != (uPars.Count() - 1))
					{
						builder.Append(", ");
					}
				}
			}

			return builder.ToString();
		}

		private void Update()
		{
			var columns = SchemaInfo.GetAllColumns(Rules.ToTableName(ModelType));
			List<SqlParameter> pars = GetSaveParameters(columns);
			var idName = Rules.GetTableIdName(ModelType);
			string sql = string.Format(@"UPDATE {0} {1} WHERE {2} = {3}",
				Rules.ToTableName(ModelType),
				GetUpdateFieldsString(pars),
				idName,
				string.Concat("@", idName));
			using (SqlConnection conn = new SqlConnection(Connection.ConnectionString))
			{
				conn.Open();
				using (SqlCommand command = new SqlCommand(sql, conn))
				{
					command.Parameters.AddRange(pars.ToArray());
					command.ExecuteNonQuery();
				}
			}
		}
	}
}

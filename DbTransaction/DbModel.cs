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
		public object Model { get; set; }

		public Type ModelType { get; set; }

		public IRules Rules { get; set; }

		public bool IsNew { get; set; }

		public DbModel(Type type, object model)
		{
			ModelType = type;
			Model = model;
			Rules = new DefaultRules();
			IsNew = GetIdValue() == 0;
		}

		private PropertyInfo GetIdProperty()
		{
			return ModelType.GetProperty(Rules.GetIdName(ModelType));
		}

		private int GetIdValue()
		{
			var prop = GetIdProperty();
			return prop != null ? (int)prop.GetValue(Model, null) : 0;
		}

		public void Save(bool saveInheritance = true)
		{
			//IS PERSISTANT MODEL
			var idProp = GetIdProperty();
			if (Model != null && idProp != null)
			{
				//CONTAINS NO INHERITANCE
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
				var value = Util.GetModelColumnValue(ModelType, Model, column);
				pars.Add(new SqlParameter()
				{
					ParameterName = string.Concat("@", column),
					SqlDbType = Util.GetSqlDbType(ModelType, column),
					Value = value
				});
			});

			return pars;
		}

		private int Insert()
		{
			var columns = SchemaInfo.GetAllColumns(Rules.GetTableName(ModelType));
			if (ModelType.BaseType == typeof(object))
			{
				columns.Remove(Rules.GetIdName(ModelType));
			}

			List<SqlParameter> pars = GetSaveParameters(columns);

			string sql = string.Format(@"INSERT INTO {0}({1}) OUTPUT inserted.{2} VALUES({3})",
				Rules.GetTableName(ModelType),
				string.Join(", ", columns),
				Rules.GetIdName(ModelType),
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
			var uPars = pars.Where(m => m.ParameterName != string.Concat("@", Rules.GetIdName(ModelType)));
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
			var columns = SchemaInfo.GetAllColumns(Rules.GetTableName(ModelType));
			List<SqlParameter> pars = GetSaveParameters(columns);
			var idName = Rules.GetIdName(ModelType);
			string sql = string.Format(@"UPDATE {0} {1} WHERE {2} = {3}",
				Rules.GetTableName(ModelType),
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

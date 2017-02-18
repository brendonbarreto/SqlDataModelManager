using Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	public class SchemaInfo
	{
		public static List<string> GetAllColumns(string tableName)
		{
			using (SqlConnection conn = new SqlConnection(Connection.ConnectionString))
			{
				conn.Open();
				List<string> names = new List<string>();
				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = conn;
					command.CommandText = @"select COLUMN_NAME
						from INFORMATION_SCHEMA.COLUMNS
						where TABLE_NAME=@tableName";
					command.Parameters.AddWithValue("tableName", tableName);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							names.Add(reader.GetString(0));
						}
					}
				}

				return names;
			}
		}
	}
}

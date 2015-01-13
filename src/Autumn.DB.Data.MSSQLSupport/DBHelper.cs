using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace Autumn.DB.Data.MSSQLSupport
{
	internal class DBHelper : IDBHelper
	{
		private static DataSet _schema = null;
		private static Dictionary<string, string> _DBTypeMapping;
		public SqlConnection selCon = null;

        public string DBObjectPrefix { get { return "["; } }
        public string DBObjectSuffix { get { return "]"; } }

		public Dictionary<string, string> DBTypeMapping
		{
			get
			{
				if (DBHelper._DBTypeMapping == null)
				{
					DBHelper._DBTypeMapping = new Dictionary<string, string>();
					DBHelper._DBTypeMapping.Add("tinyint", "sbyte?");
					DBHelper._DBTypeMapping.Add("timestamp", "byte[]");
					DBHelper._DBTypeMapping.Add("binary", "byte[]");
					DBHelper._DBTypeMapping.Add("image", "byte[]");
					DBHelper._DBTypeMapping.Add("varbinary", "byte[]");
					DBHelper._DBTypeMapping.Add("bit", "bool?");
					DBHelper._DBTypeMapping.Add("smallint", "short?");
					DBHelper._DBTypeMapping.Add("int", "int?");
					DBHelper._DBTypeMapping.Add("bigint", "long?");
					DBHelper._DBTypeMapping.Add("float", "double?");
					DBHelper._DBTypeMapping.Add("money", "decimal?");
					DBHelper._DBTypeMapping.Add("smallmoney", "decimal?");
					DBHelper._DBTypeMapping.Add("decimal", "decimal?");
					DBHelper._DBTypeMapping.Add("numeric", "decimal?");
					DBHelper._DBTypeMapping.Add("text", "string");
					DBHelper._DBTypeMapping.Add("ntext", "string");
					DBHelper._DBTypeMapping.Add("xml", "string");
					DBHelper._DBTypeMapping.Add("varchar", "string");
					DBHelper._DBTypeMapping.Add("char", "string");
					DBHelper._DBTypeMapping.Add("nchar", "string");
					DBHelper._DBTypeMapping.Add("nvarchar", "string");
					DBHelper._DBTypeMapping.Add("datetime", "DateTime?");
					DBHelper._DBTypeMapping.Add("smalldatetime", "DateTime?");
					DBHelper._DBTypeMapping.Add("real", "Single?");
					DBHelper._DBTypeMapping.Add("uniqueidentifier", "Guid?");
					DBHelper._DBTypeMapping.Add("sql_variant", "object");
				}
				return DBHelper._DBTypeMapping;
			}
		}
		public DBHelper()
		{
		}
		public DBHelper(string constr)
		{
			this.selCon = (SqlConnection)this.CreateConnection(constr);
		}
		public DBHelper(SqlConnection con)
		{
			this.selCon = con;
		}
		public int ExecuteNonQuery(string sqlCommand)
		{
			return this.ExecuteNonQuery(sqlCommand, this.CreateConnection());
		}
		public int ExecuteNonQuery(string sqlCommand, object connection)
		{
			SqlConnection sqlConnection = (SqlConnection)connection;
			sqlConnection.Open();
			SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
			sqlCommand2.CommandText = sqlCommand;
			int result = sqlCommand2.ExecuteNonQuery();
			sqlCommand2.Dispose();
			sqlConnection.Close();
			sqlConnection.Dispose();
			return result;
		}
		public object ExecuteScalar(string sqlCommand)
		{
			return this.ExecuteScalar(sqlCommand, this.CreateConnection());
		}
		public object ExecuteScalar(string sqlCommand, object connection)
		{
			SqlConnection sqlConnection = (SqlConnection)connection;
			sqlConnection.Open();
			SqlCommand sqlCommand2 = sqlConnection.CreateCommand();
			sqlCommand2.CommandText = sqlCommand;
			object result = sqlCommand2.ExecuteScalar();
			sqlCommand2.Dispose();
			sqlConnection.Close();
			sqlConnection.Dispose();
			return result;
		}
		public SqlDataReader ExecuteReader(string sqlCommand, SqlConnection connection)
		{
			ConnectionState state = connection.State;
			if (state != ConnectionState.Closed)
			{
				if (state == ConnectionState.Broken)
				{
					connection.Close();
					connection.Open();
				}
			}
			else
			{
				connection.Open();
			}
			SqlCommand sqlCommand2 = connection.CreateCommand();
			sqlCommand2.CommandText = sqlCommand;
			SqlDataReader result = sqlCommand2.ExecuteReader();
			sqlCommand2.Dispose();
			return result;
		}
		public DataTable ExecuteTable(string sqlCommand)
		{
			return this.ExecuteTable(sqlCommand, this.CreateConnection());
		}
		public DataTable ExecuteTable(string sqlCommand, object connection)
		{
			SqlConnection sqlConnection = (SqlConnection)connection;
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand, sqlConnection);
			DataTable dataTable = new DataTable();
			sqlDataAdapter.Fill(dataTable);
			sqlDataAdapter.Dispose();
			sqlConnection.Close();
			sqlConnection.Dispose();
			return dataTable;
		}
		public int ExecuteNumber(string sqlCommand)
		{
			return this.ExecuteNumber(sqlCommand, this.CreateConnection());
		}
		public int ExecuteNumber(string sqlCommand, object connection)
		{
			object obj = this.ExecuteScalar(sqlCommand, connection);
			return (obj == null || obj.ToString() == "") ? -1 : int.Parse(obj.ToString());
		}
		public object CreateConnection(string constr)
		{
			return ConnectionPool.CreateConnection(constr);
		}
		public object CreateConnection()
		{
			return (this.selCon == null) ? ConnectionPool.CreateConnection() : this.selCon;
		}
		public void ClearSchema()
		{
			if (DBHelper._schema != null)
			{
				DBHelper._schema.Dispose();
				DBHelper._schema = null;
			}
			ConnectionPool.ClearInfo();
		}
		public DataTable GetSchema(SchemaInfoType infoType)
		{
			if (DBHelper._schema == null)
			{
				DBHelper._schema = new DataSet("RuntimeSchemaFormAutumn");
				SqlConnection sqlConnection = ConnectionPool.CreateConnection();
				sqlConnection.Open();
				DBHelper._schema.Tables.AddRange(new DataTable[]
				{
					sqlConnection.GetSchema("MetaDataCollections"),
					sqlConnection.GetSchema("DataSourceInformation"),
					sqlConnection.GetSchema("DataTypes"),
					sqlConnection.GetSchema("Restrictions"),
					sqlConnection.GetSchema("ReservedWords"),
					sqlConnection.GetSchema("Users"),
					sqlConnection.GetSchema("Databases"),
					sqlConnection.GetSchema("Tables"),
					sqlConnection.GetSchema("Columns"),
					sqlConnection.GetSchema("Views"),
					sqlConnection.GetSchema("ViewColumns"),
					sqlConnection.GetSchema("ProcedureParameters"),
					sqlConnection.GetSchema("Procedures"),
					sqlConnection.GetSchema("ForeignKeys"),
					sqlConnection.GetSchema("IndexColumns"),
					sqlConnection.GetSchema("Indexes"),
					sqlConnection.GetSchema("UserDefinedTypes")
				});
				sqlConnection.Close();
			}
			return DBHelper._schema.Tables.Contains(infoType.ToString()) ? DBHelper._schema.Tables[infoType.ToString()] : null;
		}
    }
}

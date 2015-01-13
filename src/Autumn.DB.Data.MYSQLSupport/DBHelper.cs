using Autumn.DB.Config;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Autumn.DB.Data.MYSQLSupport
{
	public class DBHelper : IDBHelper
	{
		private static string connectionString = null;
		public MySqlConnection selCon = null;

        public string DBObjectPrefix { get { return "`"; } }
        public string DBObjectSuffix { get { return "`"; } }

		private static Dictionary<string, string> _DBTypeMapping;
		private static string ConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(DBHelper.connectionString))
				{
					DBHelper.connectionString = ConfigManager.ConfigurationData.ConnectionString;
				}
				return DBHelper.connectionString;
			}
		}
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
        public DBHelper() { }
        public DBHelper(string constr) { this.selCon = (MySqlConnection)this.CreateConnection(constr); }
        public DBHelper(MySqlConnection con){ this.selCon = con; }
        public object CreateConnection()
		{
			return (this.selCon == null) ? this.CreateConnection(DBHelper.ConnectionString) : this.selCon;
		}
		public object CreateConnection(string constr)
		{
			return new MySqlConnection(constr);
		}
		public MySqlConnection CreateMySqlConnection()
		{
			return (MySqlConnection)this.CreateConnection();
		}
		public MySqlConnection CreateMySqlConnection(string conStr)
		{
			return (MySqlConnection)this.CreateConnection(conStr);
		}
		public DataTable GetSchema(SchemaInfoType infoType)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public void ClearSchema()
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public int ExecuteNonQuery(string sqlCommand)
		{
			return this.ExecuteNonQuery(sqlCommand, this.CreateConnection());
		}
		public int ExecuteNonQuery(string sqlCommand, object connection)
		{
			MySqlConnection mySqlConnection = (MySqlConnection)connection;
			mySqlConnection.Open();
			MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
			mySqlCommand.CommandText = sqlCommand;
			int result = mySqlCommand.ExecuteNonQuery();
			mySqlCommand.Dispose();
			mySqlConnection.Close();
			mySqlConnection.Dispose();
			return result;
		}
		public object ExecuteScalar(string sqlCommand)
		{
			return this.ExecuteScalar(sqlCommand, this.CreateConnection());
		}
		public object ExecuteScalar(string sqlCommand, object connection)
		{
			MySqlConnection mySqlConnection = (MySqlConnection)connection;
			mySqlConnection.Open();
			MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
			mySqlCommand.CommandText = sqlCommand;
			object result = mySqlCommand.ExecuteScalar();
			mySqlCommand.Dispose();
			mySqlConnection.Close();
			mySqlConnection.Dispose();
			return result;
		}
		public DataTable ExecuteTable(string sqlCommand)
		{
			return this.ExecuteTable(sqlCommand, this.CreateConnection());
		}
		public DataTable ExecuteTable(string sqlCommand, object connection)
		{
			MySqlConnection mySqlConnection = (MySqlConnection)connection;
			MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(sqlCommand, mySqlConnection);
			DataTable dataTable = new DataTable();
			mySqlDataAdapter.Fill(dataTable);
			mySqlDataAdapter.Dispose();
			mySqlConnection.Close();
			mySqlConnection.Dispose();
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
	}
}

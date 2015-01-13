using Autumn.DB.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
namespace Autumn.DB.Data.ACCESSSupport
{
	internal class DBHelper : IDBHelper
	{
		private static Dictionary<string, string> _DBTypeMapping;
		protected OleDbConnection selCon = null;
		private static DataSet _schema = null;

        public string DBObjectPrefix { get { return "["; } }
        public string DBObjectSuffix { get { return "]"; } }

		public Dictionary<string, string> DBTypeMapping
		{
			get
			{
				if (DBHelper._DBTypeMapping == null)
				{
					DBHelper._DBTypeMapping = new Dictionary<string, string>();
					DBHelper._DBTypeMapping.Add("2", "short?");
					DBHelper._DBTypeMapping.Add("3", "int?");
					DBHelper._DBTypeMapping.Add("4", "Single?");
					DBHelper._DBTypeMapping.Add("5", "double?");
					DBHelper._DBTypeMapping.Add("6", "decimal?");
					DBHelper._DBTypeMapping.Add("7", "DateTime?");
					DBHelper._DBTypeMapping.Add("11", "bool?");
					DBHelper._DBTypeMapping.Add("17", "byte?");
					DBHelper._DBTypeMapping.Add("72", "Guid?");
					DBHelper._DBTypeMapping.Add("128", "byte[]");
					DBHelper._DBTypeMapping.Add("130", "string");
					DBHelper._DBTypeMapping.Add("131", "decimal?");
				}
				return DBHelper._DBTypeMapping;
			}
		}
		public DBHelper()
		{
		}
		public DBHelper(string constr)
		{
			this.selCon = this.CreateOleDbConnection(constr);
		}
		public DBHelper(OleDbConnection con)
		{
			this.selCon = con;
		}
		public int ExecuteNonQuery(string sqlCommand)
		{
			return this.ExecuteNonQuery(sqlCommand, this.CreateConnection());
		}
		public int ExecuteNonQuery(string sqlCommand, object connection)
		{
			OleDbConnection oleDbConnection = (OleDbConnection)connection;
			oleDbConnection.Open();
			OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
			oleDbCommand.CommandText = sqlCommand;
			int result = oleDbCommand.ExecuteNonQuery();
			oleDbCommand.Dispose();
			oleDbConnection.Close();
			oleDbConnection.Dispose();
			return result;
		}
		public object ExecuteScalar(string sqlCommand)
		{
			return this.ExecuteScalar(sqlCommand, this.CreateConnection());
		}
		public object ExecuteScalar(string sqlCommand, object connection)
		{
			OleDbConnection oleDbConnection = (OleDbConnection)connection;
			oleDbConnection.Open();
			OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
			oleDbCommand.CommandText = sqlCommand;
			object result = oleDbCommand.ExecuteScalar();
			oleDbCommand.Dispose();
			oleDbConnection.Close();
			oleDbConnection.Dispose();
			return result;
		}
		public OleDbDataReader ExecuteReader(string sqlCommand, OleDbConnection connection)
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
			OleDbCommand oleDbCommand = connection.CreateCommand();
			oleDbCommand.CommandText = sqlCommand;
			OleDbDataReader result = oleDbCommand.ExecuteReader();
			oleDbCommand.Dispose();
			return result;
		}
		public DataTable ExecuteTable(string sqlCommand)
		{
			return this.ExecuteTable(sqlCommand, this.CreateConnection());
		}
		public DataTable ExecuteTable(string sqlCommand, object con)
		{
			OleDbConnection oleDbConnection = (OleDbConnection)con;
			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(sqlCommand, oleDbConnection);
			DataTable dataTable = new DataTable();
			oleDbDataAdapter.Fill(dataTable);
			oleDbDataAdapter.Dispose();
			oleDbConnection.Close();
			oleDbConnection.Dispose();
			return dataTable;
		}
		public int ExecuteNumber(string sqlCommand)
		{
			return this.ExecuteNumber(sqlCommand, this.CreateConnection());
		}
		public int ExecuteNumber(string sqlCommand, object connection)
		{
			object obj = this.ExecuteScalar(sqlCommand, connection);
			return (obj == null) ? -1 : int.Parse(obj.ToString());
		}
		public OleDbConnection CreateOleDbConnection()
		{
			return (OleDbConnection)this.CreateConnection();
		}
		public OleDbConnection CreateOleDbConnection(string conStr)
		{
			return new OleDbConnection((conStr.IndexOf("Microsoft.Jet.OLEDB") == -1) ? ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + conStr + "\"") : conStr);
		}
		public object CreateConnection()
		{
			return (this.selCon == null) ? this.CreateConnection(ConfigManager.ConfigurationData.ConnectionString) : this.selCon;
		}
		public object CreateConnection(string constr)
		{
			return this.CreateOleDbConnection(constr);
		}
		public void ClearSchema()
		{
			if (DBHelper._schema != null)
			{
				DBHelper._schema.Dispose();
				DBHelper._schema = null;
			}
		}
		public DataTable GetSchema(SchemaInfoType infoType)
		{
			if (DBHelper._schema == null)
			{
				DBHelper._schema = new DataSet("RuntimeSchemaFormAutumn");
				OleDbConnection oleDbConnection = this.CreateOleDbConnection();
				oleDbConnection.Open();
				DBHelper._schema.Tables.AddRange(new DataTable[]
				{
					oleDbConnection.GetSchema("MetaDataCollections"),
					oleDbConnection.GetSchema("DataSourceInformation"),
					oleDbConnection.GetSchema("DataTypes"),
					oleDbConnection.GetSchema("Restrictions"),
					oleDbConnection.GetSchema("ReservedWords"),
					oleDbConnection.GetSchema("Columns"),
					oleDbConnection.GetSchema("Indexes"),
					oleDbConnection.GetSchema("Tables")
				});
				oleDbConnection.Close();
			}
			return DBHelper._schema.Tables.Contains(infoType.ToString()) ? DBHelper._schema.Tables[infoType.ToString()] : null;
		}
	}
}

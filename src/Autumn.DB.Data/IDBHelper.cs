using System;
using System.Collections.Generic;
using System.Data;

namespace Autumn.DB.Data
{
	public interface IDBHelper
	{
        string DBObjectPrefix { get; }
        string DBObjectSuffix { get; }

		Dictionary<string, string> DBTypeMapping{get;}
		object CreateConnection();
		object CreateConnection(string constr);
		DataTable GetSchema(SchemaInfoType infoType);
		void ClearSchema();
		int ExecuteNonQuery(string sqlCommand);
		int ExecuteNonQuery(string sqlCommand, object connection);
		object ExecuteScalar(string sqlCommand);
		object ExecuteScalar(string sqlCommand, object connection);
		DataTable ExecuteTable(string sqlCommand);
		DataTable ExecuteTable(string sqlCommand, object connection);
		int ExecuteNumber(string sqlCommand);
		int ExecuteNumber(string sqlCommand, object connection);
	}
}

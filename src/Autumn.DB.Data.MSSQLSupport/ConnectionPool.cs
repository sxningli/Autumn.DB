using Autumn.DB.Config;
using System;
using System.Data.SqlClient;
namespace Autumn.DB.Data.MSSQLSupport
{
	public class ConnectionPool
	{
		private static string connectionString = null;
		private static string ConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(ConnectionPool.connectionString))
				{
					ConnectionPool.connectionString = ConfigManager.ConfigurationData.ConnectionString;
				}
				return ConnectionPool.connectionString;
			}
		}
		public static void ClearInfo()
		{
			ConnectionPool.connectionString = null;
			ConnectionPool.ClearAllPool();
		}
		public static SqlConnection CreateConnection()
		{
			return ConnectionPool.CreateConnection(ConnectionPool.ConnectionString);
		}
		public static SqlConnection CreateConnection(string connectionString)
		{
			return new SqlConnection(connectionString);
		}
		public static void ClearPool(SqlConnection connection)
		{
			SqlConnection.ClearPool(connection);
		}
		public static void ClearAllPool()
		{
			SqlConnection.ClearAllPools();
		}
	}
}

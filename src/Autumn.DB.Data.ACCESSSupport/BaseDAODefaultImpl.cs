using System;
using System.Data.OleDb;
using System.Reflection;
namespace Autumn.DB.Data.ACCESSSupport
{
	public class BaseDAODefaultImpl<T> : AbstractImpl<T>
	{
		public override bool Insert(T entity)
		{
			OleDbConnection oleDbConnection = new DBHelper().CreateOleDbConnection();
			oleDbConnection.Open();
			OleDbCommand oleDbCommand = new OleDbCommand(base.BuildInsertCommandBase(entity), oleDbConnection);
			bool result;
			if (oleDbCommand.ExecuteNonQuery() > 0)
			{
				PropertyInfo propertyInfo = base.GetGenericsPropertyInfoMapping()["id"];
				oleDbCommand.CommandText = "select @@IDENTITY";
				try
				{
					propertyInfo.SetValue(entity, int.Parse(oleDbCommand.ExecuteScalar().ToString()), null);
				}
				catch
				{
				}
				result = true;
			}
			else
			{
				try
				{
					oleDbCommand.Dispose();
				}
				catch
				{
				}
				try
				{
					oleDbConnection.Close();
					oleDbConnection.Dispose();
				}
				catch
				{
				}
				result = false;
			}
			return result;
		}
	}
}

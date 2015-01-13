using MySql.Data.MySqlClient;
using System;
using System.Reflection;
namespace Autumn.DB.Data.MYSQLSupport
{
	public class BaseDAODefaultImpl<T> : AbstractImpl<T>
	{
		public override bool Insert(T entity)
		{
			MySqlConnection mySqlConnection = new DBHelper().CreateMySqlConnection();
			mySqlConnection.Open();
			MySqlCommand mySqlCommand = new MySqlCommand(base.BuildInsertCommandBase(entity), mySqlConnection);
			bool result;
            if (mySqlCommand.ExecuteNonQuery() > 0 && base.GetGenericsPropertyInfoMapping().ContainsKey("id"))
			{
				PropertyInfo propertyInfo = base.GetGenericsPropertyInfoMapping()["id"];
				mySqlCommand.CommandText = "select @@IDENTITY";
				try
				{
					propertyInfo.SetValue(entity, int.Parse(mySqlCommand.ExecuteScalar().ToString()), null);
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
					mySqlCommand.Dispose();
				}
				catch
				{
				}
				try
				{
					mySqlConnection.Close();
					mySqlConnection.Dispose();
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

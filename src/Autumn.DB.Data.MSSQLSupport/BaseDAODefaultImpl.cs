using Autumn.DB.Factory;
using System;
using System.Reflection;
using System.Text;
namespace Autumn.DB.Data.MSSQLSupport
{
	public class BaseDAODefaultImpl<T> : AbstractImpl<T>
	{
		public override bool Insert(T entity)
		{
			StringBuilder stringBuilder = new StringBuilder(base.BuildInsertCommandBase(entity));

            if (base.GetGenericsPropertyInfoMapping().ContainsKey("id"))
            {
                stringBuilder.Append(";select @@IDENTITY");

                PropertyInfo propertyInfo = base.GetGenericsPropertyInfoMapping()["id"];
                bool result;
                try
                {
                    propertyInfo.SetValue(entity, DBHelperFactory.CreateCommonDBHelper().ExecuteNumber(stringBuilder.ToString()), null);
                    stringBuilder.Length = 0;
                    result = (propertyInfo.GetValue(entity, null) is int);
                }
                catch
                {
                    result = false;
                }
                return result;
            } else {
                return DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString()) > 0;
            }
		}
	}
}

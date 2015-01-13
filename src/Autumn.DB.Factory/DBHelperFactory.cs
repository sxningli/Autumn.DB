using System;
using System.Text;

using Autumn.DB.Data;

namespace Autumn.DB.Factory
{
	public class DBHelperFactory
	{
		private static Type _type = null;
		private static Type type
		{
			get
			{
				if (DBHelperFactory._type == null)
				{
					DBHelperFactory._type = Type.GetType(Common.assemblyName + ".DBHelper");
				}
				return DBHelperFactory._type;
			}
		}
		public static IDBHelper CreateCommonDBHelper()
		{
			return Activator.CreateInstance(DBHelperFactory.type) as IDBHelper;
		}
		public static IDBHelper CreateDBHelper(DBType dbtype, string constr)
		{
			StringBuilder stringBuilder = new StringBuilder("Autumn.DB.Data.");
			stringBuilder.Append(dbtype.ToString().ToUpper()).Append("Support.DBHelper");
			return Activator.CreateInstance(Type.GetType(stringBuilder.ToString()), new object[]
			{
				constr
			}) as IDBHelper;
		}
	}
}

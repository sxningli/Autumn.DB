using System;
using System.Text;

using Autumn.DB.Config;

namespace Autumn.DB.Factory
{
	internal class Common
	{
		private static string _assemblyName = null;
		public static string assemblyName
		{
			get
			{
				if (Common._assemblyName == null)
				{
					StringBuilder stringBuilder = new StringBuilder("Autumn.DB.Data.").Append(ConfigManager.ConfigurationData.DataBase.ToUpper()).Append("Support");
					Common._assemblyName = stringBuilder.ToString();
					stringBuilder.Length = 0;
				}
				return Common._assemblyName;
			}
		}
	}
}

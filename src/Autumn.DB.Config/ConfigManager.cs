using Autumn.DB.Data;
using Autumn.DB.Factory;
using System;
using System.IO;
using System.Web;
using System.Xml;
namespace Autumn.DB.Config
{
	public class ConfigManager
	{
		public class ConfigurationData
		{
			private static string dataBase = null;
			private static string connectionString = null;
			public static string DataBase
			{
				get
				{
					if (ConfigManager.isLoad)
					{
						return ConfigManager.ConfigurationData.dataBase;
					}
					throw new ConfigurationNotLoadException();
				}
				set
				{
					if (ConfigManager.isLoad)
					{
						throw new ConfigurationDoNotModifyException();
					}
					ConfigManager.ConfigurationData.dataBase = value;
				}
			}
			public static string ConnectionString
			{
				get
				{
					if (ConfigManager.isLoad)
					{
						return ConfigManager.ConfigurationData.connectionString;
					}
					throw new ConfigurationNotLoadException();
				}
				set
				{
					if (ConfigManager.isLoad)
					{
						throw new ConfigurationDoNotModifyException();
					}
					ConfigManager.ConfigurationData.connectionString = value;
				}
			}
		}
		private static string _mapPath = null;
		private static string _path = null;
		private static string _xmlConfig = null;
		public static bool isLoad = false;
		public static string mapPath
		{
			get
			{
				try
				{
					if (ConfigManager._mapPath == null)
					{
						ConfigManager._mapPath = HttpContext.Current.Server.MapPath("~");
					}
				}
				catch
				{
				}
				return ConfigManager._mapPath;
			}
		}
		public static void LoaderConfiguration()
		{
			if (!ConfigManager.isLoad)
			{
				XmlDocument xmlDocument = new XmlDocument();
				if (ConfigManager._xmlConfig == null)
				{
					xmlDocument.Load((!string.IsNullOrEmpty(ConfigManager._path) && File.Exists(ConfigManager._path)) ? ConfigManager._path : (ConfigManager.mapPath + "/bin/autumn-config.xml"));
				}
				else
				{
					xmlDocument.LoadXml(ConfigManager._xmlConfig);
				}
				foreach (XmlNode xmlNode in xmlDocument.LastChild["database"])
				{
					if (xmlNode.Name == "type")
					{
						ConfigManager.ConfigurationData.DataBase = xmlNode.InnerText.Trim().ToUpper();
					}
					else
					{
						if (xmlNode.Name == "connection-string")
						{
							ConfigManager.ConfigurationData.ConnectionString = xmlNode.InnerXml.Trim();
						}
					}
				}
				ConfigManager.isLoad = true;
			}
		}
		public static void LoaderConfiguration(string path)
		{
			if (ConfigManager.mapPath != null)
			{
				ConfigManager._path = ((path.IndexOf("/") == 0) ? (ConfigManager.mapPath + path) : ((path.IndexOf(":") == -1) ? (ConfigManager.mapPath + "/" + path) : path));
			}
			else
			{
				ConfigManager._path = path;
			}
			ConfigManager.LoaderConfiguration();
		}
		public static void LoaderConfigurationXml(string xmlConfigContent)
		{
			ConfigManager._xmlConfig = xmlConfigContent;
			ConfigManager.LoaderConfiguration();
		}

        public static DBType? GetDBType(string dbType) {
            foreach(string name in Enum.GetNames(typeof(DBType))){
                if (dbType.ToLower() == name.ToLower()) { 
                    return Enum.Parse(typeof(DBType),name) as DBType?;
                }
            }

            return null;
        }

        public static void LoaderConfiguration(string dbType, string connectionString) { LoaderConfiguration(GetDBType(dbType).Value, connectionString); }
		public static void LoaderConfiguration(DBType dbtype, string connectionString)
		{
			ConfigManager.LoaderConfigurationXml(string.Format("<?xml version='1.0' encoding='utf-8' ?><autumn-config><database><type>{0}</type><connection-string>{1}</connection-string></database></autumn-config>", dbtype.ToString(), connectionString));
		}
		public static void ReLoaderConfiguration()
		{
			if (ConfigManager.isLoad)
			{
				ConfigManager.isLoad = false;
				ConfigManager.LoaderConfiguration();
				try
				{
					DBHelperFactory.CreateCommonDBHelper().ClearSchema();
				}
				catch
				{
				}
			}
		}
	}
}

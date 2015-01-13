using System;
namespace Autumn.DB.Config
{
	public class ConfigurationNotLoadException : Exception
	{
		private const string message = "配置信息尚未载入，请先读取配置文件信息。";
		public override string ToString()
		{
			return "配置信息尚未载入，请先读取配置文件信息。";
		}
	}
}

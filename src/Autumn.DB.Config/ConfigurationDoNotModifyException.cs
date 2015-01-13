using System;

namespace Autumn.DB.Config
{
	public class ConfigurationDoNotModifyException : Exception
	{
		private const string message = "系统运行过程中不允许修改系统信息，如果需要请重新加载配置文件。";
		public override string ToString()
		{
			return "系统运行过程中不允许修改系统信息，如果需要请重新加载配置文件。";
		}
	}
}

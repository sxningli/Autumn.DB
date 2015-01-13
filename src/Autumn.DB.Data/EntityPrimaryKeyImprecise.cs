using System;
namespace Autumn.DB.Data
{
	public class EntityPrimaryKeyImprecise : Exception
	{
		private const string message = "要操作的实体类对象ID不明确，运行中断。";
		public override string ToString()
		{
            return message;
		}
	}
}

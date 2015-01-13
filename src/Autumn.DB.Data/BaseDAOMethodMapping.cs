using System;

namespace Autumn.DB.Data
{
	public class BaseDAOMethodMapping
	{
		private object dao = null;
		public BaseDAOMethodMapping(object dao)
		{
			this.dao = dao;
		}
		public bool Insert(object entity)
		{
			return Convert.ToBoolean(this.dao.GetType().GetMethod("Insert").Invoke(this.dao, new object[]
			{
				entity
			}));
		}
		public bool Delete(int id)
		{
			return Convert.ToBoolean(this.dao.GetType().GetMethod("Delete").Invoke(this.dao, new object[]
			{
				id
			}));
		}
		public bool Update(object entity)
		{
			return Convert.ToBoolean(this.dao.GetType().GetMethod("Update").Invoke(this.dao, new object[]
			{
				entity
			}));
		}
		public object SelectById(int id)
		{
			return this.dao.GetType().GetMethod("SelectById").Invoke(this.dao, new object[]
			{
				id
			});
		}
		public object SelectAll()
		{
			return this.dao.GetType().GetMethod("SelectAll").Invoke(this.dao, null);
		}
		public object SelectByDynamic(string whereCondition)
		{
			return this.dao.GetType().GetMethod("SelectByDynamic").Invoke(this.dao, new object[]
			{
				whereCondition
			});
		}
	}
}

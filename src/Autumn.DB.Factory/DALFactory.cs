using Autumn.DB.Data;
using Autumn.DB.IDAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autumn.DB.Factory
{
	public abstract class DALFactory
	{
        /// <summary>
        /// 表前缀
        /// </summary>
        public static string TablePrefix = null;

        /// <summary>
        /// 表名处理程序
        /// </summary>
        public static TableNameProcess TableNameProcess = null;

        /// <summary>
        /// 表名映射
        /// </summary>
        public static Dictionary<string, string> Class2TableNameMapping = new Dictionary<string, string>();

		internal static Type GetBaseDaoDefaultImplByType(Type type)
		{
			Type type2 = Type.GetType(new StringBuilder(Common.assemblyName).Append(".BaseDAODefaultImpl`1[[").Append(type.FullName).Append(",").Append(type.Namespace).Append(", Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]").ToString());
			if (type2 == null)
			{
				type2 = Type.GetType(new StringBuilder(Common.assemblyName).Append(".BaseDAODefaultImpl`1[[").Append(type.FullName).Append(", App_Code.wfjtmzos, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]").ToString());
			}
			if (type2 == null)
			{
				string typeName = new StringBuilder(Common.assemblyName).Append(".BaseDAODefaultImpl`1[[").Append(type.AssemblyQualifiedName).Append("]], Autumn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null").ToString();
				type2 = Type.GetType(typeName);
			}
			return type2;
		}
		public static BaseDAOMethodMapping CreateCommonDao(Type type)
		{
			return new BaseDAOMethodMapping(Activator.CreateInstance(DALFactory.GetBaseDaoDefaultImplByType(type)));
		}
	}
	public class DALFactory<T>
	{
        #region SelectOne
        /// <summary>
        /// 动态查询单个记录
        /// </summary>
        public static T SelectOneByDynamic(string whereCondition)
        {
            IList<T> result = DALFactory<T>.SelectByDynamic(whereCondition);
            return result == null ? default(T) : result[0];
        }

        /// <summary>
        /// 根据模板查询单个记录
        /// </summary>
        public static T SelectOneByTemplate(T entity)
        {
            IList<T> result = DALFactory<T>.SelectByTemplate(entity);
            return result == null ? default(T) : result[0];
        }

        /// <summary>
        /// 根据模板查询单个记录
        /// </summary>
        public static T SelectOneByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin)
        {
            IList<T> result = DALFactory<T>.SelectByTemplate(entity, compareLogic, conditionLogin);
            return result == null ? default(T) : result[0];
        }

        /// <summary>
        /// 根据 TSQL 语句查询单个记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T SelectOneByTSQL(string sql)
        {
            IList<T> result =  DALFactory<T>.SelectByTSQL(sql);
            return result == null ? default(T) : result[0];
        }
        #endregion

        #region SelectExist
        /// <summary>
        /// 动态查询指定条件是否有数据存在
        /// </summary>
        public static bool SelectExistByDynamic(string whereCondition)
        {
            return DALFactory<T>.SelectByDynamic(whereCondition) != null;
        }

        /// <summary>
        /// 根据模板查询指定模板是否有数据存在
        /// </summary>
        public static bool SelectExistByTemplate(T entity)
        {
            return DALFactory<T>.SelectByTemplate(entity) != null;
        }

        /// <summary>
        /// 根据模板查询指定模板是否有数据存在
        /// </summary>
        public static bool SelectExistByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin)
        {
            return DALFactory<T>.SelectByTemplate(entity, compareLogic, conditionLogin) != null;
        }

        /// <summary>
        /// 根据 TSQL 查询是否有数据存在
        /// </summary>
        public static bool SelectExistByTSQL(string sql) {
            return DALFactory<T>.SelectByTSQL(sql) != null;
        }
        #endregion

		public static IBaseDAO<T> CreateCommonDao()
		{
			return Activator.CreateInstance(DALFactory.GetBaseDaoDefaultImplByType(typeof(T))) as IBaseDAO<T>;
		}
		public static bool Insert(T entity)
		{
			return DALFactory<T>.CreateCommonDao().Insert(entity);
		}
		public static bool Delete(int? id)
		{
			return DALFactory<T>.CreateCommonDao().Delete(id);
		}
        public static int DeleteByIds(params string[] ids) {
            return DALFactory<T>.CreateCommonDao().DeleteByIds(ids);
        }
        public static int DeleteByIds(params int[] ids) {
            return DALFactory<T>.CreateCommonDao().DeleteByIds(ids);
        }
        public static int DeleteByDynamic(string whereCondition) {
            return DALFactory<T>.CreateCommonDao().DeleteByDynamic(whereCondition);
        }
		public static int DeleteByTemplate(T entity)
		{
			return DALFactory<T>.CreateCommonDao().DeleteByTemplate(entity);
		}
		public static int DeleteByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin)
		{
			return DALFactory<T>.CreateCommonDao().DeleteByTemplate(entity, compareLogic, conditionLogin);
		}
        public static int DeleteByField(string field, object value) { return DALFactory<T>.CreateCommonDao().DeleteByField(field, value); }
        public static bool Update(T entity)
		{
			return DALFactory<T>.CreateCommonDao().Update(entity);
		}
        public static bool UpdateByField(T entity, string field, object value) { return DALFactory<T>.CreateCommonDao().UpdateByField(entity,field, value); }
		public static T SelectById(int? id)
		{
			return DALFactory<T>.CreateCommonDao().SelectById(id);
		}
        public static IList<T> SelectByField(string field, object value) { return DALFactory<T>.CreateCommonDao().SelectByField(field, value); }
        public static T SelectOneByField(string field, object value) { return DALFactory<T>.CreateCommonDao().SelectOneByField(field, value); }
		public static IList<T> SelectByTemplate(T entity)
		{
			return DALFactory<T>.CreateCommonDao().SelectByTemplate(entity);
		}
		public static IList<T> SelectByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin)
		{
			return DALFactory<T>.CreateCommonDao().SelectByTemplate(entity, compareLogic, conditionLogin);
		}
		public static IList<T> SelectAll()
		{
			return DALFactory<T>.CreateCommonDao().SelectAll();
		}
        public static IList<T> SelectAll(int pageSize, int pageNo) {
            return DALFactory<T>.CreateCommonDao().SelectAll(pageSize, pageNo);
        }
        public static IList<T> SelectAll(int pageSize, int pageNo, string appendQuery) {
            return DALFactory<T>.CreateCommonDao().SelectAll(pageSize, pageNo, appendQuery);
        }
        public static IList<T> SelectAll(string fields, int pageSize, int pageNo, string appendQuery) {
            return DALFactory<T>.CreateCommonDao().SelectAll(fields, pageSize, pageNo, appendQuery);
        }
        public static int SelectAllCount() {
            return DALFactory<T>.SelectAllCount();
        }
        public static int SelectAllCount(string whereCondition) {
            return DALFactory<T>.SelectAllCount(whereCondition);
        }
        public static IList<T> SelectByDynamic(string whereCondition)
		{
			return DALFactory<T>.CreateCommonDao().SelectByDynamic(whereCondition);
		}
		public static IList<T> SelectByTSQL(string sqlCommand)
		{
			return DALFactory<T>.CreateCommonDao().SelectByTSQL(sqlCommand);
		}
	}
}

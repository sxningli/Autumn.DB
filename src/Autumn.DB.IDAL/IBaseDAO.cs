using System;
using System.Collections.Generic;

namespace Autumn.DB.IDAL
{
	public interface IBaseDAO<T>
	{
		bool Insert(T entity);
		bool Delete(int? id);
        int DeleteByIds(params string[] ids);
        int DeleteByIds(params int[] ids);
        int DeleteByDynamic(string whereCondition);
        int DeleteByField(string field, object value);
		int DeleteByTemplate(T entity);
		int DeleteByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin);
		bool Update(T entity);
        bool UpdateByField(T entity, string field, object value);
		T SelectById(int? id);
        IList<T> SelectByField(string field, object value);
        T SelectOneByField(string field, object value);
		IList<T> SelectByTemplate(T entity);
        IList<T> SelectByTemplate(T entity, params string[] columnNames);
        IList<T> SelectByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin, params string[] columnNames);
		IList<T> SelectAll();
        IList<T> SelectAll(int pageSize, int pageNo);
        IList<T> SelectAll(int pageSize, int pageNo, string appendQuery);
        IList<T> SelectAll(string fields, int pageSize, int pageNo, string appendQuery);
        int SelectAllCount();
        int SelectAllCount(string whereCondition);
		IList<T> SelectByDynamic(string whereCondition);
		IList<T> SelectByTSQL(string sqlCommand);
	}
}

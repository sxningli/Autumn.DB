using Autumn.DB.Config;
using Autumn.DB.Factory;
using Autumn.DB.IDAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace Autumn.DB.Data
{
    /// <summary>
    /// 表名处理程序
    /// </summary>
    public delegate string TableNameProcess(Type type);

	public abstract class AbstractImpl<T> : IBaseDAO<T>
	{
		private Type _type = null;
		private PropertyInfo[] _propertyInfo = null;
		private Dictionary<string, PropertyInfo> mapping = null;
		private Dictionary<string, PropertyInfo> customMapping = new Dictionary<string, PropertyInfo>();
		protected Dictionary<string, PropertyInfo> GetGenericsPropertyInfoMapping()
		{
			if (this.mapping == null)
			{
				this.mapping = new Dictionary<string, PropertyInfo>();
				PropertyInfo[] genericsProperties = this.GetGenericsProperties();
				for (int i = 0; i < genericsProperties.Length; i++)
				{
					PropertyInfo propertyInfo = genericsProperties[i];
					if (propertyInfo.IsDefined(typeof(CustomType), false))
					{
						this.customMapping.Add(propertyInfo.Name.ToLower(), propertyInfo);
					}
					else
					{
						this.mapping.Add(propertyInfo.Name.ToLower(), propertyInfo);
					}
				}
			}
			return this.mapping;
		}
		protected T CreateInstance()
		{
			return (T)Activator.CreateInstance(this.GetGenericsType());
		}
		protected PropertyInfo[] GetGenericsProperties()
		{
			if (this._propertyInfo == null)
			{
				this._propertyInfo = this.GetGenericsType().GetProperties();
			}
			return this._propertyInfo;
		}
		protected Type GetGenericsType()
		{
			if (this._type == null)
			{
				this._type = typeof(T);
			}
			return this._type;
		}
		protected bool IsEntityLink(PropertyInfo info)
		{
			return info.PropertyType.GetProperty("Id") != null;
		}
		protected bool IsCompositeProperty(PropertyInfo info)
		{
            return info is IList;
			//return this.IsEntityLink(info) || info is IList;
		}
		protected T FillEntiry(DataRow row)
		{
			T t = this.CreateInstance();
			foreach (DataColumn dataColumn in row.Table.Columns)
			{
				PropertyInfo propertyInfo = null;
				if (this.GetGenericsPropertyInfoMapping().ContainsKey(dataColumn.ColumnName.ToLower()))
				{
					propertyInfo = this.GetGenericsPropertyInfoMapping()[dataColumn.ColumnName.ToLower()];
				}
				else
				{
					if (this.customMapping.ContainsKey(dataColumn.ColumnName.ToLower()))
					{
						propertyInfo = this.customMapping[dataColumn.ColumnName.ToLower()];
					}
				}
				if (propertyInfo != null)
				{
					object obj = row[dataColumn];
					if (obj is DBNull)
					{
						propertyInfo.SetValue(t, null, null);
					}
					else
					{
						try
						{
							if (this.IsEntityLink(propertyInfo))
							{
								propertyInfo.SetValue(t, DALFactory.CreateCommonDao(propertyInfo.PropertyType).SelectById(int.Parse(obj.ToString())), null);
							}
							else
							{
                                if (propertyInfo.PropertyType == typeof(bool)) {
                                    if (!(obj is bool)) {
                                        propertyInfo.SetValue(t, int.Parse(obj.ToString())==1, null);

                                        continue;
                                    }
                                }

                                propertyInfo.SetValue(t, obj, null);
							}
						}
						catch
						{
						}
					}
				}
			}
			PropertyInfo[] genericsProperties = this.GetGenericsProperties();
			for (int i = 0; i < genericsProperties.Length; i++)
			{
				PropertyInfo propertyInfo = genericsProperties[i];
				if (propertyInfo is IList)
				{
					Type type = propertyInfo.GetType();
					int num = type.FullName.IndexOf("[");
					try
					{
						string typeName = type.FullName.Substring(++num, type.FullName.Length - (num + 1));
						propertyInfo.SetValue(t, DALFactory.CreateCommonDao(Type.GetType(typeName)).SelectByDynamic(propertyInfo.Name + "=" + this.GetGenericsPropertyInfoMapping()["id"].GetValue(t, null)), null);
					}
					catch
					{
					}
				}
			}
			return t;
		}
		internal string BuildInsertCommandBase(T entity)
		{
			StringBuilder stringBuilder = new StringBuilder("insert into ").Append(this.CName(this.GetGenericsType(),true)).Append("(");
			PropertyInfo[] genericsProperties = this.GetGenericsProperties();
			for (int i = 0; i < genericsProperties.Length; i++)
			{
				PropertyInfo propertyInfo = genericsProperties[i];
				if (propertyInfo.Name.ToLower() != "id" && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()) && !this.IsCompositeProperty(propertyInfo))
				{
					stringBuilder.Append(this.CName(propertyInfo.Name)).Append(",");
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1).Append(") values(");
			genericsProperties = this.GetGenericsProperties();
			for (int i = 0; i < genericsProperties.Length; i++)
			{
				PropertyInfo propertyInfo = genericsProperties[i];
				if (propertyInfo.Name.ToLower() != "id" && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()) && !this.IsCompositeProperty(propertyInfo))
				{
                    PropertyInfo p = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()];

					object value = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()].GetValue(entity, null);
					if (value == null || (value is DateTime && DateTime.Parse(value.ToString()).Year == 1)) {
						stringBuilder.Append("null,");
					} else {
                        if (IsEntityLink(p)) {
                            stringBuilder.Append(p.PropertyType.GetProperty("Id").GetValue(value, null)).Append(",");
                        }else if (value is bool) {
                            stringBuilder.Append(value).Append(",");
                        } else {
                            stringBuilder.Append("'").Append(SQLSecurity.SecurityParam( value.ToString() )).Append("',");
                        }
					}
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1).Append(")");
			return stringBuilder.ToString();
		}
        internal string CName(string key) { return this.CName(key, null, false); }
        internal string CName(Type type, bool intelligent) { return this.CName(null, type, intelligent); }
		internal string CName(string key, Type type , bool intelligent)
		{
            if (string.IsNullOrEmpty(key)) {
                key = type.Name;
            }

			StringBuilder stringBuilder = new StringBuilder();
			string value2;
			string value;
			if (ConfigManager.ConfigurationData.DataBase == "MYSQL")
			{
				value = (value2 = "`");
			}
			else
			{
				value2 = "[";
				value = "]";
			}

            stringBuilder.Append(value2);

            if (intelligent)
            {
                if (DALFactory.Class2TableNameMapping.ContainsKey(key))
                {
                    stringBuilder.Append(DALFactory.Class2TableNameMapping[key]);
                }
                else if (DALFactory.TableNameProcess != null)
                {
                    stringBuilder.Append(DALFactory.TableNameProcess(type));
                }
                else
                {
                    if (DALFactory.TablePrefix != null && !key.ToLower().StartsWith(DALFactory.TablePrefix.ToLower()))
                    {
                        stringBuilder.Append(DALFactory.TablePrefix);
                    }

                    stringBuilder.Append(key);
                }
            } else {
                stringBuilder.Append(key);
            }

            return stringBuilder.Append(value).ToString();
		}
        protected string DBValue(object value) {
            if (value == null || (value is DateTime && DateTime.Parse(value.ToString()).Year == 1)) {
                return "null";
            } else if (value is int || value is long || value is double || value is decimal || value is float || value is short) {
                return value.ToString();
            } else if (value is bool) {
                return (bool)value ? "true" : "false";
            } else {
                return new StringBuilder("'").Append(SQLSecurity.SecurityParam(value.ToString())).Append("'").ToString();
            }
        }
        public virtual bool Insert(T entity)
		{
			return DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(this.BuildInsertCommandBase(entity)) > 0;
		}
		public virtual bool Delete(int? id)
		{
			StringBuilder stringBuilder = new StringBuilder("delete from ").Append(this.CName(this.GetGenericsType(),true)).Append(" where id=").Append(id);
			bool result = DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString()) == 1;
			stringBuilder.Length = 0;
			return result;
		}
        public virtual int DeleteByIds(params string[] ids) {
            if(ids==null || ids.Length ==0){ return 0;}

            List<int> list = new List<int>();

            int id;

            foreach (string idStr in ids) {
                if (int.TryParse(idStr, out id)) {
                    list.Add(id);
                }
            }

            return this.DeleteByIds(list.ToArray());
        }
        public virtual int DeleteByIds(params int[] ids) {
            if (ids == null || ids.Length == 0) { return 0; }

            StringBuilder buffer = new StringBuilder();

            foreach (int id in ids) {
                buffer.Append(" or id=").Append(id);
            }

            buffer.Remove(0, 4);

            return DeleteByDynamic(buffer.ToString());
        }
        public virtual int DeleteByDynamic(string whereCondition) {
            StringBuilder stringBuilder = new StringBuilder("delete from ").Append(this.CName(this.GetGenericsType(), true)).Append(" where ").Append(whereCondition);

            int result = DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString());

            stringBuilder.Length = 0;

            return result;
        }
        public virtual int DeleteByField(string field, object value) {
            StringBuilder conditionBuffer = new StringBuilder(CName(field)).Append("=");

            if (value == null || (value is DateTime && DateTime.Parse(value.ToString()).Year == 1))
            {
                conditionBuffer.Remove(conditionBuffer.Length - 1, 1).Append(" is null");
            } else {
                conditionBuffer.Append(DBValue(value));
            }

            return this.DeleteByDynamic(conditionBuffer.ToString());
        }
        public virtual int DeleteByTemplate(T entity)
		{
			return this.DeleteByTemplate(entity, CompareLogic.EQUALS, ConditionLogic.AND);
		}
		public virtual int DeleteByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin)
		{
			StringBuilder stringBuilder = new StringBuilder("delete from ").Append(this.CName(this.GetGenericsType(),true)).Append(" where ");
			bool flag = false;
			PropertyInfo[] genericsProperties = this.GetGenericsProperties();
			for (int i = 0; i < genericsProperties.Length; i++)
			{
				PropertyInfo propertyInfo = genericsProperties[i];
				if (!this.IsCompositeProperty(propertyInfo) && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()))
				{
					object value = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()].GetValue(entity, null);
					if ((value != null && !(value is DateTime)) || (value is DateTime && DateTime.Parse(value.ToString()).Year != 1))
					{
						flag = true;
						stringBuilder.Append(this.CName(propertyInfo.Name)).Append((compareLogic == CompareLogic.EQUALS) ? "=" : "<>").Append("'").Append(SQLSecurity.SecurityParam( value.ToString() )).Append("' ").Append((conditionLogin == ConditionLogic.AND) ? "and " : "or ");
					}
				}
			}
			stringBuilder.Length -= (flag ? ((conditionLogin == ConditionLogic.AND) ? 5 : 4) : 6);
			int result = DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString());
			stringBuilder.Length = 0;
			return result;
		}
		public virtual bool Update(T entity)
		{
            object value = this.GetGenericsPropertyInfoMapping()["id"].GetValue(entity, null);
            if (value == null)
            {
                throw new EntityPrimaryKeyImprecise();
            }

            return this.UpdateByField(entity, "id", value);
            /*
			object value = this.GetGenericsPropertyInfoMapping()["id"].GetValue(entity, null);
			if (value == null)
			{
				throw new EntityPrimaryKeyImprecise();
			}
			StringBuilder stringBuilder = new StringBuilder("update ").Append(this.CName(this.GetGenericsType(),true)).Append(" set ");
			PropertyInfo[] genericsProperties = this.GetGenericsProperties();
			for (int i = 0; i < genericsProperties.Length; i++)
			{
				PropertyInfo propertyInfo = genericsProperties[i];
				if (propertyInfo.Name.ToLower() != "id" && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()) && !this.IsCompositeProperty(propertyInfo))
				{
					stringBuilder.Append(this.CName(propertyInfo.Name)).Append("=");
                    PropertyInfo v2p = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()];
					object value2 = v2p.GetValue(entity, null);

                    if (value2 == null || (value2 is DateTime && DateTime.Parse(value2.ToString()).Year == 1))
                    {
                        stringBuilder.Append("null,");
                    } else if (IsEntityLink(v2p))
                    {
                        stringBuilder.Append(v2p.PropertyType.GetProperty("Id").GetValue(value2, null)).Append(",");
                    }
                    else if (value2 is bool) {
                        stringBuilder.Append(value2).Append(",");
                    } else {
                        stringBuilder.Append("'").Append(SQLSecurity.SecurityParam( value2.ToString() )).Append("',");
                    }
				}
			}
			stringBuilder.Length--;
			stringBuilder.Append(" where id=").Append(value);
			bool result = DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString()) == 1;
			stringBuilder.Length = 0;
			return result;
             */
		}
        public virtual bool UpdateByField(T entity, string field, object value) {
            /*
            if (value == null) {
                throw new EntityPrimaryKeyImprecise();
            }
             */

            StringBuilder stringBuilder = new StringBuilder("update ").Append(this.CName(this.GetGenericsType(), true)).Append(" set ");
            PropertyInfo[] genericsProperties = this.GetGenericsProperties();
            for (int i = 0; i < genericsProperties.Length; i++)
            {
                PropertyInfo propertyInfo = genericsProperties[i];
                if (propertyInfo.Name.ToLower() != "id" && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()) && !this.IsCompositeProperty(propertyInfo))
                {
                    stringBuilder.Append(this.CName(propertyInfo.Name)).Append("=");
                    PropertyInfo v2p = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()];
                    object value2 = v2p.GetValue(entity, null);

                    if (IsEntityLink(v2p))
                    {
                        stringBuilder.Append(v2p.PropertyType.GetProperty("Id").GetValue(value2, null)).Append(",");
                    }else{
                        stringBuilder.Append(DBValue(value2)).Append(",");
                    }
                }
            }
            stringBuilder.Length--;

            if (field == null || field == "id") {
                stringBuilder.Append(" where id=").Append(value);
            } else {
                if (value == null || (value is DateTime && DateTime.Parse(value.ToString()).Year == 1))
                {
                    stringBuilder.Append(" where ").Append(CName(field)).Append(" is null");
                }
                else
                {
                    stringBuilder.Append(" where ").Append(CName(field)).Append("=").Append(DBValue(value));
                }
            }

            bool result = DBHelperFactory.CreateCommonDBHelper().ExecuteNonQuery(stringBuilder.ToString()) == 1;
            stringBuilder.Length = 0;
            return result;
        }
        public virtual T SelectById(int? id)
		{
			StringBuilder stringBuilder = new StringBuilder("select * from ").Append(this.CName(this.GetGenericsType(),true)).Append(" where id = ").Append(id);
			DataTable dataTable = DBHelperFactory.CreateCommonDBHelper().ExecuteTable(stringBuilder.ToString());
			stringBuilder.Length = 0;
			T result;
			if (dataTable.Rows.Count < 1)
			{
				dataTable.Clear();
				dataTable.Dispose();
				result = default(T);
			}
			else
			{
				T t = this.FillEntiry(dataTable.Rows[0]);
				dataTable.Clear();
				dataTable.Dispose();
				result = t;
			}
			return result;
		}
        public virtual IList<T> SelectByField(string field, object value) {
            StringBuilder conditionBuffer = new StringBuilder(CName(field)).Append("=");

            if (value == null || (value is DateTime && DateTime.Parse(value.ToString()).Year == 1))
            {
                conditionBuffer.Remove(conditionBuffer.Length - 1, 1).Append(" is null");
            }
            else
            {
                conditionBuffer.Append(DBValue(value));
            }

            return this.SelectByDynamic(conditionBuffer.ToString());
        }
        public virtual T SelectOneByField(string field, object value) {
            var list = this.SelectByField(field, value);

            if (list == null || list.Count == 0) { return default(T); }

            return list[0];
        }
        public virtual IList<T> SelectByTemplate(T entity)
		{
			return this.SelectByTemplate(entity, CompareLogic.EQUALS, ConditionLogic.AND);
		}
        public virtual IList<T> SelectByTemplate(T entity, params string[] columnNames) {
            return this.SelectByTemplate(entity, CompareLogic.EQUALS, ConditionLogic.AND,columnNames);
        }
       // public virtual IList<T> SelectByTemplate(T entity, CompareLogic compareLogic, ConditionLogin conditionLogin) { return this.SelectByTemplate(entity, compareLogic, conditionLogin, null); }
        public virtual IList<T> SelectByTemplate(T entity, CompareLogic compareLogic, ConditionLogic conditionLogin, params string[] columnNames)
		{
			StringBuilder stringBuilder = new StringBuilder("select * from ").Append(this.CName(this.GetGenericsType(),true)).Append(" where ");
			bool flag = false;

            if (columnNames == null || columnNames.Length == 0)
            {
                PropertyInfo[] genericsProperties = this.GetGenericsProperties();
                for (int i = 0; i < genericsProperties.Length; i++)
                {
                    PropertyInfo propertyInfo = genericsProperties[i];
                    if (!this.IsCompositeProperty(propertyInfo) && this.GetGenericsPropertyInfoMapping().ContainsKey(propertyInfo.Name.ToLower()))
                    {
                        object value = this.GetGenericsPropertyInfoMapping()[propertyInfo.Name.ToLower()].GetValue(entity, null);
                        if (value != null && (!(value is DateTime) || DateTime.Parse(value.ToString()).Year != 1))
                        {
                            flag = true;
                            stringBuilder.Append(this.CName(propertyInfo.Name)).Append((compareLogic == CompareLogic.EQUALS) ? "=" : "<>").Append("'").Append(SQLSecurity.SecurityParam( value.ToString() )).Append("' ").Append((conditionLogin == ConditionLogic.AND) ? "and " : "or ");
                        }
                    }
                }
            } else {
                flag = true;

                foreach (string propertyName in columnNames)
                {
                    PropertyInfo property = this.GetGenericsPropertyInfoMapping()[propertyName.ToLower()];

                    if (property.CanRead)
                    {
                        object value = this.GetGenericsPropertyInfoMapping()[propertyName.ToLower()].GetValue(entity, null);

                        if (value != null && (!(value is DateTime) || DateTime.Parse(value.ToString()).Year != 1))
                        {
                            stringBuilder.Append(this.CName(propertyName)).Append((compareLogic == CompareLogic.EQUALS) ? "=" : "<>").Append("'").Append(SQLSecurity.SecurityParam(value.ToString())).Append("' ").Append((conditionLogin == ConditionLogic.AND) ? "and " : "or ");
                        }
                    }
                }
            }

			stringBuilder.Length -= (flag ? ((conditionLogin == ConditionLogic.AND) ? 5 : 4) : 6);
			DataTable dataTable = DBHelperFactory.CreateCommonDBHelper().ExecuteTable(stringBuilder.ToString());
			stringBuilder.Length = 0;
			IList<T> result;
			if (dataTable.Rows.Count < 1)
			{
				dataTable.Clear();
				dataTable.Dispose();
				result = null;
			}
			else
			{
				IList<T> list = new List<T>();
				foreach (DataRow row in dataTable.Rows)
				{
					list.Add(this.FillEntiry(row));
				}
				dataTable.Clear();
				dataTable.Dispose();
				result = list;
			}
			return result;
		}
		
        public virtual IList<T> SelectAll() { return this.SelectAll(0, 0); }
        public virtual IList<T> SelectAll(int pageSize, int pageNo) { return this.SelectAll(pageSize, pageNo, null); }
        public virtual IList<T> SelectAll(int pageSize, int pageNo, string appendQuery) { return this.SelectAll(null,pageSize,pageNo,appendQuery); }
        public virtual IList<T> SelectAll(string fields, int pageSize, int pageNo , string appendQuery) {
            StringBuilder stringBuilder = _SelectAllBuilder();

            if (!string.IsNullOrEmpty(fields)) {
                //"select * from ..."
                stringBuilder
                    .Remove(7, 1)  //remove *
                    .Insert(7, fields);
            }

            if (!string.IsNullOrEmpty(appendQuery)){
                stringBuilder.Append(appendQuery);
            }

            this._SelectPaging(stringBuilder, pageSize, pageNo);

            DataTable dataTable = DBHelperFactory.CreateCommonDBHelper().ExecuteTable(stringBuilder.ToString());
            stringBuilder.Length = 0;
            IList<T> result;
            if (dataTable.Rows.Count < 1)
            {
                dataTable.Clear();
                dataTable.Dispose();
                result = null;
            }
            else
            {
                IList<T> list = new List<T>();
                foreach (DataRow row in dataTable.Rows)
                {
                    list.Add(this.FillEntiry(row));
                }
                dataTable.Clear();
                dataTable.Dispose();
                result = list;
            }
            return result;
        }
        public virtual int SelectAllCount() { return this.SelectAllCount(null); }
        public virtual int SelectAllCount(string condition) {
            StringBuilder stringBuilder = _SelectAllBuilder();

            if (!string.IsNullOrEmpty(condition)) {
                stringBuilder.Append(" where ").Append(condition);
            }

            return DBHelperFactory.CreateCommonDBHelper().ExecuteNumber(_SelectAllCount(stringBuilder));
        }
        protected string _SelectAllCount(StringBuilder selectBuilder) {
            return selectBuilder.ToString().Replace("select *", "select count(*)");
        }
        protected StringBuilder _SelectPaging(StringBuilder selectBuilder , int pageSize, int pageNo) {
            if(pageSize>0 && pageNo>0){
                if (ConfigManager.ConfigurationData.DataBase == "MYSQL")
                {
                    int start = (pageNo-1)*pageSize;

                    selectBuilder.Append(" limit ").Append(start).Append(",").Append(pageSize);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return selectBuilder;
        }
        protected StringBuilder _SelectAllBuilder() { return new StringBuilder("select * from ").Append(this.CName(this.GetGenericsType(), true)); }

        public virtual IList<T> SelectByDynamic(string whereCondition)
		{
			StringBuilder stringBuilder = new StringBuilder("select * from ").Append(this.CName(this.GetGenericsType(),true)).Append(" where ").Append(whereCondition);
			DataTable dataTable = DBHelperFactory.CreateCommonDBHelper().ExecuteTable(stringBuilder.ToString());
			stringBuilder.Length = 0;
			IList<T> result;
			if (dataTable.Rows.Count < 1)
			{
				dataTable.Clear();
				dataTable.Dispose();
				result = null;
			}
			else
			{
				IList<T> list = new List<T>();
				foreach (DataRow row in dataTable.Rows)
				{
					list.Add(this.FillEntiry(row));
				}
				dataTable.Clear();
				dataTable.Dispose();
				result = list;
			}
			return result;
		}
		public virtual IList<T> SelectByTSQL(string sqlCommand)
		{
			DataTable dataTable = DBHelperFactory.CreateCommonDBHelper().ExecuteTable(sqlCommand);
			IList<T> result;
			if (dataTable.Rows.Count < 1)
			{
				dataTable.Clear();
				dataTable.Dispose();
				result = null;
			}
			else
			{
				IList<T> list = new List<T>();
				foreach (DataRow row in dataTable.Rows)
				{
					list.Add(this.FillEntiry(row));
				}
				dataTable.Clear();
				dataTable.Dispose();
				result = list;
			}
			return result;
		}
	}
}

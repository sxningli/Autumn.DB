using System.Text.RegularExpressions;

namespace Autumn.DB.Data
{
    /// <summary>
    /// SQL数据安全类
    /// </summary>
    public class SQLSecurity
    {
        /// <summary>
        /// 特殊字符正则（用以匹配SQL内单引号）
        /// </summary>
        protected static Regex SpecialRegex = new Regex("(?<=(^|[^']))'(?=($|[^']))");

        /// <summary>
        /// 将 SQL 语句进行安全化处理
        /// </summary>
        public static string SecurityParam(string sql)
        {
            if (sql == null) { return null; }

            //将单引号替换为双引号
            return SpecialRegex.Replace(sql, "''");
        }
    }
}

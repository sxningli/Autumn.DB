using Autumn.DB.Data.ACCESSSupport;
using System;
using System.Data.OleDb;
namespace Autumn.DB.Data.EXCELSupport
{
	internal class DBHelper : Autumn.DB.Data.ACCESSSupport.DBHelper
	{
		public DBHelper()
		{
		}
		public DBHelper(string constr)
		{
			this.selCon = (OleDbConnection)this.CreateConnection(constr);
		}
		public DBHelper(OleDbConnection con)
		{
			this.selCon = con;
		}
		public new object CreateConnection(string str)
		{
			return base.CreateConnection((str.IndexOf("Microsoft.Jet.OLEDB") == -1) ? ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"" + str + "\";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1'") : str);
		}
	}
}

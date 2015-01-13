using Autumn.DB.Config;
using Autumn.DB.Data;
using Autumn.DB.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
namespace Autumn.DB.ORM
{
	public class ORMUtil
	{
		private static DataTable _columnsTable = null;
		private static DataTable _forkeyTable = null;
		private static string _csprojFilePath = null;
		public static DataTable columnsTable
		{
			get
			{
				return ORMUtil._columnsTable;
			}
			set
			{
				ORMUtil._columnsTable = value;
			}
		}
		private static bool IsForkeyColumn(DataRow columnRowMeta)
		{
			if (ORMUtil._forkeyTable == null)
			{
				ORMUtil._forkeyTable = DBHelperFactory.CreateCommonDBHelper().GetSchema(SchemaInfoType.ForeignKeys);
			}
			return false;
		}
		public static string GetNameSpaceByProjectPath(string path)
		{
			string[] files = Directory.GetFiles(path);
			string text = null;
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				if (text2.IndexOf(".csproj") != -1)
				{
					ORMUtil._csprojFilePath = text2;
					StreamReader streamReader = new StreamReader(text2);
					text = streamReader.ReadToEnd();
					text = text.Substring(text.IndexOf("<RootNamespace>") + 15);
					text = text.Substring(0, text.IndexOf("</RootNamespace>"));
					streamReader.Close();
					streamReader.Dispose();
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				ORMUtil._csprojFilePath = null;
			}
			return text;
		}
		public static void Mapping(string configurationXml, string modelPath, string nameSpace, string[] createEntityTables, string[] createListEntitys)
		{
			if (nameSpace == null)
			{
				nameSpace = ORMUtil.GetNameSpaceByProjectPath(modelPath);
			}
			if (configurationXml != null)
			{
				ConfigManager.LoaderConfigurationXml(configurationXml);
			}
			DataTable schema = DBHelperFactory.CreateCommonDBHelper().GetSchema(SchemaInfoType.Columns);
			DataTable schema2 = DBHelperFactory.CreateCommonDBHelper().GetSchema(SchemaInfoType.ForeignKeys);
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, string> dBTypeMapping = DBHelperFactory.CreateCommonDBHelper().DBTypeMapping;
			XmlDocument xmlDocument = null;
			XmlNode xmlNode = null;
			if (ORMUtil._csprojFilePath != null)
			{
				xmlDocument = new XmlDocument();
				xmlDocument.Load(ORMUtil._csprojFilePath);
				foreach (XmlNode xmlNode2 in xmlDocument.LastChild)
				{
					if (xmlNode2.Name == "ItemGroup")
					{
						xmlNode = xmlNode2;
					}
				}
			}
			for (int i = 0; i < createEntityTables.Length; i++)
			{
				string text = createEntityTables[i];
				string text2 = (text.Length > 1) ? (text.Substring(0, 1).ToUpper() + text.Substring(1)) : text.ToUpper();
				stringBuilder.Length = 0;
				stringBuilder.Append("using System;\n");
				stringBuilder.Append("\nnamespace ").Append(nameSpace);
				stringBuilder.Append("\n{\n");
				stringBuilder.Append("\tpublic class ").Append(text2).Append("\n\t{\n");
				DataView defaultView = schema.DefaultView;
				defaultView.Sort = "ORDINAL_POSITION";
				DataRow[] array = defaultView.ToTable(false, new string[]
				{
					"TABLE_NAME",
					"COLUMN_NAME",
					"ORDINAL_POSITION",
					"DATA_TYPE"
				}).Select(string.Format("TABLE_NAME='{0}'", text));
				for (int j = 0; j < array.Length; j++)
				{
					DataRow dataRow = array[j];
					string text3 = dataRow["COLUMN_NAME"].ToString();
					string value = (text3.Length > 1) ? (text3.Substring(0, 1).ToLower() + text3.Substring(1)) : text3.ToLower();
					string value2 = (text3.Length > 1) ? (text3.Substring(0, 1).ToUpper() + text3.Substring(1)) : text3.ToUpper();
					stringBuilder.Append("\t\tprivate ").Append(dBTypeMapping[dataRow["DATA_TYPE"].ToString()]).Append(" ").Append(value).Append(";\n");
					stringBuilder.Append("\n\t\tpublic ").Append(dBTypeMapping[dataRow["DATA_TYPE"].ToString()]).Append(" ").Append(value2).Append("\n\t\t{\n\t\t\tget { return ").Append(value).Append("; }\n\t\t\tset { ").Append(value).Append(" = value; }\n\t\t}\n\n");
				}
				stringBuilder.Append("\t}\n}");
				StreamWriter streamWriter = new StreamWriter(modelPath + "\\" + text2 + ".cs");
				streamWriter.Write(stringBuilder);
				streamWriter.Flush();
				streamWriter.Close();
				if (xmlNode != null)
				{
					XmlElement xmlElement = xmlDocument.CreateElement("Compile", xmlDocument["Project"].NamespaceURI);
					xmlElement.SetAttribute("Include", text2 + ".cs");
					xmlNode.AppendChild(xmlElement);
				}
			}
			if (xmlNode != null)
			{
				xmlDocument.Save(ORMUtil._csprojFilePath);
				xmlDocument = null;
			}
		}
	}
}

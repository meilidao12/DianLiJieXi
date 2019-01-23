using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Services.DataBase
{
    public class AccessHelper
    {
        private OleDbHelper oledb;
        private string accessPath;
        private string dataBaseName = "YgSet.accdb";
        private string provider;
        public AccessHelper()
        {
            oledb = new OleDbHelper();
            if (File.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dataBaseName)))
            {
                if (dataBaseName.Split('.')[1] == ExtensionName.accdb.ToString())
                {
                    provider = Provider.AccessProvider2007;
                }
                else
                {
                    provider = Provider.AccessProvider2003;
                }
                oledb.Url = string.Format("Provider={0};Data Source={1}", provider,Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dataBaseName));
            }
        }

        public AccessHelper(string url)
        {
            oledb = new OleDbHelper();
            accessPath = url;
            if (!string.IsNullOrEmpty(accessPath) && File.Exists(accessPath))
            {
                string[] a = accessPath.Split('\\');
                if (a[a.Length - 1].Split('.')[1] == ExtensionName.accdb.ToString())
                {
                    provider = Provider.AccessProvider2007;
                }
                else
                {
                    provider = Provider.AccessProvider2003;
                }
                oledb.Url = string.Format("Provider={0};Data Source={1}",provider ,accessPath);
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool TestConn
        {
            get
            {
                return oledb.OleDbConnectionTest(oledb.Url);
            }
        }

        public void CreateTable()
        {
            string commandText = @"CREATE TABLE {0}({1} CHAR(10) NOT NULL,{2} 类型(数据长度))";
            var conn = new OleDbConnection(oledb.Url);
            //
        }

        public DataTable GetDataTable(string commandText)
        {
            DataSet ds;
            oledb.GetDataSet(commandText, out ds);
            return ds.Tables.Count == 0 ? null : ds.Tables[0];
        }

        public List<T> GetDataTable<T>(string commandText) where T :new()
        {
            DataSet ds;
            oledb.GetDataSet(commandText, out ds);
            if (ds.Tables.Count == 0) return null;
            DataTable dt = ds.Tables[0];
            List<T> list = new List<T>();             
            Type type = typeof(T);
            string tempName = "";
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;    
                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }


        public object ExecuteScalar(string commandText)
        {
            return oledb.GetSingle(commandText);
        }
        public bool Execute(string commandText)
        {          
                return oledb.OleDbExecute(commandText);  
        }

        public bool Execute(string commandText, OleDbParameter[] paras)
        {
            return oledb.OleDbExecute(commandText, paras);
        }

        public bool Execute(string commandText, DataTable dt)
        {
            return oledb.UpdateDbData(commandText, dt);
        }
    }

    public enum ExtensionName
    {
        accdb = 0,
        mdb = 1
    }

    public class Provider
    {
        public static string AccessProvider2003
        {
            get { return "Microsoft.Jet.OLEDB.4.0"; }
        }
        public static string AccessProvider2007
        {
            get { return "Microsoft.ACE.OleDb.12.0"; }
        }
        public static string AccessProvider2013
        {
            get { return "Microsoft.ACE.OleDb.15.0"; }
        }
    }

    /*
    增删改查例子
    增：insert into table (字段1, 字段2) values ('值','值')

    删：delete from table where 字段 = '值'

    改：update table set 姓名='梁朝伟' , 年龄='55' where 姓名= '梁朝伟'"

    查：select * from table where 字段 = '值'
    */
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Services.DataBase
{
    /*
    适用于不频繁的sql操作
    */
    public  class SqlHelper
    {
        SqlConnection conn;
        private string UrlRemote; /*= @"Data Source =192.168.1.106; Initial Catalog = YanGang_Data; User Id = sa; Password = sasa;";*/
        //private string UrlRemote = @"Data Source =192.168.11.51; Initial Catalog = JinMa_DiBang; User Id = dibang; Password = dibang;";
        //private string UrlLocal = @"server=.;database=Example;Trusted_Connection=SSPI;Connect Timeout=2";
        //private string UrlLocal1 = @"Data Source=localhost;Initial Catalog=Example;Integrated Security=True";        
        IniHelper ini = new IniHelper(System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini");
        public SqlHelper()
        {
            conn = new SqlConnection();
            UrlRemote = ini.ReadIni("Config", "SqlUrl");
            SimpleLogHelper.Instance.WriteLog(LogType.Info, "数据库连接地址：" + UrlRemote);
            conn.ConnectionString = UrlRemote;
        }

        public SqlHelper(string url)
        {
            conn = new SqlConnection();
            //conn.ConnectionString = UrlLocal;
            conn.ConnectionString = url;
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }           
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                conn.Close();
                return true;
            }
            catch
            {
                return false;
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
                switch (conn.State)
                {
                    case ConnectionState.Open:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <returns>返回SqlDataReader型数据</returns>
        /*
            需以下面方式将数据读出：
            SqlDataReader dr = sqlHelper.GetDataTable("select * from Example1");
            while(dr.Read())
            {
                Debug.WriteLine(dr["Name"].ToString());
            }
         */
        public SqlDataReader GetDataTable(string commandText)
        {
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            SqlCommand cmd = new SqlCommand(commandText, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            return dr;            
        }

        public DataTable GetDataTable1(string commandText)
        {
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            SqlCommand cmd = new SqlCommand(commandText, conn);
            SqlDataReader dr = cmd.ExecuteReader();
            return ConvertDataReaderToDataTable(dr);
        }

        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <typeparam name="T">泛型 为任意模型类</typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public List<T> GetDataTable<T>(string commandText) where T : new()
        {
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            SqlCommand cmd = new SqlCommand(commandText, conn);
            List<T> list;
            Type type = typeof(T);
            string tempName = string.Empty;
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.HasRows)
                {
                    list = new List<T>();
                    while (dr.Read())
                    {
                        T t = new T();
                        PropertyInfo[] propertys = typeof(T).GetProperties();
                        foreach (PropertyInfo pi in propertys)
                        {
                            tempName = pi.Name;
                            if (readerExists(dr, tempName))
                            {
                                if (!pi.CanWrite) continue;
                                var value = dr[tempName];
                                if (value != DBNull.Value)
                                {
                                    pi.SetValue(t, value.ToString(), null);
                                }
                            }
                        }
                        list.Add(t);
                    }
                    return list;
                }
            }
            return null;
        }



        public static DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
        {
            ///定义DataTable
            DataTable datatable = new DataTable();

            try
            {    ///动态添加表的数据列
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    DataColumn myDataColumn = new DataColumn();
                    myDataColumn.DataType = dataReader.GetFieldType(i);
                    myDataColumn.ColumnName = dataReader.GetName(i);
                    datatable.Columns.Add(myDataColumn);
                }

                ///添加表的数据
                while (dataReader.Read())
                {
                    DataRow myDataRow = datatable.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        myDataRow[i] = dataReader[i].ToString();
                    }
                    datatable.Rows.Add(myDataRow);
                    myDataRow = null;
                }
                ///关闭数据读取器
                dataReader.Close();
                return datatable;
            }
            catch (Exception ex)
            {
                ///抛出类型转换错误
                //SystemError.CreateErrorLog(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 判断SqlDataReader是否存在某列
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        private bool readerExists(SqlDataReader dr, string columnName)
        {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }

        /// <summary>
        /// 执行增删改操作
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public bool Execute(string commandText)
        {
            bool result;
            if(!TestConn)
            {
                if (!Open()) return false; 
            }
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = commandText;
            try
            {
                sqlCommand.ExecuteNonQuery();
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                Close();
            }
            return result;
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Reflection;
using Services;
namespace Services.DataBase
{
     public class OracleHelper
    {
        OracleConnection conn = null;
        string Url = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)));User Id=system;Password=password";
        public OracleHelper()
        {
            //IniHelper ini = new IniHelper(System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini");
            //SimpleLogHelper.Instance.WriteLog(LogType.Info,"ini地址为：" + System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini");
            //Url = ini.ReadIni("Config", "OracleUrl");
            //SimpleLogHelper.Instance.WriteLog(LogType.Info,"远程数据库地址为："+ Url);
            conn = new OracleConnection(Url);
        }

        public OracleHelper(string url)
        {
            //IniHelper ini = new IniHelper(System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini");
            //SimpleLogHelper.Instance.WriteLog(LogType.Info,"ini地址为：" + System.AppDomain.CurrentDomain.BaseDirectory + @"\Set.ini");
            //Url = ini.ReadIni("Config", "OracleUrl");
            //SimpleLogHelper.Instance.WriteLog(LogType.Info,"远程数据库地址为："+ Url);
            conn = new OracleConnection(url);
        }

        #region  ---方法：Open Close TestConn
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

        public bool TestConn
        {
            get
            {
                try
                {
                    switch (conn.State)
                    {
                        case ConnectionState.Open:
                            return true;
                        default:
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    SimpleLogHelper.Instance.WriteLog(LogType.Info, ex);
                    return false;
                }
               
            }
        }
        #endregion

        public OracleDataReader GetDataTable(string commandText)
        {
            OracleDataReader dr;
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            try
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            }
            catch
            {
                dr = null;
            }
            return dr;
        }

        public DataTable GetDataTable1(string commandText)
        {
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            OracleDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
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
            OracleCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            List<T> list;
            Type type = typeof(T);
            string tempName = string.Empty;
            using (OracleDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
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

        /// <summary>
        /// 判断SqlDataReader是否存在某列
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        private bool readerExists(OracleDataReader dr, string columnName)
        {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }

        public static DataTable ConvertDataReaderToDataTable(OracleDataReader dataReader)
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
    }
}

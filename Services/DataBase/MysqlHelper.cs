using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Services.DataBase
{
    public class MysqlHelper
    {
        MySqlConnection conn;
        //string Url = @"Server=10.0.1.234; Database=meterwater_data; Uid =tianchen; Password =tianchen;";
        string Url = AppSetting.Get("MySqlUrl");
        public MysqlHelper()
        {
            conn = new MySqlConnection(Url);
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
                switch (conn.State)
                {
                    case ConnectionState.Open:
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        public MySqlDataReader GetDataTable(string commandText)
        {
            MySqlDataReader dr;
            if (!TestConn)
            {
                if (!Open()) return null;
            }
            try
            {
                MySqlCommand cmd = conn.CreateCommand();
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
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
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
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = commandText;
            List<T> list;
            Type type = typeof(T);
            string tempName = string.Empty;
            try
            {
                using (MySqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
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
                                        pi.SetValue(t, value, null);
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
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 判断SqlDataReader是否存在某列
        /// </summary>
        /// <param name="dr">SqlDataReader</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        private bool readerExists(MySqlDataReader dr, string columnName)
        {
            dr.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (dr.GetSchemaTable().DefaultView.Count > 0);
        }

        public static DataTable ConvertDataReaderToDataTable(MySqlDataReader dataReader)
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

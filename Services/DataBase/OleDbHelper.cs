using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace Services.DataBase
{ 
    public class OleDbHelper
    {
        public string Url;

        public string Error;

        /// <summary>
        /// 测试连接数据库
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool OleDbConnectionTest(string url)
        {
            try
            {
                using (OleDbConnection oldCon = new OleDbConnection(url))
                {
                    if (oldCon.State == ConnectionState.Closed)
                        oldCon.Open();

                    if (oldCon.State == System.Data.ConnectionState.Open)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public bool OleDbExecute(string command)
        {
            try
            {
                using (OleDbConnection oldCon = new OleDbConnection(Url))
                {
                    if (oldCon.State == ConnectionState.Closed)
                        oldCon.Open();

                    OleDbCommand oleCmd = new OleDbCommand(command, oldCon);
                    oleCmd.CommandTimeout = 0;
                    
                    return oleCmd.ExecuteNonQuery() > 0; ;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public bool TableExists(string tableName)
        {
            string sql = @"IF  EXISTS (SELECT name FROM sysobjects WHERE name = '" + tableName + @"') select 'exists' as result else select 'notexists' as result";
            try
            {
                string result = GetSingle(sql).ToString();
                return result.Equals("exists");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool OleDbExecute(string command, OleDbParameter[] parameters)
        {
            try
            {
                using (OleDbConnection oldCon = new OleDbConnection(Url))
                {
                    if (oldCon.State == ConnectionState.Closed)
                        oldCon.Open();

                    OleDbCommand oleCmd = new OleDbCommand(command, oldCon);
                    oleCmd.Parameters.AddRange(parameters);

                    oleCmd.CommandTimeout = 0;
                    oleCmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        public object GetSingle(string command)
        {
            try
            {
                using (OleDbConnection oldCon = new OleDbConnection(Url))
                {
                    if (oldCon.State == ConnectionState.Closed)
                        oldCon.Open();

                    OleDbCommand oleCmd = new OleDbCommand(command, oldCon);
                    oleCmd.CommandTimeout = 0;
                    oleCmd.CommandType = System.Data.CommandType.Text;

                    return oleCmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return null;
            }
        }

        public bool GetDataSet(string command, out DataSet ds)
        {
            ds = null;
            try
            {
                using (OleDbConnection oldCon = new OleDbConnection(Url))
                {
                    OleDbCommand oleCmd = new OleDbCommand(command, oldCon);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(oleCmd);

                    ds = new DataSet();
                    adapter.Fill(ds);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
        }

        //public bool UpdateDbData(string command, DataSet ds)
        //{
        //    if (UpdateDbData(ds.Tables[0], "SystemOption") >= 0)
        //        return true;
        //    else
        //        return false;
        //}

        public bool UpdateDbData(string commandText, DataTable table)
        {
            int affect = -1;
            try
            {

                OleDbConnection connection = new OleDbConnection(Url);
                using (connection)
                {
                    // Create the select command.
                    OleDbCommand command = new OleDbCommand(commandText, connection);
                    // Create the DbDataAdapter.
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    // Create the DbCommandBuilder.
                    OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
                    builder.RefreshSchema();
                    // Get the insert, update and delete commands.
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    adapter.DeleteCommand = builder.GetDeleteCommand();
                    var dd = table.GetChanges();
                    affect = adapter.Update(table);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return affect > 0 ? true : false;
        }

        /// <summary>
        /// 批量操作表
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="dt"></param>
        /// <param name="strTblName">数据库表名</param>
        /// <param name="where">除主键外的条件</param>
        /// <returns></returns>
        private int UpdateDbData(DataTable dt, string strTblName, string where = null)
        {
            int affect = -1;

            StringBuilder cmdFields = new StringBuilder();
            foreach (DataColumn column in dt.Columns)
            {
                cmdFields.Append(column.ColumnName + ",");
            }
            if (cmdFields.Length > 0)
            {
                cmdFields = cmdFields.Remove(cmdFields.Length - 1, 1);
            }
            using (OleDbConnection oldCon = new OleDbConnection(Url))
            {
                OleDbCommand myCommand = new OleDbCommand(string.Format("select {0} from {1}", cmdFields, strTblName), oldCon);
                try
                {
                    OleDbDataAdapter myAdapter = new OleDbDataAdapter();
                    myAdapter.SelectCommand = myCommand;
                    OleDbCommandBuilder myCommandBuilder = new OleDbCommandBuilder
                    {
                        DataAdapter = myAdapter,
                        SetAllValues = true,
                        ConflictOption = ConflictOption.OverwriteChanges,
                    };
                    myCommandBuilder.RefreshSchema();


                    myAdapter.InsertCommand = myCommandBuilder.GetInsertCommand(true);
                    myAdapter.InsertCommand.Connection = oldCon;
                    myAdapter.UpdateCommand = myCommandBuilder.GetUpdateCommand(true);
                    myAdapter.DeleteCommand = myCommandBuilder.GetDeleteCommand(true);

                    if (!string.IsNullOrEmpty(where))
                    {
                        myAdapter.UpdateCommand.CommandText = myAdapter.UpdateCommand.CommandText.Replace("WHERE", "WHERE " + where + " and ");
                    }

                    var dd = dt.GetChanges();
                    affect = myAdapter.Update(dt);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {

                }
            }

            return affect;
        }
    }
}

/*
常用sql语句：
  1.获取最新的一条数据
    SELECT * FROM test ORDER BY Addtime DESC LIMIT 2; 
*/

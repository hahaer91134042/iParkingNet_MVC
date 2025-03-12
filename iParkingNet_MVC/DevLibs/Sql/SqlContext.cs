using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Dapper;

/**
 * Creat by Hill
 * 隨插即用型 可以用在無法使用MastPage得情況
 **/
namespace DevLibs
{
    public class SqlContext : IDisposable
    {
        private SqlConnection SqlConn;
        private int Timeout = 10000;
        //private SqlCommand SqlCmd;
        //private SqlDataAdapter sqlAdapter;
        private string connStr;

        public SqlContext(ISql sql)
        {
            connStr = sql.dbConfig().connetString();
            Timeout = sql.timeOut();
            SqlConn = new SqlConnection(connStr);

            SqlConn.Open();
        }
        //public SqlContext(string connectionStr)
        //{
        //    connStr = connectionStr;
        //    SqlConn = new SqlConnection(connectionStr);
        //    SqlConn.Open();
        //}


        /// <summary>
        /// Use Dapper 1.50.5 for .net framework 4.6.1
        /// usage page:https://dotblogs.com.tw/oldnick/2018/01/15/dapper
        /// </summary>
        /// <typeparam name="T">data object</typeparam>
        /// <param name="cmd">sql Cmd</param>
        /// <param name="o">query parameter</param>
        /// <returns></returns>
        public IEnumerable<T> query<T>(string cmd, object o = null)
        {
            return SqlConn.Query<T>(cmd, o);
        }
        public T queryFirst<T>(string cmd, object o = null) => SqlConn.QueryFirstOrDefault<T>(cmd, o);
        public int execute(string cmd, object o = null)
        {
            //return row effect number
            return SqlConn.Execute(cmd, o);
        }

        public DataTable query(string cmdStr)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand(cmdStr, SqlConn);
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
                adapter.SelectCommand.CommandTimeout = Timeout;
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
            catch (Exception)
            {

            }
            return null;
        }

        //public void close()
        //{        
        //}

        public void Dispose()
        {
            SqlConn.Close();
        }
    }
}


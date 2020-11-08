using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PSI.DbUtility
{
    public class SqlHelper
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        /// <summary>
        /// 增、刪、改的通用方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters">參數</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, int cmdType, params SqlParameter[] parameters)
        {
            int result = 0;
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                result = cmd.ExecuteNonQuery();  // 執行T-SQL並返回受影響的行數
                cmd.Parameters.Clear();
            }
            return result;
        }

        /// <summary>
        /// 執行SQL查詢，返回第一行第一列的值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql, int cmdType, params SqlParameter[] parameters)
        {
            object result = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                result = cmd.ExecuteScalar();  // 執行T-SQL並返回受影響的行數
                cmd.Parameters.Clear();
                // DBNull 當至資料庫查詢，符合的資料回傳的資料欄位沒有值，此欄位的值就=DBNull
                // null 此時是至資料庫查詢但沒有符合的資料
                if (result == null || result == DBNull.Value)
                {
                    return null;
                }
                else
                {
                    return result;
                }
            }
        }

        public static SqlDataReader SqlDataReader(string sql, int cmdType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
            SqlDataReader reader;
            try
            {
                conn.Open();
                // 若用using會關閉資料庫連線，然而此方法須維持連線才能取得資料，DAL確定使用完後才可關閉連線。
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw new Exception($"reader發生異常，{ex}");
            }
        }

        /// <summary>
        /// 取得單個資料表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, int cmdType, params SqlParameter[] parameters)
        {
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// 取得多個資料表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, int cmdType, params SqlParameter[] parameters)
        {
            DataSet ds = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = BuilderCommand(conn, sql, cmdType, null, parameters);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 執行批次SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmdType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool ExecuteTrans(List<string> listSql)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand cmd = BuilderCommand(conn, "", 1, trans);
                try
                {
                    int count = 0;
                    for(int i = 0; i < listSql.Count; i++)
                    {
                        if(listSql[i].Length > 0)
                        {
                            cmd.CommandText = listSql[i];
                            cmd.CommandType = CommandType.Text;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("執行出現異常");
                }
            }
        }

        /// <summary>
        /// 執行批次可帶有參數或預存程序的SQL
        /// </summary>
        /// <param name="cmdList"></param>
        /// <returns></returns>
        public static bool ExecuteTrans(List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                SqlCommand cmd = BuilderCommand(conn, "", 1, trans);
                try
                {
                    int count = 0;
                    for (int i = 0; i < cmdList.Count; i++)
                    {
                        cmd.CommandText = cmdList[i].CommandText;
                        cmd.CommandType = cmdList[i].IsProcess ?  CommandType.StoredProcedure : CommandType.Text;
                        if(cmdList[i].Paras != null && cmdList[i].Paras.Length > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddRange(cmdList[i].Paras);
                        }
                        count += cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception("執行出現異常");
                }
            }
        }


        /// <summary>
        /// 建立Sql Command
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private static SqlCommand BuilderCommand(SqlConnection conn, string sql, int cmdType, SqlTransaction trans, params SqlParameter[] parameters)
        {
            if(conn == null) throw new ArgumentException("SqlConnection不能為空");
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (cmdType == 2) cmd.CommandType = CommandType.StoredProcedure;
            if (conn.State == ConnectionState.Closed) conn.Open();
            if (trans != null) cmd.Transaction = trans;
            if(parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }

        /// <summary>
        /// 用於一系列資料須環環相扣處理時
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T ExecuteTrans<T>(Func<IDbCommand, T> action)
        {
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                IDbTransaction trans = conn.BeginTransaction();
                IDbCommand cmd = conn.CreateCommand();
                cmd.Transaction = trans;
                return action(cmd);
            }
        }



    }
}

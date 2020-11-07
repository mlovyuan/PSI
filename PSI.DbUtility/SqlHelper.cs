using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

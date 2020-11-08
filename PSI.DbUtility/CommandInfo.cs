using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI.DbUtility
{
    public class CommandInfo
    {
        public string CommandText;
        public SqlParameter[] Paras;
        public bool IsProcess; // 是否有預存程序

        public CommandInfo()
        {

        }
        public CommandInfo(string cmdText, bool isProcess)
        {
            CommandText = cmdText;
            IsProcess = isProcess;
        }
        public CommandInfo(string cmdText, bool isProcess, SqlParameter[] paras)
        {
            CommandText = cmdText;
            IsProcess = isProcess;
            Paras = paras;
        }
    }
}

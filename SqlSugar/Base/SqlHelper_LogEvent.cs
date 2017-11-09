using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 执行日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <param name="isStarting"></param>
        protected virtual void ExecLogEvent(string sql, SqlParameter[] pars, bool isStarting = true)
        {
            if (this.IsEnableLogEvent)
            {
                Action<string, string> logAction;
                if (isStarting) {
                    this.CmdBeginTime = DateTime.Now;
                    logAction = LogEventStarting;
                }
                else {
                    logAction = LogEventCompleted;
                }

                if (logAction != null)
                {
                    if (pars == null || pars.Length == 0)
                    {
                        logAction(sql, null);
                    }
                    else
                    {
                        logAction(sql, JsonConverter.Serialize(pars.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }

        }

    }
}

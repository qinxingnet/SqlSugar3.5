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
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, object pars)
        {
            return ExecuteCommand(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 创建一个新的SQL连接
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual SqlCommand CreateSqlCommand(string sql, params SqlParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                sqlCommand.Transaction = _tran;
            }
            if (pars != null) sqlCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(sqlCommand.Parameters);
            }

            return sqlCommand;
        }
        /// <summary>
        /// 执行SQL返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual int ExecuteCommand(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);

            SqlCommand sqlCommand = CreateSqlCommand(sql, pars);

            int count = -1;
            CheckConnectionOpen(() =>
            {
                count = sqlCommand.ExecuteNonQuery();
                if (this.IsAutoClearParameters)
                    sqlCommand.Parameters.Clear();
                ExecLogEvent(sql, pars, false);
            });

            return count;
        }



    }
}

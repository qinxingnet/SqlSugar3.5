using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：底层SQL辅助函数
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：
    /// </summary>
    public abstract partial class SqlHelper : IDisposable
    {

        /// <summary>
        /// 连接对象
        /// </summary>
        protected SqlConnection _sqlConnection;

        /// <summary>
        /// 主连接
        /// </summary>
        protected SqlConnection _masterConnection = null;
        /// <summary>
        /// 从连接
        /// </summary>
        protected List<SqlConnection> _slaveConnections = null;


        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 初始化 SqlHelper 类的新实例
        /// </summary>
        /// <param name="masterConnectionString"></param>
        /// <param name="slaveConnectionStrings"></param>
        public SqlHelper(string masterConnectionString, List<string> slaveConnectionStrings)
        {
            _masterConnection = new SqlConnection(masterConnectionString);
            if (slaveConnectionStrings == null || slaveConnectionStrings.Count == 0)
            {
                _slaveConnections = new List<SqlConnection>()
                {
                    _masterConnection
                };
            }
            else
            {
                _slaveConnections = new List<SqlConnection>();
                foreach (var item in slaveConnectionStrings)
                {
                    _slaveConnections.Add(new SqlConnection(item));
                }
            }
        }


        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual SqlDataReader GetReader(string sql, object pars)
        {
            return GetReader(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual SqlDataReader GetReader(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlCommand sqlCommand = CreateSqlCommand(sql, pars);
            SqlDataReader sqlDataReader = null;
            CheckConnectionOpen(() =>
            {
                sqlDataReader = sqlCommand.ExecuteReader();
                if (this.IsAutoClearParameters) sqlCommand.Parameters.Clear();
                ExecLogEvent(sql, pars, false);
            });

            return sqlDataReader;
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, object pars)
        {
            return GetList<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null);
            return reval;
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T">可以是int、string等，也可以是类或者数组、字典</typeparam>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, object pars)
        {
            return GetSingle<T>(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 根据SQL获取T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual T GetSingle<T>(string sql, params SqlParameter[] pars)
        {
            var reval = SqlSugarTool.DataReaderToList<T>(typeof(T), GetReader(sql, pars), null).Single();
            return reval;
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, object pars)
        {
            return GetDataTable(sql, SqlSugarTool.GetParameters(pars));
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataTable GetDataTable(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            _sqlDataAdapter.SelectCommand.CommandType = this.CommandType;
            if (pars != null)
                _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }

            DataTable dt = new DataTable();
            CheckConnectionOpen(() =>
            {
                _sqlDataAdapter.Fill(dt);
                if (this.IsAutoClearParameters) _sqlDataAdapter.SelectCommand.Parameters.Clear();
                ExecLogEvent(sql, pars, false);
            });

            return dt;
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, object pars)
        {
            return GetDataSetAll(sql, SqlSugarTool.GetParameters(pars));
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual DataSet GetDataSetAll(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(false);
            ExecLogEvent(sql, pars, true);
            SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(sql, _sqlConnection);
            if (_tran != null)
            {
                _sqlDataAdapter.SelectCommand.Transaction = _tran;
            }
            if (this.IsGetPageParas)
            {
                SqlSugarToolExtensions.RequestParasToSqlParameters(_sqlDataAdapter.SelectCommand.Parameters);
            }
            _sqlDataAdapter.SelectCommand.CommandTimeout = this.CommandTimeOut;
            _sqlDataAdapter.SelectCommand.CommandType = this.CommandType;
            if (pars != null) _sqlDataAdapter.SelectCommand.Parameters.AddRange(pars);


            DataSet ds = new DataSet();
            CheckConnectionOpen(() =>
            {
                _sqlDataAdapter.Fill(ds);
                if (this.IsAutoClearParameters) _sqlDataAdapter.SelectCommand.Parameters.Clear();
                ExecLogEvent(sql, pars, false);
            });
            return ds;
        }

        /// <summary>
        /// 获取行列结构
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public virtual object GetScalar(string sql, params SqlParameter[] pars)
        {
            SetCurrentConnection(true);
            ExecLogEvent(sql, pars, true);

            SqlCommand sqlCommand = CreateSqlCommand(sql, pars);

            object scalar = 0;
            CheckConnectionOpen(() =>
            {
                scalar = sqlCommand.ExecuteScalar();
                scalar = (scalar ?? 0);
                if (this.IsAutoClearParameters) sqlCommand.Parameters.Clear();
                ExecLogEvent(sql, pars, false);
            });
            return scalar;
        }

        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        public virtual void Dispose()
        {
            if (_sqlConnection != null)
            {
                if (_sqlConnection.State != ConnectionState.Closed)
                {
                    if (_tran != null)  _tran.Commit();
                    _sqlConnection.Close();
                }
                _sqlConnection = null;
            }
        }

    }
}

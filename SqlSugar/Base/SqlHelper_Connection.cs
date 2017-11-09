using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 获取当前数据库连接对象
        /// </summary>
        /// <returns></returns>
        public virtual SqlConnection GetConnection()
        {
            return _sqlConnection;
        }

        /// <summary>
        /// 设置当前主从连接对象
        /// </summary>
        /// <param name="isMaster"></param>
        public virtual void SetCurrentConnection(bool isMaster)
        {
            if (_slaveConnections != null && _slaveConnections.Count > 0)//开启主从模式
            {
                if (isMaster || _tran != null)
                {
                    _sqlConnection = _masterConnection;
                }
                else
                {
                    var count = _slaveConnections.Count;
                    _sqlConnection = _slaveConnections[new Random().Next(0, count - 1)];
                }
            }
        }


        /// <summary>
        /// 检查数据库连接，若未连接，连接数据库
        /// </summary>
        protected virtual void CheckConnectionOpen()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
        }


        /// <summary>
        /// 检查数据库连接，若未连接，连接数据库
        /// </summary>
        protected virtual void CheckConnectionOpen(Action willConnectedAction)
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
            willConnectedAction();
        }

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class SqlHelper
    {
        /// <summary>
        /// 开始事务
        /// </summary>
        public virtual void BeginTran()
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                _tran = _sqlConnection.BeginTransaction();
            });
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        public virtual void BeginTran(IsolationLevel iso)
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                _tran = _sqlConnection.BeginTransaction(iso);
            });

        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="transactionName"></param>
        public virtual void BeginTran(string transactionName)
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                _tran = _sqlConnection.BeginTransaction(transactionName);
            });

        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="iso">指定事务行为</param>
        /// <param name="transactionName"></param>
        public virtual void BeginTran(IsolationLevel iso, string transactionName)
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                _tran = _sqlConnection.BeginTransaction(iso, transactionName);
            });
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public virtual void RollbackTran()
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                if (_tran != null)
                {
                    _tran.Rollback();
                    _tran = null;
                }
            });
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public virtual void CommitTran()
        {
            SetCurrentConnection(true);
            CheckConnectionOpen(() =>
            {
                if (_tran != null)
                {
                    _tran.Commit();
                    _tran = null;
                }
            });

        }

    }
}

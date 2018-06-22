using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;
using System.Data.SqlClient;
using Mediachase.Data.Provider;

namespace Mediachase.Data.Provider
{
    public class TransactionScope : IDisposable
    {
        private TransactionScope current;
        private bool hasRolledBack;
        private Hashtable transactions;
        private int txCount;
        private static LocalDataStoreSlot txSlot;

        // Nested Types
        public delegate IDbConnection ConnectionDelegate();

        public Hashtable Transactions
        {
            get
            {
                return this.current.transactions;
            }
        }

        // Methods
        static TransactionScope()
        {
            TransactionScope._isolationLevel = IsolationLevel.ReadCommitted; // same default isolation level as the .NET 2.0 one
            TransactionScope.txSlot = Thread.AllocateDataSlot();
        }

        public TransactionScope()
        {
            this.AssignCurrentTx();
            this.current.txCount++;
        }

        public TransactionScope(IsolationLevel isolation)
        {
            TransactionScope._isolationLevel = isolation;
            this.AssignCurrentTx();
            this.current.txCount++;
        }

        private void AssignCurrentTx()
        {
            object obj = Thread.GetData(TransactionScope.txSlot);
            if (obj != null)
            {
                this.current = (TransactionScope)obj;
            }
            else
            {
                this.current = this;
                this.current.transactions = new Hashtable();
                Thread.SetData(TransactionScope.txSlot, this);
            }       
        }

        public void Complete()
        {
            this.current.txCount--;
            if (this.current.txCount == 0)
            {
                foreach (TransactionScope.Transaction tran in this.current.transactions.Values)
                {
                    IDbConnection connection = tran.sqlTx.Connection;
                    tran.sqlTx.Commit();
                    tran.sqlTx.Dispose();                    
                    tran.sqlTx = null;
                    if ((connection != null) && (connection.State == ConnectionState.Open))
                    {
                        connection.Close();
                    }
                }
                this.current.transactions.Clear();
            }
        }

        public static void DeEnlist(IDbCommand cmd)
        {
            if (TransactionScope.GetCurrentTx() == null)
            {
                cmd.Connection.Dispose();
            }
        }

        public static void Enlist(IDbCommand cmd, string connectionString, ConnectionDelegate connection)
        {
            TransactionScope scope = TransactionScope.GetCurrentTx();
            if (scope == null)
            {
                cmd.Connection = connection();
                cmd.Connection.ConnectionString = connectionString;
                cmd.Connection.Open();
            }
            else
            {
                TransactionScope.Transaction transaction = scope.transactions[connectionString] as TransactionScope.Transaction;
                if (transaction == null)
                {
                    //transaction = new TransactionScope.Transaction();
                    IDbConnection conn = connection();
                    conn.ConnectionString = connectionString;
                    conn.Open();
                    transaction = new TransactionScope.Transaction(conn);// This is Roee's Bug Fix
                    if (TransactionScope._isolationLevel != IsolationLevel.Unspecified)
                    {
                        transaction.sqlTx = conn.BeginTransaction(TransactionScope._isolationLevel);
                    }
                    else
                    {
                        transaction.sqlTx = conn.BeginTransaction();
                    }
                    scope.transactions[connectionString] = transaction;
                }
                //cmd.Connection = transaction.sqlTx.Connection;
                cmd.Connection = transaction.connectionReference; // This is Roee's Bug Fix
                cmd.Transaction = transaction.sqlTx;
            }
        }

        /// <summary>
        /// Returns false if connection is resused, otherwise true. Make sure to close connection in that case.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="connectionString"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool OpenConnection(IDbCommand cmd, string connectionString, ConnectionDelegate connection)
        {
            TransactionScope scope = TransactionScope.GetCurrentTx();
            if (scope == null)
            {
                cmd.Connection = connection();
                cmd.Connection.ConnectionString = connectionString;
                cmd.Connection.Open();
                return true;
            }
            else
            {
                TransactionScope.Transaction transaction = scope.transactions[connectionString] as TransactionScope.Transaction;
                if (transaction == null)
                {
                    cmd.Connection = connection();
                    cmd.Connection.ConnectionString = connectionString;
                    cmd.Connection.Open();
                    return true;
                }
                else
                {
                    cmd.Connection = transaction.sqlTx.Connection;
                    cmd.Transaction = transaction.sqlTx;
                    return false;
                }
            }
        }

        private static TransactionScope GetCurrentTx()
        {
            return (Thread.GetData(TransactionScope.txSlot) as TransactionScope);
        }

        private void Rollback()
        {
            if (!this.current.hasRolledBack && (this.current.txCount > 0))
            {
                this.current.hasRolledBack = true;
                foreach (TransactionScope.Transaction transaction1 in this.current.transactions.Values)
                {
                    IDbConnection connection1 = transaction1.sqlTx.Connection;
                    try
                    {
                        transaction1.sqlTx.Rollback();
                        transaction1.sqlTx.Dispose();
                        transaction1.sqlTx = null;
                        if ((connection1 != null) && (connection1.State == ConnectionState.Open))
                        {
                            connection1.Close();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                this.current.transactions.Clear();
                this.current.txCount = 0;
            }       
        }

        void IDisposable.Dispose()
        {
            if (this == this.current)
            {
                if ((this.current.txCount > 0) && (this.current.transactions.Count > 0))
                {
                    this.Rollback();
                }
                Thread.SetData(TransactionScope.txSlot, null);
            }
        }

        // Properties
        public static IsolationLevel IsolationLevel 
        {
            get
            {
                return TransactionScope._isolationLevel;
            }
            set
            {
                TransactionScope._isolationLevel = value;
            }        
        }

        // Fields
        private static IsolationLevel _isolationLevel = IsolationLevel.ReadCommitted;

        public class Transaction
        {
            // Methods
            public Transaction(IDbConnection connection)
            {
                connectionReference = connection;
            }

            // Fields
            public IDbTransaction sqlTx;

            public IDbConnection connectionReference;
        }
    }
 

}

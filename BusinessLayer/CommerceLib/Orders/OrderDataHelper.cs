using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public class OrderDataHelper
    {
        /// <summary>
        /// Creates the config data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateConfigDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = OrderConfiguration.Instance.Connections.ConfigurationAppDatabase;
            return cmd;
        }

        /// <summary>
        /// Creates the tran data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateTranDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = OrderConfiguration.Instance.Connections.TransactionAppDatabase;
            return cmd;
        }
    }
}

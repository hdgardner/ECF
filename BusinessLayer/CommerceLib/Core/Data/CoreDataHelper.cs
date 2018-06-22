using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Core.Data
{
    /// <summary>
    /// Implements core data helper method.
    /// </summary>
    public static class CoreDataHelper
    {
        /// <summary>
        /// Creates the data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = CoreConfiguration.Instance.Connection.AppDatabase;
            return cmd;
        }
    }
}

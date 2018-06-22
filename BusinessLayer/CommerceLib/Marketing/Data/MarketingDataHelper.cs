using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Marketing.Data
{
    /// <summary>
    /// Implements helper methods for marketing data.
    /// </summary>
    public static class MarketingDataHelper
    {
        /// <summary>
        /// Creates the data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = MarketingConfiguration.Instance.Connection.AppDatabase;
            return cmd;
        }
    }
}

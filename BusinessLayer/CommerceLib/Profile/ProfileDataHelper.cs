using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Implements helper methods for the profile data.
    /// </summary>
    public static class ProfileDataHelper
    {
        /// <summary>
        /// Creates the data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = ProfileConfiguration.Instance.Connection.AppDatabase;
            return cmd;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Helper class for catalog data and data commands
    /// </summary>
    public static class CatalogDataHelper
    {
        /// <summary>
        /// Creates the data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = CatalogConfiguration.Instance.Connection.AppDatabase;
            return cmd;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using System.Configuration;
using System.Globalization;

namespace TaxImporter
{
    public static class TaxImporterDataHelper
    {
        //
        // Borrowed from Mediachase.Commerce.Catalog.Data in CatalogDataHelper.cs
        // referenced in CommerceLib\Catalog\Data\CatalogAdmin.cs
        // 

        /// <summary>
        /// Creates the data command.
        /// </summary>
        /// <returns></returns>
        public static DataCommand CreateDataCommand()
        {
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = ConfigurationManager.ConnectionStrings["EcfSqlConnection"].ToString();
            return cmd;
        }

        //
        // Borrowed from Mediachase.Commerce.Storage.DataHelper in DataHelper.cs
        // referenced in CommerceLib\Catalog\Data\CatalogAdmin.cs
        // 

        /// <summary>
        /// Creates the name of the insert stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateInsertStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Insert", new object[] { TableName });
        }

        /// <summary>
        /// Creates the name of the update stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateUpdateStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Update", new object[] { TableName });
        }

        /// <summary>
        /// Creates the name of the delete stored procedure.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <returns></returns>
        internal static string CreateDeleteStoredProcedureName(string TableName)
        {
            if (TableName == null)
            {
                throw new ArgumentNullException("TableName");
            }

            return string.Format(CultureInfo.InvariantCulture, "ecf_{0}_Delete", new object[] { TableName });
        }
    }
}

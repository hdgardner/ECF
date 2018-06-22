using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;

namespace TaxImporter
{
    class TaxDao
    {
        /// <summary>
        /// Deletes the tax record.
        /// </summary>
        /// <param name="taxId">The tax id.</param>
        /// <returns></returns>
        public static int DeleteTax(int taxId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("Tax");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("TaxId", taxId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the tax record.
        /// </summary>
        /// <param name="tax">A tax dto.</param>
        /// <returns></returns>
        public static int InsertTax(TaxDto tax)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("Tax");
            cmd.Parameters = new DataParameters();
            DataParameter taxId = new DataParameter("TaxId", tax.TaxId);
            taxId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(taxId);
            cmd.Parameters.Add(new DataParameter("TaxType", tax.TaxType));
            cmd.Parameters.Add(new DataParameter("Name", tax.Name));
            cmd.Parameters.Add(new DataParameter("SortOrder", tax.SortOrder));
            cmd.Parameters.Add(new DataParameter("ApplicationId", tax.ApplicationId));
            
            DataResult result = DataService.ExecuteNonExec(cmd);

            // return TaxId
            return int.Parse(taxId.Value.ToString());
        }

        #region Update Disabled
        //public static int UpdateTax(int taxId, TaxDto tax)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("Tax");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("TaxId", taxId));
        //    cmd.Parameters.Add(new DataParameter("TaxType", tax.TaxType));
        //    cmd.Parameters.Add(new DataParameter("Name", tax.Name));
        //    cmd.Parameters.Add(new DataParameter("SortOrder", tax.SortOrder));
        //    cmd.Parameters.Add(new DataParameter("ApplicationId", tax.ApplicationId));
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks if the specified tax exists.
        /// </summary>
        /// <param name="tax">A tax dto.</param>
        /// <returns></returns>
        public static bool Exists(TaxDto tax)
        {
            // See Unique Index: IX_Tax(Unique, Non-Clustered)
            // Uses: Name, ApplicationId

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM Tax WHERE Name='{0}' and ApplicationId='{1}'", 
                tax.Name, 
                tax.ApplicationId);
            count = int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());

            if (count >= 1) // count should never be greater than one.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the tax id.
        /// </summary>
        /// <param name="tax">A tax dto.</param>
        /// <returns>The tax id.</returns>
        public static int GetTaxId(TaxDto tax)
        {
            // for use as a FK value
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT TaxId FROM Tax WHERE Name='{0}' and ApplicationId='{1}'", 
                tax.Name, 
                tax.ApplicationId);
            return int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());
        }
    }
}

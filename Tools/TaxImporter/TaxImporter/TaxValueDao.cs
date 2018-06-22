using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;

namespace TaxImporter
{
    class TaxValueDao
    {
        /// <summary>
        /// Deletes the tax value record.
        /// </summary>
        /// <param name="taxValueId">The tax value id.</param>
        /// <returns></returns>
        public static int DeleteTaxValue(int taxValueId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("TaxValue");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("TaxValueId", taxValueId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the tax value record.
        /// </summary>
        /// <param name="taxValue">A tax value dto.</param>
        /// <returns></returns>
        public static int InsertTaxValue(TaxValueDto taxValue)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("TaxValue");
            cmd.Parameters = new DataParameters();
            DataParameter taxValueId = new DataParameter("TaxValueId", taxValue.TaxValueId);
            taxValueId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(taxValueId);
            cmd.Parameters.Add(new DataParameter("Percentage", taxValue.Percentage));
            cmd.Parameters.Add(new DataParameter("TaxId", taxValue.TaxId));
            cmd.Parameters.Add(new DataParameter("TaxCategory", taxValue.TaxCategory));
            cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", taxValue.JurisdictionGroupId));
            cmd.Parameters.Add(new DataParameter("SiteId", taxValue.SiteId));
            cmd.Parameters.Add(new DataParameter("AffectiveDate", taxValue.AffectiveDate));

            DataResult result = DataService.ExecuteNonExec(cmd);

            // return TaxValueId
            return int.Parse(taxValueId.Value.ToString());
        }

        #region Update Disabled
        //public static int UpdateTaxValue(int taxValueId, TaxValueDto taxValue)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("TaxValue");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("TaxValueId", taxValueId));
        //    cmd.Parameters.Add(new DataParameter("Percentage", taxValue.Percentage));
        //    cmd.Parameters.Add(new DataParameter("TaxId", taxValue.TaxId));
        //    cmd.Parameters.Add(new DataParameter("TaxCategory", taxValue.TaxCategory));
        //    cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", taxValue.JurisdictionGroupId));
        //    cmd.Parameters.Add(new DataParameter("SiteId", taxValue.SiteId));
        //    cmd.Parameters.Add(new DataParameter("AffectiveDate", taxValue.AffectiveDate));
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks if the specified tax value exists.
        /// </summary>
        /// <param name="taxValue">A tax value dto.</param>
        /// <returns></returns>
        public static bool Exists(TaxValueDto taxValue)
        {
            // Check for uniqueness by: TaxCategory, SiteId

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM TaxValue WHERE TaxCategory='{0}' and SiteId='{1}'",
                taxValue.TaxCategory,
                taxValue.SiteId);
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
        /// Gets the tax value id.
        /// </summary>
        /// <param name="taxValue">A tax value dto.</param>
        /// <returns>The tax value id.</returns>
        public static int GetTaxValueId(TaxValueDto taxValue)
        {
            // for use as a FK value
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT TaxValueId FROM TaxValue WHERE TaxCategory='{0}' and SiteId='{1}'",
                taxValue.TaxCategory,
                taxValue.SiteId);
            return int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;

namespace TaxImporter
{
    class TaxLanguageDao
    {
        /// <summary>
        /// Deletes the tax language record.
        /// </summary>
        /// <param name="taxLanguageId">The tax language id.</param>
        /// <returns></returns>
        public static int DeleteTaxLanguage(int taxLanguageId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("TaxLanguage");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("TaxId", taxLanguageId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the tax language record.
        /// </summary>
        /// <param name="taxLanguage">A tax language dto.</param>
        /// <returns></returns>
        public static int InsertTaxLanguage(TaxLanguageDto taxLanguage)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("TaxLanguage");
            cmd.Parameters = new DataParameters();
            DataParameter taxLanguageId = new DataParameter("TaxLanguageId", taxLanguage.TaxLanguageId);
            taxLanguageId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(taxLanguageId);
            cmd.Parameters.Add(new DataParameter("DisplayName", taxLanguage.DisplayName));
            cmd.Parameters.Add(new DataParameter("LanguageCode", taxLanguage.LanguageCode));
            cmd.Parameters.Add(new DataParameter("TaxId", taxLanguage.TaxId));

            DataResult result = DataService.ExecuteNonExec(cmd);

            // return TaxLanguageId
            return int.Parse(taxLanguageId.Value.ToString());
        }

        #region Update Disabled
        //public static int UpdateTaxLanguage(int taxLanguageId, TaxLanguageDto taxLanguage)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("TaxLanguage");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("TaxLanguageId", taxLanguageId));
        //    cmd.Parameters.Add(new DataParameter("DisplayName", taxLanguage.DisplayName));
        //    cmd.Parameters.Add(new DataParameter("LanguageCode", taxLanguage.LanguageCode));
        //    cmd.Parameters.Add(new DataParameter("TaxId", taxLanguage.TaxId));
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks if the specified tax language exists
        /// </summary>
        /// <param name="taxLanguage">A tax language dto.</param>
        /// <returns></returns>
        public static bool Exists(TaxLanguageDto taxLanguage)
        {
            // Check for uniqueness by: TaxCategory, SiteId

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM TaxLanguage WHERE Displayname='{0}' and LanguageCode='{1}' and TaxId='{2}'",
                taxLanguage.DisplayName,
                taxLanguage.LanguageCode,
                taxLanguage.TaxId);
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
        /// Gets the tax language id.
        /// </summary>
        /// <param name="taxLanguage">A tax language dto.</param>
        /// <returns>The tax language id.</returns>
        public static int GetTaxLanguageId(TaxLanguageDto taxLanguage)
        {
            // for use as a FK value
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT TaxLanguageId FROM TaxLanguage WHERE Displayname='{0}' and LanguageCode='{1}' and TaxId='{2}'",
                taxLanguage.DisplayName,
                taxLanguage.LanguageCode,
                taxLanguage.TaxId);
            return int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());
        }
    }
}
    
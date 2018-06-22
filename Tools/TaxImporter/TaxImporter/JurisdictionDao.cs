using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Mediachase.Data.Provider;

namespace TaxImporter
{
    class JurisdictionDao
    {
        /// <summary>
        /// Deletes the jurisdiction record.
        /// </summary>
        /// <param name="jurisdictionId">The jurisdiction id.</param>
        /// <returns></returns>
        public static int DeleteJurisdiction(int jurisdictionId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("Jurisdiction");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("SiteId", jurisdictionId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the jurisdiction record.
        /// </summary>
        /// <param name="jurisdiction">A jurisdiction dto.</param>
        /// <returns></returns>
        public static int InsertJurisdiction(JurisdictionDto jurisdiction)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("Jurisdiction");
            cmd.Parameters = new DataParameters();
            DataParameter jurisdictionId = new DataParameter("JurisdictionId", jurisdiction.JurisdictionId);
            jurisdictionId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(jurisdictionId);
            cmd.Parameters.Add(new DataParameter("DisplayName", jurisdiction.DisplayName));
            cmd.Parameters.Add(new DataParameter("StateProvinceCode", jurisdiction.StateProvinceCode));
            cmd.Parameters.Add(new DataParameter("CountryCode", jurisdiction.CountryCode));
            cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdiction.JurisdictionType));
            cmd.Parameters.Add(new DataParameter("ZipPostalCodeStart", jurisdiction.ZipPostalCodeStart));
            cmd.Parameters.Add(new DataParameter("ZipPostalCodeEnd", jurisdiction.ZipPostalCodeEnd));
            cmd.Parameters.Add(new DataParameter("City", jurisdiction.City));
            cmd.Parameters.Add(new DataParameter("District", jurisdiction.District));
            cmd.Parameters.Add(new DataParameter("County", jurisdiction.County));
            cmd.Parameters.Add(new DataParameter("GeoCode", jurisdiction.GeoCode));
            cmd.Parameters.Add(new DataParameter("ApplicationId", jurisdiction.ApplicationId));
            cmd.Parameters.Add(new DataParameter("Code", jurisdiction.Code));
            
            DataResult result = DataService.ExecuteNonExec(cmd);

            // return JurisdictionId
            return int.Parse(jurisdictionId.Value.ToString());
        }

        #region Update Disabled
        //public static int UpdateJurisdiction(int jurisdictionId, JurisdictionDto jurisdiction)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("Jurisdiction");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("JurisdictionId", jurisdictionId));
        //    cmd.Parameters.Add(new DataParameter("DisplayName", jurisdiction.DisplayName));
        //    cmd.Parameters.Add(new DataParameter("StateProvinceCode", jurisdiction.StateProvinceCode));
        //    cmd.Parameters.Add(new DataParameter("CountryCode", jurisdiction.CountryCode));
        //    cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdiction.JurisdictionType));
        //    cmd.Parameters.Add(new DataParameter("ZipPostalCodeStart", jurisdiction.ZipPostalCodeStart));
        //    cmd.Parameters.Add(new DataParameter("ZipPostalCodeEnd", jurisdiction.ZipPostalCodeEnd));
        //    cmd.Parameters.Add(new DataParameter("City", jurisdiction.City));
        //    cmd.Parameters.Add(new DataParameter("District", jurisdiction.District));
        //    cmd.Parameters.Add(new DataParameter("County", jurisdiction.County));
        //    cmd.Parameters.Add(new DataParameter("GeoCode", jurisdiction.GeoCode));
        //    cmd.Parameters.Add(new DataParameter("ApplicationId", jurisdiction.ApplicationId));
        //    cmd.Parameters.Add(new DataParameter("Code", jurisdiction.Code)); ;
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks if the specified jurisdiction exists.
        /// </summary>
        /// <param name="jurisdiction">A jurisdiction dto.</param>
        /// <returns></returns>
        public static bool Exists(JurisdictionDto jurisdiction)
        {
            // Check for uniqueness by: JurisdictionType, Code

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM Jurisdiction WHERE JurisdictionType='{0}' and Code='{1}'", 
                jurisdiction.JurisdictionType,
                jurisdiction.Code);
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
        /// Gets the jurisdiction id.
        /// </summary>
        /// <param name="jurisdiction">A jurisdiction dto.</param>
        /// <returns>The jurisdiction id.</returns>
        public static int GetJurisdictionId(JurisdictionDto jurisdiction)
        {
            // for use as a FK value
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT JurisdictionId FROM Jurisdiction WHERE JurisdictionType='{0}' and Code='{1}'",
                jurisdiction.JurisdictionType,
                jurisdiction.Code);
            return int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());
        }
    }
}

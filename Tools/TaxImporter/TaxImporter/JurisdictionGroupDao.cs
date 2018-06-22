using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;

namespace TaxImporter
{
    class JurisdictionGroupDao
    {
        /// <summary>
        /// Deletes the jurisdiction group record.
        /// </summary>
        /// <param name="jurisdictionGroupId">The jurisdiction group id.</param>
        /// <returns></returns>
        public static int DeleteJurisdictionGroup(int jurisdictionGroupId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("JurisdictionGroup");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionGroupId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the jurisdiction group record.
        /// </summary>
        /// <param name="jurisdictionGroup">A jurisdiction group dto.</param>
        /// <returns></returns>
        public static int InsertJurisdictionGroup(JurisdictionGroupDto jurisdictionGroup)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("JurisdictionGroup");
            cmd.Parameters = new DataParameters();
            DataParameter jurisdictionGroupId = new DataParameter("JurisdictionGroupId", jurisdictionGroup.JurisdictionGroupId);
            jurisdictionGroupId.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(jurisdictionGroupId);
            cmd.Parameters.Add(new DataParameter("ApplicationId", jurisdictionGroup.ApplicationId));
            cmd.Parameters.Add(new DataParameter("DisplayName", jurisdictionGroup.DisplayName));
            cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdictionGroup.JurisdictionType));
            cmd.Parameters.Add(new DataParameter("Code", jurisdictionGroup.Code));
            
            DataResult result = DataService.ExecuteNonExec(cmd);

            // return JurisdictionGroupId
            return int.Parse(jurisdictionGroupId.Value.ToString());
        }

        #region Update Disabled
        //public static int UpdateJurisdictionGroup(int jurisdictionGroupId, JurisdictionGroupDto jurisdictionGroup)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("JurisdictionGroup");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionGroupId));
        //    cmd.Parameters.Add(new DataParameter("ApplicationId", jurisdictionGroup.ApplicationId));
        //    cmd.Parameters.Add(new DataParameter("DisplayName", jurisdictionGroup.DisplayName));
        //    cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdictionGroup.JurisdictionType));
        //    cmd.Parameters.Add(new DataParameter("Code", jurisdictionGroup.Code));
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks if the specified jurisdiction group exists.
        /// </summary>
        /// <param name="jurisdictionGroup">A jurisdiction group dto.</param>
        /// <returns></returns>
        public static bool Exists(JurisdictionGroupDto jurisdictionGroup)
        {
            // Check for uniqueness by: JurisdictionType, Code

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM JurisdictionGroup WHERE JurisdictionType='{0}' and Code='{1}'",
                jurisdictionGroup.JurisdictionType,
                jurisdictionGroup.Code);
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
        /// Gets the jurisdiction group id.
        /// </summary>
        /// <param name="jurisdictionGroup">A jurisdiction group dto.</param>
        /// <returns>The jurisdiction group id.</returns>
        public static int GetJurisdictionGroupId(JurisdictionGroupDto jurisdictionGroup)
        {
            // for use as a FK value
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT JurisdictionGroupId FROM JurisdictionGroup WHERE JurisdictionType='{0}' and Code='{1}'",
                jurisdictionGroup.JurisdictionType,
                jurisdictionGroup.Code);
            return int.Parse(DataService.ExecuteScalar(cmd).Scalar.ToString());
        }
    }
}

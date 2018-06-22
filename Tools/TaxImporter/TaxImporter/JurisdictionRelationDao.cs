using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;

namespace TaxImporter
{
    class JurisdictionRelationDao
    {
        /// <summary>
        /// Deletes the jurisdiction relation record.
        /// </summary>
        /// <param name="jursidctionId">The jursidction id.</param>
        /// <param name="jurisdictionGroupId">The jurisdiction group id.</param>
        /// <returns></returns>
        public static int DeleteJurisdictionRelation(int jursidctionId, int jurisdictionGroupId)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateDeleteStoredProcedureName("JurisdictionRelation");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("JurisdictionId", jursidctionId));
            cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionGroupId));
            return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        }

        /// <summary>
        /// Inserts the jurisdiction relation record.
        /// </summary>
        /// <param name="jurisdictionRelation">A jurisdiction relation dto.</param>
        public static void InsertJurisdictionRelation(JurisdictionRelationDto jurisdictionRelation)
        {
            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandText = TaxImporterDataHelper.CreateInsertStoredProcedureName("JurisdictionRelation");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("JurisdictionId", jurisdictionRelation.JurisdictionId));
            cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionRelation.JurisdictionGroupId));
            DataService.ExecuteNonExec(cmd);
        }

        #region Update Disabled
        //public static int UpdateJurisdictionRelation(int jurisdictionId, int jurisdictionGroupId)
        //{
        //    DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
        //    cmd.CommandText = TaxImporterDataHelper.CreateUpdateStoredProcedureName("JurisdictionRelation");
        //    cmd.Parameters = new DataParameters();
        //    cmd.Parameters.Add(new DataParameter("JurisdictionId", jurisdictionId));
        //    cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionGroupId));
        //    return Int32.Parse(DataService.ExecuteNonExec(cmd).Scalar.ToString());
        //}
        #endregion

        /// <summary>
        /// Checks whether the specified jurisdiction relation exists.
        /// </summary>
        /// <param name="jurisdictionRelation">A jurisdiction relation dto.</param>
        /// <returns></returns>
        public static bool Exists(JurisdictionRelationDto jurisdictionRelation)
        {
            // Check for uniqueness by: JurisdictionId, JurisdictionGroupId

            int count;

            DataCommand cmd = TaxImporterDataHelper.CreateDataCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT COUNT(*) FROM JurisdictionRelation WHERE JurisdictionId='{0}' and JurisdictionGroupId='{1}'",
                jurisdictionRelation.JurisdictionId,
                jurisdictionRelation.JurisdictionGroupId);
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
    }
}

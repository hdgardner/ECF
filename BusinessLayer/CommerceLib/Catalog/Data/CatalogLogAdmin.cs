using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Exceptions;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrative functions for the log
    /// </summary>
    public class CatalogLogAdmin
    {
        private CatalogLogDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CatalogLogDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogLogAdmin"/> class.
        /// </summary>
        internal CatalogLogAdmin()
        {
            _DataSet = new CatalogLogDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogLogAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogLogAdmin(CatalogLogDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="created">The created.</param>
        /// <param name="startingRecord">The starting record.</param>
        /// <param name="numberOfRecords">The number of records.</param>
        /// <param name="totalRecords">The total records.</param>
        internal void Load(string operation, string objectType, DateTime created, int startingRecord, int numberOfRecords, ref int totalRecords)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_CatalogLog");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));

            if (!String.IsNullOrEmpty(operation))
                cmd.Parameters.Add(new DataParameter("Operation", operation, DataParameterType.NVarChar, 50));

            if (!String.IsNullOrEmpty(objectType))
                cmd.Parameters.Add(new DataParameter("ObjectType", objectType, DataParameterType.NVarChar, 50));

            if (created != DateTime.MinValue)
                cmd.Parameters.Add(new DataParameter("Created", created, DataParameterType.DateTime));

            cmd.Parameters.Add(new DataParameter("StartingRec", startingRecord, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("NumRecords", numberOfRecords, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("CatalogLog");

            DataService.LoadDataSet(cmd);

            if (CurrentDto.CatalogLog.Count > 0)
                totalRecords = (int)this.CurrentDto.CatalogLog[0]["TotalCount"];
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "CatalogLog");
                scope.Complete();
            }
        }
    }
}
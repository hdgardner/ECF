using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Commerce.Core.Data
{
    /// <summary>
    /// Implements administrative functions for the log
    /// </summary>
    public class LogAdmin
    {
        private LogDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public LogDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAdmin"/> class.
        /// </summary>
        internal LogAdmin()
        {
            _DataSet = new LogDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal LogAdmin(LogDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the specified operation.
        /// </summary>
        /// <param name="source">Is system log.</param>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="created">The created.</param>
        /// <param name="startingRecord">The starting record.</param>
        /// <param name="numberOfRecords">The number of records.</param>
        /// <param name="totalRecords">The total records.</param>
        internal void Load(bool isSystemLog, string source, string operation, string objectType, DateTime created, int startingRecord, int numberOfRecords, ref int totalRecords)
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_ApplicationLog");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", AppContext.Current.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("IsSystemLog", isSystemLog, DataParameterType.Bit, 1));

            if (!String.IsNullOrEmpty(source))
                cmd.Parameters.Add(new DataParameter("Source", source, DataParameterType.NVarChar, 100));

            if (!String.IsNullOrEmpty(operation))
                cmd.Parameters.Add(new DataParameter("Operation", operation, DataParameterType.NVarChar, 50));

            if (!String.IsNullOrEmpty(objectType))
                cmd.Parameters.Add(new DataParameter("ObjectType", objectType, DataParameterType.NVarChar, 50));

            if (created != DateTime.MinValue)
                cmd.Parameters.Add(new DataParameter("Created", created, DataParameterType.DateTime));

            cmd.Parameters.Add(new DataParameter("StartingRec", startingRecord, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("NumRecords", numberOfRecords, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("ApplicationLog");

            DataService.LoadDataSet(cmd);

            if (CurrentDto.ApplicationLog.Count > 0)
                totalRecords = (int)this.CurrentDto.ApplicationLog[0]["TotalCount"];
        }


        /// <summary>
        /// Load the log by id.
        /// </summary>
        internal void Load(int logId)
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_ApplicationLog_LogId");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("LogId", logId, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("ApplicationLog");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(null, cmd, CurrentDto, "ApplicationLog");
                scope.Complete();
            }
        }
    }
}
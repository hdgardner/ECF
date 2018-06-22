using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Core.Data
{
    /// <summary>
    /// Implements the methods for and represents the application administrator.
    /// </summary>
    public class AppAdmin
    {
        private AppDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        internal AppDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAdmin"/> class.
        /// </summary>
        internal AppAdmin()
        {
            _DataSet = new AppDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal AppAdmin(AppDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads by the application ID.
        /// </summary>
        /// <param name="appId">The app id.</param>
        internal void LoadByApplication(Guid appId)
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_Application");
            cmd.Parameters = new DataParameters();

            if(appId == Guid.Empty)
                cmd.Parameters.Add(new DataParameter("ApplicationId", DataParameterType.UniqueIdentifier));
            else
                cmd.Parameters.Add(new DataParameter("ApplicationId", appId, DataParameterType.UniqueIdentifier));

            cmd.DataSet = CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("Application");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads by the application name.
        /// </summary>
        /// <param name="name">The name.</param>
        internal void LoadByApplication(string name)
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_ApplicationByName");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 200));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("Application");

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Updates the catalog.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = CoreDataHelper.CreateDataCommand();
            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(null, cmd, CurrentDto, "Application");
                scope.Complete();
            }
        }
    }
}

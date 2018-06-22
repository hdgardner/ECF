using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Profile.Dto;

namespace Mediachase.Commerce.Profile.Data
{
    /// <summary>
    /// Contains all the functions needed to perform administration on the role permissions
    /// </summary>
    public class PermissionAdmin
    {
        private PermissionDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public PermissionDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionAdmin"/> class.
        /// </summary>
        internal PermissionAdmin()
        {
            _DataSet = new PermissionDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal PermissionAdmin(PermissionDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Updates the permissions.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.RolePermission == null)
                return;

            if (CurrentDto.RolePermission.Count == 0)
                return;

            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(ProfileContext.MetaDataContext, cmd, CurrentDto, "RolePermission");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified roles.
        /// </summary>
        /// <param name="roles">The roles.</param>
        internal void Load(string[] roles)
        {
            string list = String.Empty;

            foreach (string role in roles)
            {
                if (String.IsNullOrEmpty(list))
                    list = role;
                else
                    list = list + "," + role;
            }

            DataCommand cmd = ProfileDataHelper.CreateDataCommand();
            cmd.CommandText = String.Format("ecf_RolePermission");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", ProfileConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("Roles", list, DataParameterType.NVarChar));

            cmd.DataSet = this.CurrentDto;
            cmd.TableMapping = DataHelper.MapTables("RolePermission");

            DataService.LoadDataSet(cmd);
        }
    }
}
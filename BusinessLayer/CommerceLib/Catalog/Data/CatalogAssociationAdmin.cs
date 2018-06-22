using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Exceptions;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrator operations for the catalog association.
    /// </summary>
    public class CatalogAssociationAdmin
    {
		private const string _CatalogAssociationTableName = "CatalogAssociation";
		private const string _CatalogEntryAssociationTableName = "CatalogEntryAssociation";
		private const string _AssociationTypeTableName = "AssociationType";

        private CatalogAssociationDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        internal CatalogAssociationDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        /// <value>The mapping.</value>
        private DataTableMapping[] Mapping
        {
            get
            {
				return DataHelper.MapTables(_CatalogAssociationTableName, _CatalogEntryAssociationTableName, _AssociationTypeTableName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogAssociationAdmin"/> class.
        /// </summary>
        internal CatalogAssociationAdmin()
        {
            _DataSet = new CatalogAssociationDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogAssociationAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CatalogAssociationAdmin(CatalogAssociationDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the specified catalog association id.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
		internal void Load(int catalogAssociationId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_CatalogAssociation");
            cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("CatalogAssociationId", catalogAssociationId, DataParameterType.Int));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the specified catalog association name.
        /// </summary>
        /// <param name="catalogAssociationName">Name of the catalog association.</param>
        internal void Load(string catalogAssociationName)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_CatalogAssociationByName");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("AssociationName", catalogAssociationName, DataParameterType.NVarChar, 150));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.CatalogAssociation == null)
                return;

            if (CurrentDto.CatalogAssociation.Count == 0)
                return;

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, _CatalogAssociationTableName, _CatalogEntryAssociationTableName);
                scope.Complete();
            }
        }

		/// <summary>
		/// Saves this instance.
		/// </summary>
		internal void SaveAssociationType()
		{
			if (CurrentDto.AssociationType == null)
				return;

			if (CurrentDto.AssociationType.Count == 0)
				return;

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, _AssociationTypeTableName);
				scope.Complete();
			}
		}

        /// <summary>
        /// Deletes the specified catalog association id.
        /// </summary>
        /// <param name="catalogAssociationId">The catalog association id.</param>
        internal static void Delete(int catalogAssociationId)
        {
            CatalogAssociationAdmin admin = new CatalogAssociationAdmin();
            admin.Load(catalogAssociationId);

            if (admin.CurrentDto.CatalogAssociation.Count == 0)
                throw new InvalidObjectException();

            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = DataHelper.CreateDeleteStoredProcedureName("CatalogAssociation");
            cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("CatalogAssociationId", catalogAssociationId));

			DataService.ExecuteNonExec(cmd);
        }

        /// <summary>
        /// Loads the association by catalog entry id.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        internal void LoadByCatalogEntryId(int catalogEntryId)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_CatalogAssociation_CatalogEntryId");
            cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("CatalogEntryId", catalogEntryId, DataParameterType.Int));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads the association by catalog entry code.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
		internal void LoadByCatalogEntryCode(int catalogId, string catalogEntryCode)
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_CatalogAssociation_CatalogEntryCode");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
            cmd.Parameters.Add(new DataParameter("CatalogEntryCode", catalogEntryCode, DataParameterType.NVarChar, 100));
            cmd.DataSet = CurrentDto;
            cmd.TableMapping = Mapping;

            DataService.LoadDataSet(cmd);
        }

		/// <summary>
		/// Loads the association by catalog id.
		/// </summary>
		/// <param name="catalogId">The catalog id.</param>
		internal void LoadByCatalogId(int catalogId)
		{
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = String.Format("ecf_CatalogAssociation_CatalogId");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("CatalogId", catalogId, DataParameterType.Int));
			cmd.DataSet = CurrentDto;
			cmd.TableMapping = Mapping;

			DataService.LoadDataSet(cmd);
		}
    }
}
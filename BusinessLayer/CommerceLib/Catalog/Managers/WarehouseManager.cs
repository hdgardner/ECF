using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Catalog.Managers
{
	/// <summary>
	/// Implements operations for the warehouse manager.
	/// </summary>
	public static class WarehouseManager
	{
		/// <summary>
		/// Gets the warehouses.
		/// </summary>
		/// <returns></returns>
		public static WarehouseDto GetWarehouseDto()
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalog-warehouse", CatalogConfiguration.Instance.ApplicationId.ToString());

			WarehouseDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (WarehouseDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = CatalogDataHelper.CreateDataCommand();
				cmd.CommandText = "ecf_Warehouse";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.DataSet = new WarehouseDto();
				cmd.TableMapping = DataHelper.MapTables("Warehouse");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (WarehouseDto)results.DataSet;

				// Insert to the cache collection
				CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogSchemaTimeout);
			}

			return dto;
		}

		/// <summary>
		/// Gets the warehouses.
		/// </summary>
		/// <returns></returns>
		public static WarehouseDto GetWarehouseByWarehouseId(int warehouseId)
		{
			// Assign new cache key, specific for site guid and response groups requested
			//string cacheKey = CatalogCache.CreateCacheKey("catalog-warehouse-id", CatalogConfiguration.Instance.ApplicationId.ToString(), warehouseId);

			WarehouseDto dto = null;

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_Warehouse_WarehouseId";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("WarehouseId", warehouseId, DataParameterType.Int));
			cmd.DataSet = new WarehouseDto();
			cmd.TableMapping = DataHelper.MapTables("Warehouse");

			DataResult results = DataService.LoadDataSet(cmd);

			dto = (WarehouseDto)results.DataSet;

			return dto;
		}

		/// <summary>
		/// Gets the warehouse id by name.
		/// </summary>
		/// <returns>WarehouseId. If warehouse is not found, reutrns -1.</returns>
		public static int GetWarehouseIdByName(string name)
		{
			WarehouseDto dto = GetWarehouseDto();

			int warehouseId = -1;

			if (dto != null)
			{
				WarehouseDto.WarehouseRow[] whRows = (WarehouseDto.WarehouseRow[])dto.Warehouse.Select(String.Format("Name='{0}'", name));
				if (whRows != null && whRows.Length > 0)
					warehouseId = whRows[0].WarehouseId;
			}

			return warehouseId;
		}

        /// <summary>
        /// Gets the warehouse id by code.
        /// </summary>
        /// <returns>WarehouseId. If warehouse is not found, reutrns -1.</returns>
        public static int GetWarehouseIdByCode(string code)
        {
            WarehouseDto dto = GetWarehouseDto();

            int warehouseId = -1;

            if (dto != null)
            {
                WarehouseDto.WarehouseRow[] whRows = (WarehouseDto.WarehouseRow[])dto.Warehouse.Select(String.Format("Code='{0}'", code));
                if (whRows != null && whRows.Length > 0)
                    warehouseId = whRows[0].WarehouseId;
            }

            return warehouseId;
        }

		/// <summary>
		/// Saves changes in WarehouseDto.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SaveWarehouse(WarehouseDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("WarehouseDto can not be null"));

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, dto, "Warehouse");
				scope.Complete();
			}
		}
	}
}
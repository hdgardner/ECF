using System;
using System.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Catalog.Data;

namespace Mediachase.Commerce.Catalog.Managers
{
	/// <summary>
	/// Implements operations for the tax manager.
	/// </summary>
	public static class CatalogTaxManager
	{
		#region Tax Functions
		/// <summary>
		/// Gets the taxes.
		/// </summary>
		/// <returns></returns>
		public static CatalogTaxDto GetTaxCategories()
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = CatalogCache.CreateCacheKey("catalog-taxcategory", CatalogConfiguration.Instance.ApplicationId.ToString());

			CatalogTaxDto dto = null;

			// check cache first
			object cachedObject = CatalogCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CatalogTaxDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = CatalogDataHelper.CreateDataCommand();
				cmd.CommandText = "ecf_TaxCategory";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.DataSet = new CatalogTaxDto();
				cmd.TableMapping = DataHelper.MapTables("TaxCategory");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (CatalogTaxDto)results.DataSet;

				// Insert to the cache collection
				CatalogCache.Insert(cacheKey, dto, CatalogConfiguration.Instance.Cache.CatalogSchemaTimeout);
			}

			return dto;
		}

		/// <summary>
		/// Gets the tax category by id.
		/// </summary>
		/// <returns></returns>
		public static CatalogTaxDto GetTaxCategoryByTaxCategoryId(int taxCategoryId)
		{
			CatalogTaxDto dto = null;

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_TaxCategory_TaxCategoryId";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("TaxCategoryId", taxCategoryId, DataParameterType.Int));
			cmd.DataSet = new CatalogTaxDto();
			cmd.TableMapping = DataHelper.MapTables("TaxCategory");

			DataResult results = DataService.LoadDataSet(cmd);

			dto = (CatalogTaxDto)results.DataSet;

			return dto;
		}

		/// <summary>
		/// Gets the tax category by name.
		/// </summary>
		/// <returns></returns>
		public static CatalogTaxDto GetTaxCategoryByName(string taxCategoryName)
		{
			CatalogTaxDto dto = null;

			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_TaxCategory_Name";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Name", taxCategoryName, DataParameterType.NVarChar, 50));
			cmd.DataSet = new CatalogTaxDto();
			cmd.TableMapping = DataHelper.MapTables("TaxCategory");

			DataResult results = DataService.LoadDataSet(cmd);

			dto = (CatalogTaxDto)results.DataSet;

			return dto;
		}

		/// <summary>
		/// Gets the tax category id by name.
		/// </summary>
		/// <returns>TaxCategoryId. If tax category is not found, returns -1.</returns>
		public static int GetTaxCategoryIdByName(string name)
		{
			CatalogTaxDto dto = GetTaxCategoryByName(name);

			int tcId = -1;

			if (dto != null && dto.TaxCategory.Count > 0)
				tcId = dto.TaxCategory[0].TaxCategoryId;

			return tcId;
		}

		/// <summary>
		/// Gets the tax category name by id.
		/// </summary>
		/// <returns>TaxCategory name. If tax category is not found, returns empty string.</returns>
		public static string GetTaxCategoryNameById(int id)
		{
			CatalogTaxDto dto = GetTaxCategoryByTaxCategoryId(id);

			string tcName = String.Empty;

			if (dto != null && dto.TaxCategory.Count > 0)
				tcName = dto.TaxCategory[0].Name;

			return tcName;
		}

		/// <summary>
		/// Creates tax category with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="updateExisting">If true and tax category with the specified name already exists, it will be updated. Otherwise, a new category will be created.</param>
		/// <returns></returns>
		public static CatalogTaxDto CreateTaxCategory(string name, bool updateExisting)
		{
			CatalogTaxDto taxCategoryDto = GetTaxCategoryByName(name);
			CatalogTaxDto.TaxCategoryRow row = null;

			if (taxCategoryDto != null && taxCategoryDto.TaxCategory.Count > 0 && updateExisting)
			{
				// do nothing
			}
			else
			{
				taxCategoryDto = new CatalogTaxDto();

				row = taxCategoryDto.TaxCategory.NewTaxCategoryRow();
				row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
				row.Name = name;

				taxCategoryDto.TaxCategory.Rows.Add(row);

				SaveTaxCategory(taxCategoryDto);
			}

			return taxCategoryDto;
		}
		#endregion

		#region Edit Tax Functions
		/// <summary>
		/// Saves changes in TaxCategory table.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SaveTaxCategory(CatalogTaxDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("TaxDto can not be null"));

			// TODO: Check if user is allowed to perform this operation
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, dto, "TaxCategory");
				scope.Complete();
			}
		}
		#endregion
	}
}
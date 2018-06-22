using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
    /// <summary>
    /// Country manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class CountryManager
    {
        #region Country Functions
		/// <summary>
		/// Returns all visible countries and regions.
		/// </summary>
		/// <returns></returns>
		public static CountryDto GetCountries()
		{
			return GetCountries(false);
		}
        /// <summary>
        /// Gets the countries.
        /// </summary>
		/// <param name="returnInactive">If true, all countries will be returned, otherwise only visible.</param>
        /// <returns></returns>
		public static CountryDto GetCountries(bool returnInactive)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("countries", returnInactive.ToString());

			CountryDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (CountryDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Country]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
				cmd.DataSet = new CountryDto();
				cmd.TableMapping = DataHelper.MapTables("Country", "StateProvince");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (CountryDto)results.DataSet;

				// Insert to the cache collection
				OrderCache.Insert(cacheKey, dto, OrderConfiguration.Instance.Cache.CountryCollectionTimeout);
			}

			return dto;
		}

		/// <summary>
		/// Gets the country by id.
		/// </summary>
		/// <param name="countryId">Id of the country.</param>
		/// <param name="returnInactive">If true, all regions will be returned, otherwise only visible.</param>
		/// <returns></returns>
		public static CountryDto GetCountry(int countryId, bool returnInactive)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_Country_CountryId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("CountryId", countryId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new CountryDto();
			cmd.TableMapping = DataHelper.MapTables("Country", "StateProvince");

			DataResult results = DataService.LoadDataSet(cmd);

			return (CountryDto)results.DataSet;
		}

		/// <summary>
		/// Gets the country by code.
		/// </summary>
		/// <param name="code">Country code.</param>
		/// <param name="returnInactive">If true, all regions will be returned, otherwise only visible.</param>
		/// <returns></returns>
		public static CountryDto GetCountry(string code, bool returnInactive)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_Country_Code]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Code", code, DataParameterType.NVarChar, 3));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new CountryDto();
			cmd.TableMapping = DataHelper.MapTables("Country", "StateProvince");

			DataResult results = DataService.LoadDataSet(cmd);

			return (CountryDto)results.DataSet;
		}
		#endregion

		#region Edit Country Functions
        /// <summary>
        /// Saves changes in CountryDto. Not implemented!
        /// </summary>
        /// <param name="dto">The dto.</param>
		public static void SaveCountry(CountryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("CountryDto can not be null"));

			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "Country", "StateProvince");
				scope.Complete();
			}
        }
        #endregion
    }
}

using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
    /// <summary>
    /// Shipping manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class ShippingManager
    {
        #region Shipping Functions
        /// <summary>
        /// Gets the shipping methods.
        /// </summary>
        /// <param name="languageid">The languageid.</param>
        /// <param name="returnInactive">if set to <c>true</c> [return inactive].</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingMethods(string languageid, bool returnInactive)
		{
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = OrderCache.CreateCacheKey("shipping-methods", languageid, returnInactive.ToString());

            ShippingMethodDto dto = null;

            // check cache first
            object cachedObject = OrderCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (ShippingMethodDto)cachedObject;


            // Load the object
            if (dto == null)
            {
                DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
                cmd.CommandText = "[ecf_ShippingMethod_Language]";
                cmd.Parameters = new DataParameters();
                cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("LanguageId", String.IsNullOrEmpty(languageid) ? null : languageid, DataParameterType.NVarChar, 10));
                cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
                cmd.DataSet = new ShippingMethodDto();
                cmd.TableMapping = DataHelper.MapTables("ShippingOption", "ShippingOptionParameter", "ShippingMethod", "ShippingMethodParameter", "ShippingMethodCase", "ShippingCountry", "ShippingRegion", "ShippingPaymentRestriction", "Package", "ShippingPackage");

                DataResult results = DataService.LoadDataSet(cmd);

                dto = (ShippingMethodDto)results.DataSet;

                // Insert to the cache collection
                OrderCache.Insert(cacheKey, dto, OrderConfiguration.Instance.Cache.ShippingCollectionTimeout);
            }

			return dto;
		}

        /// <summary>
        /// Returns list of active shipping methods.
        /// </summary>
        /// <param name="languageid">The languageid.</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingMethods(string languageid)
		{
			return GetShippingMethods(languageid, false);
		}

        /// <summary>
        /// Returns shippping option by shipping option id.
        /// </summary>
        /// <param name="shippingOptionId">The shipping option id.</param>
        /// <param name="returnInactive">If true, return inactive shipping methods based on specified ShippingOption</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingOption(Guid shippingOptionId, bool returnInactive)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingOption_ShippingOptionId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ShippingOptionId", shippingOptionId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new ShippingMethodDto();
			cmd.TableMapping = DataHelper.MapTables("ShippingOption", "ShippingOptionParameter", "Package", "ShippingPackage");

			DataResult results = DataService.LoadDataSet(cmd);

			return (ShippingMethodDto)results.DataSet;
		}

        /// <summary>
        /// Returns shippping option by shipping option id.
		/// </summary>
        /// <param name="shippingOptionId">The shipping option id.</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingOption(Guid shippingOptionId)
		{
			return GetShippingOption(shippingOptionId, false);
		}

        /// <summary>
        /// Returns shippping method by shipping method id.
        /// </summary>
        /// <param name="shippingMethodId">The shipping method id.</param>
        /// <param name="returnInactive">If true, return inactive shipping methods</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingMethod(Guid shippingMethodId, bool returnInactive)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingMethod_ShippingMethodId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ShippingMethodId", shippingMethodId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new ShippingMethodDto();
			cmd.TableMapping = DataHelper.MapTables("ShippingOption", "ShippingOptionParameter", "ShippingMethod", "ShippingMethodParameter", "ShippingMethodCase", "ShippingCountry", "ShippingRegion", "ShippingPaymentRestriction", "Package", "ShippingPackage");

			DataResult results = DataService.LoadDataSet(cmd);

			return (ShippingMethodDto)results.DataSet;
		}

        /// <summary>
        /// Returns active shippping method by shipping method id.
        /// </summary>
        /// <param name="shippingMethodId">The shipping method id.</param>
        /// <returns></returns>
		public static ShippingMethodDto GetShippingMethod(Guid shippingMethodId)
		{
			return GetShippingMethod(shippingMethodId, false);
		}

		/// <summary>
		/// Returns shippping package by shipping option id.
		/// </summary>
		/// <returns></returns>
		public static ShippingMethodDto GetShippingPackages()
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingPackage]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.DataSet = new ShippingMethodDto();
			cmd.TableMapping = DataHelper.MapTables("Package");

			DataResult results = DataService.LoadDataSet(cmd);

			return (ShippingMethodDto)results.DataSet;
		}

		/// <summary>
		/// Returns shippping package by packageId.
		/// </summary>
		/// <param name="packageId">The packageId.</param>
		/// <returns></returns>
		public static ShippingMethodDto GetShippingPackage(int packageId)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingPackage_PackageId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("PackageId", packageId, DataParameterType.Int));
			cmd.DataSet = new ShippingMethodDto();
			cmd.TableMapping = DataHelper.MapTables("Package");

			DataResult results = DataService.LoadDataSet(cmd);

			return (ShippingMethodDto)results.DataSet;
		}

		/// <summary>
		/// Returns shippping package by package name.
		/// </summary>
		/// <param name="name">The package name.</param>
		/// <returns></returns>
		public static ShippingMethodDto GetShippingPackage(string name)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingPackage_Name]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 100));
			cmd.DataSet = new ShippingMethodDto();
			cmd.TableMapping = DataHelper.MapTables("Package");

			DataResult results = DataService.LoadDataSet(cmd);

			return (ShippingMethodDto)results.DataSet;
		}

		/// <summary>
		/// Gets the ShippingPackage id by name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static int GetShippingPackageIdByName(string name)
		{
			ShippingMethodDto dto = GetShippingPackages();

			int packageId = -1;

			if (dto != null)
			{
				ShippingMethodDto.PackageRow[] packageRows = (ShippingMethodDto.PackageRow[])dto.Package.Select(String.Format("Name='{0}'", name));
				if (packageRows != null && packageRows.Length > 0)
					packageId = packageRows[0].PackageId;
			}

			return packageId;
		}

		/// <summary>
		/// Returns shippping method cases by shipping method id.
		/// </summary>
		/// <returns></returns>
		public static DataTable GetShippingMethodCases(Guid shippingMethodId)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingMethod_GetCases]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ShippingMethodId", shippingMethodId, DataParameterType.UniqueIdentifier));

			return DataService.LoadTable(cmd);
		}

		/// <summary>
		/// Returns shippping method cases by shipping method id.
		/// </summary>
		/// <returns></returns>
		public static DataTable GetShippingMethodCases(Guid shippingMethodId, string countryCode, string stateProvinceCode, string zipPostalCode, string district, string county, string city, decimal? total)
		{
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_ShippingMethod_GetCases]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ShippingMethodId", shippingMethodId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("CountryCode", String.IsNullOrEmpty(countryCode) ? null : countryCode, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("Total", total, DataParameterType.Money));
			cmd.Parameters.Add(new DataParameter("StateProvinceCode", String.IsNullOrEmpty(stateProvinceCode) ? null : stateProvinceCode, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("ZipPostalCode", String.IsNullOrEmpty(zipPostalCode) ? null : zipPostalCode, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("District", String.IsNullOrEmpty(district) ? null : district, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("County", String.IsNullOrEmpty(county) ? null : county, DataParameterType.NVarChar, 50));
			cmd.Parameters.Add(new DataParameter("City", String.IsNullOrEmpty(city) ? null : city, DataParameterType.NVarChar, 50));

			return DataService.LoadTable(cmd);
		}

        #endregion

        #region Edit Shipping Functions
        /// <summary>
        /// Saves the shipping.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SaveShipping(ShippingMethodDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("ShippingMethodDto can not be null"));

			// TODO: Check if user is allowed to perform this operation
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "ShippingOption", "ShippingOptionParameter", "Package", "ShippingPackage", "ShippingMethod", "ShippingMethodParameter", "ShippingMethodCase", "ShippingCountry", "ShippingRegion", "ShippingPaymentRestriction");
				scope.Complete();
			}
        }

		/// <summary>
		/// Saves the package.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SavePackage(ShippingMethodDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("ShippingMethodDto can not be null"));

			// TODO: Check if user is allowed to perform this operation
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "Package");
				scope.Complete();
			}
		}
        #endregion
    }
}
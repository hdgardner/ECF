using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
    /// <summary>
    /// Implements operations for the tax manager.
    /// </summary>
	public static class TaxManager
	{
		#region Tax Functions
		/// <summary>
		/// Gets the taxes.
		/// </summary>
		/// <returns></returns>
		public static TaxDto GetTaxDto(TaxType? type)
		{
			return GetTaxDto(type, null);
		}

		/// <summary>
		/// Gets the taxes.
		/// </summary>
		/// <returns></returns>
		public static TaxDto GetTaxDto(TaxType? type, string languageCode)
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_Tax]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			if (type.HasValue)
				cmd.Parameters.Add(new DataParameter("TaxType", type.Value.GetHashCode(), DataParameterType.Int));
			cmd.DataSet = new TaxDto();
			cmd.TableMapping = DataHelper.MapTables("Tax", "TaxLanguage", "TaxValue");

			DataResult results = DataService.LoadDataSet(cmd);

			return (TaxDto)results.DataSet;
		}

		/// <summary>
		/// Gets the tax by id.
		/// </summary>
		/// <returns></returns>
		public static TaxDto GetTax(int taxId)
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_Tax_TaxId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("TaxId", taxId, DataParameterType.Int));
			cmd.DataSet = new TaxDto();
			cmd.TableMapping = DataHelper.MapTables("Tax", "TaxLanguage", "TaxValue");

			DataResult results = DataService.LoadDataSet(cmd);

			return (TaxDto)results.DataSet;
		}

		/// <summary>
		/// Gets the tax by name.
		/// </summary>
		/// <returns></returns>
		public static TaxDto GetTax(string name)
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_Tax_TaxName]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 50));
			cmd.DataSet = new TaxDto();
			cmd.TableMapping = DataHelper.MapTables("Tax", "TaxLanguage", "TaxValue");

			DataResult results = DataService.LoadDataSet(cmd);

			return (TaxDto)results.DataSet;
		}

        /// <summary>
        /// Gets the taxes.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="taxCategory">The tax category.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="stateProvinceCode">The state province code.</param>
        /// <param name="zipPostalCode">The zip postal code.</param>
        /// <param name="district">The district.</param>
        /// <param name="county">The county.</param>
        /// <param name="city">The city.</param>
        /// <returns></returns>
        public static DataTable GetTaxes(
            Guid siteId,
            string taxCategory,
            string languageCode,
            string countryCode,
            string stateProvinceCode,
            string zipPostalCode,
            string district,
            string county,
            string city)
        {
            DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
            cmd.CommandText = String.Format("ecf_GetTaxes");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("SiteId", siteId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("TaxCategory", taxCategory, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("LanguageCode", languageCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("CountryCode", countryCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("StateProvinceCode", stateProvinceCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("ZipPostalCode", zipPostalCode, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("District", district, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("County", county, DataParameterType.NVarChar, 50));
            cmd.Parameters.Add(new DataParameter("City", city, DataParameterType.NVarChar, 50));

            return DataService.LoadTable(cmd);
        }
		#endregion

		#region Edit Tax Functions
		/// <summary>
		/// Saves changes in TaxDto.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SaveTax(TaxDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("TaxDto can not be null"));

			// TODO: Check if user is allowed to perform this operation
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "Tax", "TaxLanguage", "TaxValue");
				scope.Complete();
			}
		}

		#endregion
	}
}
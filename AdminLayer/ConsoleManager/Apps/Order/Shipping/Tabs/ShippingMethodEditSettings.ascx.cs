using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	/// <summary>
	///		Summary description for ShippingMethodEditSettings.
	/// </summary>
	public partial class ShippingMethodEditSettings : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _ShippingGatewayConfigurationBasePath = "~/Apps/Order/Shipping/Plugins/";
		private const string _ShippingGatewayConfigurationFileName = "/ConfigureShipping.ascx";

		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
		public string LanguageCode
		{
			get
			{
				if (Parameters["lang"] != null)
					return Parameters["lang"];
				else
					return String.Empty;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblCountries.Text = RM.GetString("SHIPPINGMETHODEDIT_RESTRICTED_COUNTRIES");
			lblRegions.Text = RM.GetString("SHIPPINGMETHODEDIT_RESTRICTED_REGIONS");

			if (!this.IsPostBack)
				BindForm();
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingMethod.Count > 0)
			{
				try
				{
					ShippingMethodDto.ShippingMethodRow shippingRow = _ShippingMethodDto.ShippingMethod[0];
					
					// set initial state of dual lists
					BindCountriesList(shippingRow);
					BindRegionsList(shippingRow);
					BindPaymentsList(shippingRow);
				}
				catch (Exception ex)
				{
					DisplayErrorMessage("Error during binding gateway settings: " + ex.Message);
					return;
				}
			}
			else
			{
				BindCountriesList(null);
				BindRegionsList(null);
				BindPaymentsList(null);
			}
		}

        /// <summary>
        /// Binds the countries list.
        /// </summary>
        /// <param name="shippingRow">The shipping row.</param>
		private void BindCountriesList(ShippingMethodDto.ShippingMethodRow shippingRow)
		{
			List<CountryDto.CountryRow> leftCountries = new List<CountryDto.CountryRow>();
			List<CountryDto.CountryRow> rightCountries = new List<CountryDto.CountryRow>();

			CountryDto dto = CountryManager.GetCountries(true);

			bool allToLeft = false; // if true, then add all countries to the left list

			if (shippingRow != null)
			{
				ShippingMethodDto.ShippingCountryRow[] restrictedCountryRows = shippingRow.GetShippingCountryRows();
				if (restrictedCountryRows != null && restrictedCountryRows.Length > 0)
				{
					foreach (CountryDto.CountryRow countryRow in dto.Country)
					{
						bool found = false;
						foreach (ShippingMethodDto.ShippingCountryRow restrictedCountryRow in restrictedCountryRows)
						{
							if (countryRow.CountryId == restrictedCountryRow.CountryId)
							{
								found = true;
								break;
							}
						}

						if (found)
							rightCountries.Add(countryRow);
						else
							leftCountries.Add(countryRow);
					}

					CountryList.LeftDataSource = leftCountries;
					CountryList.RightDataSource = rightCountries;
				}
				else
					// add all countries to the left list
					allToLeft = true;
			}
			else
				allToLeft = true;

			if (allToLeft)
				// add all countries to the left list
				CountryList.LeftDataSource = dto.Country;
			
			CountryList.DataBind();
		}

        /// <summary>
        /// Binds the regions list.
        /// </summary>
        /// <param name="shippingRow">The shipping row.</param>
		private void BindRegionsList(ShippingMethodDto.ShippingMethodRow shippingRow)
		{
			List<CountryDto.StateProvinceRow> leftRegions = new List<CountryDto.StateProvinceRow>();
			List<CountryDto.StateProvinceRow> rightRegions = new List<CountryDto.StateProvinceRow>();

			CountryDto dto = CountryManager.GetCountries(true);

			bool allToLeft = false; // if true, then add all regions to the left list

			if (shippingRow != null)
			{
				ShippingMethodDto.ShippingRegionRow[] restrictedRegionRows = shippingRow.GetShippingRegionRows();
				if (restrictedRegionRows != null && restrictedRegionRows.Length > 0)
				{
					foreach (CountryDto.StateProvinceRow stateProvinceRow in dto.StateProvince)
					{
						bool found = false;
						foreach (ShippingMethodDto.ShippingRegionRow restrictedRegionRow in restrictedRegionRows)
						{
							if (stateProvinceRow.StateProvinceId == restrictedRegionRow.StateProvinceId)
							{
								found = true;
								break;
							}
						}

						if (found)
							rightRegions.Add(stateProvinceRow);
						else
							leftRegions.Add(stateProvinceRow);
					}

					RegionList.LeftDataSource = leftRegions;
					RegionList.RightDataSource = rightRegions;
				}
				else
					// add all regions to the left list
					allToLeft = true;
			}
			else
				allToLeft = true;

			if (allToLeft)
				// add all regions to the left list
				RegionList.LeftDataSource = dto.StateProvince;
			
			RegionList.DataBind();
		}

        /// <summary>
        /// Binds the payments list.
        /// </summary>
        /// <param name="shippingRow">The shipping row.</param>
		private void BindPaymentsList(ShippingMethodDto.ShippingMethodRow shippingRow)
		{
			List<PaymentMethodDto.PaymentMethodRow> leftPayments = new List<PaymentMethodDto.PaymentMethodRow>();
			List<PaymentMethodDto.PaymentMethodRow> rightPayments = new List<PaymentMethodDto.PaymentMethodRow>();

			PaymentMethodDto dto = PaymentManager.GetPaymentMethods(shippingRow != null ? shippingRow.LanguageId : LanguageCode, true);

			bool allToLeft = false; // if true, then add all payments to the left list

			if (shippingRow != null)
			{
				ShippingMethodDto.ShippingPaymentRestrictionRow[] restrictedPaymentRows = shippingRow.GetShippingPaymentRestrictionRows();
				if (restrictedPaymentRows != null && restrictedPaymentRows.Length > 0)
				{
					foreach (PaymentMethodDto.PaymentMethodRow paymentMethodRow in dto.PaymentMethod)
					{
						bool found = false;
						foreach (ShippingMethodDto.ShippingPaymentRestrictionRow restrictedPaymentRow in restrictedPaymentRows)
						{
							if (paymentMethodRow.PaymentMethodId == restrictedPaymentRow.PaymentMethodId)
							{
								found = true;
								break;
							}
						}

						if (found)
							rightPayments.Add(paymentMethodRow);
						else
							leftPayments.Add(paymentMethodRow);
					}

					PaymentsList.LeftDataSource = leftPayments;
					PaymentsList.RightDataSource = rightPayments;
				}
				else
					// add all payments to the left list
					allToLeft = true;
			}
			else
				allToLeft = true;

			if (allToLeft)
				// add all payments to the left list
				PaymentsList.LeftDataSource = dto.PaymentMethod;

			PaymentsList.DataBind();
		}

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_ShippingMethodDto = (ShippingMethodDto)context[_ShippingMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			ShippingMethodDto dto = (ShippingMethodDto)context[_ShippingMethodDtoString];

			if (dto == null)
				// dto must be created in base shipping control that holds tabs
				return;

			ShippingMethodDto.ShippingMethodRow shippingRow = null;

			// create the row if it doesn't exist; or update its modified date if it exists
			if (dto.ShippingMethod.Count > 0)
				shippingRow = dto.ShippingMethod[0];
			else
				return;

			// 1. populate countries

			// a). delete rows from dto that are not selected
			foreach (ShippingMethodDto.ShippingCountryRow row in shippingRow.GetShippingCountryRows())
			{
				bool found = false;
				foreach (ListItem item in CountryList.RightItems)
				{
					if (String.Compare(item.Value, row.CountryId.ToString(), true) == 0)
					{
						found = true;
						break;
					}
				}

				if (!found)
					row.Delete();
			}

			// b). add selected rows to dto
			foreach (ListItem item in CountryList.RightItems)
			{
				bool exists = false;
				foreach (ShippingMethodDto.ShippingCountryRow row in shippingRow.GetShippingCountryRows())
				{
					if (String.Compare(item.Value, row.CountryId.ToString(), true) == 0)
					{
						exists = true;
						break;
					}
				}

				if (!exists)
				{
					ShippingMethodDto.ShippingCountryRow restrictedRow = dto.ShippingCountry.NewShippingCountryRow();
					restrictedRow.CountryId = Int32.Parse(item.Value);
					restrictedRow.ShippingMethodId = shippingRow.ShippingMethodId;

					// add the row to the dto
					dto.ShippingCountry.Rows.Add(restrictedRow);
				}
			}


			// 2. populate regions

			// a). delete rows from dto that are not selected
			foreach (ShippingMethodDto.ShippingRegionRow row in shippingRow.GetShippingRegionRows())
			{
				bool found = false;
				foreach (ListItem item in RegionList.RightItems)
				{
					if (String.Compare(item.Value, row.StateProvinceId.ToString(), true) == 0)
					{
						found = true;
						break;
					}
				}

				if (!found)
					row.Delete();
			}

			// b). add selected rows to dto
			foreach (ListItem item in RegionList.RightItems)
			{
				bool exists = false;
				foreach (ShippingMethodDto.ShippingRegionRow row in shippingRow.GetShippingRegionRows())
				{
					if (String.Compare(item.Value, row.StateProvinceId.ToString(), true) == 0)
					{
						exists = true;
						break;
					}
				}

				if (!exists)
				{
					ShippingMethodDto.ShippingRegionRow restrictedRow = dto.ShippingRegion.NewShippingRegionRow();
					restrictedRow.StateProvinceId = Int32.Parse(item.Value);
					restrictedRow.ShippingMethodId = shippingRow.ShippingMethodId;

					// add the row to the dto
					dto.ShippingRegion.Rows.Add(restrictedRow);
				}
			}

			// 3. populate payments restrictions

			// a). delete rows from dto that are not selected
			foreach (ShippingMethodDto.ShippingPaymentRestrictionRow row in shippingRow.GetShippingPaymentRestrictionRows())
			{
				bool found = false;
				foreach (ListItem item in PaymentsList.RightItems)
				{
					if (String.Compare(item.Value, row.PaymentMethodId.ToString(), true) == 0 && !row.RestrictShippingMethods)
					{
						found = true;
						break;
					}
				}

				if (!found)
					row.Delete();
			}

			// b). add selected rows to dto
			foreach (ListItem item in PaymentsList.RightItems)
			{
				bool exists = false;
				foreach (ShippingMethodDto.ShippingPaymentRestrictionRow row in shippingRow.GetShippingPaymentRestrictionRows())
				{
					if (String.Compare(item.Value, row.PaymentMethodId.ToString(), true) == 0 && !row.RestrictShippingMethods)
					{
						exists = true;
						break;
					}
				}

				if (!exists)
				{
					ShippingMethodDto.ShippingPaymentRestrictionRow restrictedRow = dto.ShippingPaymentRestriction.NewShippingPaymentRestrictionRow();
					restrictedRow.PaymentMethodId = new Guid(item.Value);
					restrictedRow.ShippingMethodId = shippingRow.ShippingMethodId;
					restrictedRow.RestrictShippingMethods = false;

					// add the row to the dto
					dto.ShippingPaymentRestriction.Rows.Add(restrictedRow);
				}
			}
		}
		#endregion
	}
}
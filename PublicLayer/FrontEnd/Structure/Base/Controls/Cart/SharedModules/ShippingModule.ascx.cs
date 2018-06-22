using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders;
using Mediachase.Cms;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.eCF.PublicStore.SharedModules
{
    /// <summary>
    /// Shipping module displays shipping rates for specified shipping package. It provides a way to pick
    /// a rate from the list.
    /// </summary>
    public partial class ShippingModule : BaseStoreUserControl
    {
        private CartHelper _CartHelper = null;
        /// <summary>
        /// Gets or sets the cart helper.
        /// </summary>
        /// <value>The cart helper.</value>
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }

        private OrderAddress _OrderAddress = null;
        /// <summary>
        /// Gets or sets the order address.
        /// </summary>
        /// <value>The order address.</value>
        public OrderAddress OrderAddress { get { return _OrderAddress; } set { _OrderAddress = value; } }
        
        private ShippingRate[] _ShippingRates = null;


        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                if (this.OrderAddress != null)
                    BindShippingRates();

                base.CreateChildControls();
                ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Binds the shipping rates.
        /// </summary>
        private void BindShippingRates()
        {
            CountryDto country;

            bool countryFound = false;
            bool regionFound = false;
            int shippingCountryId = -1;
            int shippingStateProvinceId = -1;
            bool isRestricted;

            //get the country id for the order address
            country = CountryManager.GetCountry(OrderAddress.CountryCode, true);
            if (country.Country != null && country.Country.Rows.Count > 0)
            {
                shippingCountryId = country.Country[0].CountryId;
                countryFound = true;
            }

            //get the state id for the order address
            CountryDto regions = CountryManager.GetCountries(true);
            if (regions.StateProvince != null && regions.StateProvince.Count > 0)
            {
                foreach (CountryDto.StateProvinceRow stateProvinceRow in regions.StateProvince)
                {
                    if (stateProvinceRow.Name == OrderAddress.State || stateProvinceRow.Name == OrderAddress.RegionCode)
                    {
                        shippingStateProvinceId = stateProvinceRow.StateProvinceId;
                        regionFound = true;
                        break;
                    }
                }
            }

            //Get the list of all shipping methods to be filtered
            ShippingMethodDto methods = ShippingManager.GetShippingMethods(CMSContext.Current.LanguageName); //Thread.CurrentThread.CurrentUICulture.Name);

            // filter the list for only methods that apply to this particular cart's shipping address
            List<ShippingMethodDto.ShippingMethodRow> shippingRows = new List<ShippingMethodDto.ShippingMethodRow>();

            if (countryFound || regionFound)
            {
                foreach (ShippingMethodDto.ShippingMethodRow method in methods.ShippingMethod.Rows)
                {
                    isRestricted = false;

                    if (countryFound)
                    {
                        //first check the restricted countries
                        ShippingMethodDto.ShippingCountryRow[] paymentCountryRestrictions = method.GetShippingCountryRows();
                        if (paymentCountryRestrictions != null && paymentCountryRestrictions.Length > 0)
                        {
                            foreach (ShippingMethodDto.ShippingCountryRow restrictedCountryRow in paymentCountryRestrictions)
                            {
                                if (restrictedCountryRow.CountryId == shippingCountryId)
                                {
                                    isRestricted = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (regionFound)
                    {
                        //now check for restricted regions
                        ShippingMethodDto.ShippingRegionRow[] restrictedRegionRows = method.GetShippingRegionRows();
                        if (restrictedRegionRows != null && restrictedRegionRows.Length > 0)
                        {
                            foreach (ShippingMethodDto.ShippingRegionRow restrictedRegionRow in restrictedRegionRows)
                            {
                                if (shippingStateProvinceId == restrictedRegionRow.StateProvinceId) 
                                {
                                    isRestricted = true;
                                    break;
                                }
                            }
                        }
                    }

                    //if the method has not been excluded based on country or region, including in shipping collection
                    if (!isRestricted)
                        shippingRows.Add(method);
                }
            }
            else
            {
                //just add all of the rows to the generic list
                foreach (ShippingMethodDto.ShippingMethodRow method in methods.ShippingMethod.Rows)
                    shippingRows.Add(method);
            }

            // request rates, make sure we request rates not bound to selected delivery method
            if (_ShippingRates == null)
            {
                List<ShippingRate> list = new List<ShippingRate>();
                foreach (ShippingMethodDto.ShippingMethodRow row in shippingRows)
                {
                    // Check if package contains shippable items, if it does not use the default shipping method instead of the one specified
                    Type type = Type.GetType(row.ShippingOptionRow.ClassName);
                    if (type == null)
                    {
                        throw new TypeInitializationException(row.ShippingOptionRow.ClassName, null);
                    }
                    string message = String.Empty;
                    IShippingGateway provider = (IShippingGateway)Activator.CreateInstance(type);

                    List<LineItem> items = new List<LineItem>();
                    foreach(LineItem lineItem in CartHelper.LineItems)
                    {
                        if(lineItem.ShippingAddressId == OrderAddress.Name.ToString())
                            items.Add(lineItem);
                    }

                    if (items.Count > 0)
                        list.Add(provider.GetRate(row.ShippingMethodId, items.ToArray(), ref message));
                }

                _ShippingRates = list.ToArray();
            }

            if (_ShippingRates == null)
                return;

            string selectedValue = String.Empty;

            /*
            if (this.ShipmentPackage.Details.DeliveryMethod != null)
                selectedValue = ShipmentPackage.Details.DeliveryMethod.StoreMethod + ";" + ShipmentPackage.Details.DeliveryMethod.ServiceSpeed;
             * */

            if (ShippingRatesList.Items.Count > 0)
            {
                selectedValue = ShippingRatesList.SelectedValue;
                ShippingRatesList.Items.Clear();
            }

            // Bind control
            string lowestOptionValue = String.Empty;
            decimal lowestPrice = Decimal.MinusOne; // set to -1 initially
            foreach (ShippingRate listItem in _ShippingRates)
            {
                ListItem item = new ListItem(String.Format("{1} - {0}", listItem.Name, CurrencyFormatter.FormatCurrency(listItem.Price, listItem.CurrencyCode)), listItem.Id.ToString());

                if (selectedValue.CompareTo(item.Value) == 0)
                    item.Selected = true;

                if (listItem.Price < lowestPrice || lowestPrice == Decimal.MinusOne)
                {
                    lowestPrice = listItem.Price;
                    lowestOptionValue = item.Value;
                }

                ShippingRatesList.Items.Add(item);
            }

            // Select the least expensive option if none selected yet
            if (String.IsNullOrEmpty(selectedValue))
            {
                foreach (ListItem listItem in ShippingRatesList.Items)
                {
                    if (lowestOptionValue.CompareTo(listItem.Value) == 0)
                        listItem.Selected = true;
                }
            }

            // if nothing is selected make sure the very first element is selected
            // also save that selection to the current order
            if (ShippingRatesList.SelectedIndex < 0 && ShippingRatesList.Items.Count > 0)
            {
                ShippingRatesList.SelectedIndex = 0;
            }

            // update the selected delivery option
            UpdateDeliveryMethod();

            // Bind it
            ShippingRatesList.DataBind();

            // Hide if no items available
            if (ShippingRatesList.Items.Count == 0)
                this.Visible = false;
            else
                this.Visible = true;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ShippingRatesList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void ShippingRatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDeliveryMethod();
        }

        /// <summary>
        /// Updates the delivery method.
        /// </summary>
        private void UpdateDeliveryMethod()
        {
            ShippingMethodDto methods = ShippingManager.GetShippingMethods(Thread.CurrentThread.CurrentUICulture.Name);

            foreach (LineItem lineItem in CartHelper.LineItems)
            {
				if (String.Compare(lineItem.ShippingAddressId, OrderAddress.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (ShippingRatesList.Items.Count > 0)
					{
						ShippingMethodDto.ShippingMethodRow row = methods.ShippingMethod.FindByShippingMethodId(new Guid(ShippingRatesList.SelectedValue));
						lineItem.ShippingMethodName = row.DisplayName;
						lineItem.ShippingMethodId = row.ShippingMethodId;
					}
				}
            }
        }
    }
}

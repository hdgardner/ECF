using System;
using System.Text;
using System.Net;
using System.Collections;
using System.Globalization;
using Mediachase.Commerce.Orders;
using System.Threading;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using System.Data;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Commerce.Plugins.Shipping
{
	/// <summary>
	/// Summary description for Generic.
	/// </summary>
	public class WeightJurisdictionGateway : IShippingGateway
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WeightJurisdictionGateway"/> class.
		/// </summary>
		public WeightJurisdictionGateway()
		{
		}

		#region IShippingGateway Members

		/// <summary>
		/// Returns the package option array when method id and package that needs to be send is passed.
		/// Use passed message string to pass errors back to the application if any occured.
		/// </summary>
		/// <param name="methodId"></param>
		/// <param name="items">The items.</param>
		/// <param name="message">The message.</param>
		/// <returns>empty array if no results found</returns>
		public ShippingRate GetRate(Guid methodId, LineItem[] items, ref string message)
		{
			if (items == null || items.Length == 0)
				return null;

			ShippingRate rate = null;

			ShippingMethodDto methodDto = ShippingManager.GetShippingMethod(methodId);

			if (methodDto != null && methodDto.ShippingMethod.Count > 0)
			{
				ShippingMethodDto.ShippingMethodRow method = methodDto.ShippingMethod[0];

				if (items[0].Parent != null && items[0].Parent.Parent != null)
				{
					// find shipping address
					OrderAddress shippingAddress = null;
					OrderAddressCollection addresses = items[0].Parent.Parent.OrderAddresses;
					foreach (OrderAddress address in addresses)
					{
						if (String.Compare(address.Name, items[0].ShippingAddressId, StringComparison.OrdinalIgnoreCase) == 0)
						{
							shippingAddress = address;
							break;
						}
					}

					if (shippingAddress != null)
					{
						// calculate total item weight
						decimal weight = 0;

						foreach (LineItem item in items)
						{
							if (!String.IsNullOrEmpty(item.CatalogEntryId))
							{
								CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntryDto(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

								if (dto.CatalogEntry.Count > 0)
								{
									CatalogEntryDto.VariationRow[] variationRows = dto.CatalogEntry[0].GetVariationRows();
									if (variationRows != null && variationRows.Length > 0)
										weight += (decimal)variationRows[0].Weight * item.Quantity;
								}
							}
						}

						// get shipping method cases
						DataTable casesTable = ShippingManager.GetShippingMethodCases(methodId, shippingAddress.CountryCode, shippingAddress.State,
							shippingAddress.PostalCode, shippingAddress.RegionCode, null, shippingAddress.City, weight);

                        // get the row with the greatest weight ; the rows all contain weights less than the order weight
                        double? shippingPrice = null;
                        double maxWeight = 0;
                        object objprice;
                        foreach (DataRow caseRow in casesTable.Rows)
                        {
                            object obj = caseRow["Total"];
                            if (obj != null && obj != DBNull.Value)
                            {
                                double rowweight = (double)obj;
                                if (maxWeight < rowweight)
                                {
                                    maxWeight = rowweight;
                                    objprice = caseRow["Charge"];
                                    if (objprice != null && objprice != DBNull.Value)
                                    {
                                        shippingPrice = (double)objprice;
                                    }
                                }
                                else if (maxWeight == rowweight)
                                {
                                    //if the weight is the same, take the lower price
                                    objprice = caseRow["Charge"];
                                    if (objprice != null && objprice != DBNull.Value)
                                    {
                                        double newprice = (double)objprice;
                                        if ((shippingPrice == null) || (newprice < shippingPrice))
                                            shippingPrice = newprice;
                                    }
                                }
                            }
                        }

                        if (shippingPrice == null)
                            shippingPrice = 0;

                        rate = new ShippingRate(methodId, method.DisplayName, method.BasePrice + (decimal)shippingPrice, method.Currency);
					}
				}
			}
			else
			{
				message = "The shipping method could not be loaded.";
				return null;
			}

			return rate;
		}

		#endregion
	}
}

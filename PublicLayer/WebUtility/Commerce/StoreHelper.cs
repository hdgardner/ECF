using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Web;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Cms;
using Mediachase.Commerce.Catalog.Dto;
using System.Web.Security;
using System.Globalization;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog.Managers;
using System.Collections.Specialized;
using Mediachase.Cms.Util;
using System.Collections;

namespace Mediachase.Cms.WebUtility.Commerce
{
    /// <summary>
    /// Store helper.
    /// </summary>
    public class StoreHelper
    {
        /// <summary>
        /// Loads the payment plugin.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public static Control LoadPaymentPlugin(UserControl control, string keyword)
        {
            System.Web.UI.Control paymentCtrl = null;
            string path = String.Concat("~/Structure/Base/Controls/Cart/PaymentGateways/", keyword, "/PaymentMethod.ascx");
            if (File.Exists(HttpContext.Current.Server.MapPath(path)))
            {
                paymentCtrl = control.LoadControl(path);
            }
            else
            {
                // Control not found, use generic one
				paymentCtrl = control.LoadControl(String.Format("~/Structure/Base/Controls/Cart/PaymentGateways/Generic/PaymentMethod.ascx"));
            }

            paymentCtrl.ID = keyword;
            return paymentCtrl;
        }

        /// <summary>
        /// Returns address formatted as a string
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        public static string GetAddressString(OrderAddress info)
        {
            if (info == null)
                return String.Empty;

            string strAddress = String.Empty;

            strAddress = info.FirstName;

            if (!String.IsNullOrEmpty(info.LastName))
                strAddress += " " + info.LastName + ", ";

            if (!String.IsNullOrEmpty(info.Organization))
                strAddress += info.Organization + ", ";

            if (!String.IsNullOrEmpty(info.Line1))
                strAddress += info.Line1 + ", ";

            if (!String.IsNullOrEmpty(info.Line2))
                strAddress += info.Line2 + ", ";

            if (!String.IsNullOrEmpty(info.City))
                strAddress += info.City + ", ";

            if (!String.IsNullOrEmpty(info.State))
                strAddress += info.State + ", ";

            if (!String.IsNullOrEmpty(info.CountryName))
                strAddress += info.CountryName + ", ";

            if (!String.IsNullOrEmpty(info.PostalCode))
                strAddress += info.PostalCode + ", ";

            if (!String.IsNullOrEmpty(info.DaytimePhoneNumber))
                strAddress += info.DaytimePhoneNumber;

            if (strAddress.EndsWith(", "))
                strAddress = strAddress.Remove(strAddress.LastIndexOf(", "));

            if (strAddress.StartsWith(", "))
                strAddress = strAddress.Remove(strAddress.IndexOf(", "));

            return strAddress.Trim();
        }

        /// <summary>
        /// Gets the quantity as string.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public static string GetQuantityAsString(decimal quantity)
        {
            if (Decimal.Round(quantity, 0) != quantity)
            {
                return quantity.ToString();
            }
            else
            {
                return Convert.ToInt32(quantity).ToString();
            }
        }

		/// <summary>
		/// Checks if customer address collection already contains the specified address.
		/// </summary>
		/// <param name="collection">Customer addresses collection (Profile.Account.Addresses).</param>
		/// <param name="address">Address to check.</param>
		/// <returns>True, if address is already in the collection.</returns>
		/// <remarks>Only address' properties are checked (like first, last name, city, state,...). Address name and addressId are ignored.
		/// </remarks>
		public static bool IsAddressInCollection(CustomerAddressCollection collection, CustomerAddress address)
		{
			if (address == null)
				return false;

			bool found = false;

			foreach (CustomerAddress tmpAddress in collection)
			{
				if (CheckAddressesEquality(tmpAddress, address))
				{
					found = true;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// Checks if 2 customer addresses are the same (have same first, last names, city, state,...).
		/// Address ids and names are ignored.
		/// </summary>
		/// <param name="address1"></param>
		/// <param name="address2"></param>
		/// <returns></returns>
		public static bool CheckAddressesEquality(CustomerAddress address1, CustomerAddress address2)
		{
			bool addressesEqual = false;

			if (address1 == null && address2 == null)
				addressesEqual = true;
			else if (address1 == null && address2 != null)
				addressesEqual = false;
			else if (address1 != null && address2 == null)
				addressesEqual = false;
			else if (address1 != null && address2 != null)
			{
				addressesEqual = (String.Compare(address1.City, address2.City, true) == 0) &&
					(String.Compare(address1.CountryCode, address2.CountryCode) == 0) &&
					(String.Compare(address1.CountryName, address2.CountryName) == 0) &&
					(String.Compare(address1.DaytimePhoneNumber, address2.DaytimePhoneNumber) == 0) &&
					(String.Compare(address1.EveningPhoneNumber, address2.EveningPhoneNumber) == 0) &&
					(String.Compare(address1.FaxNumber, address2.FaxNumber) == 0) &&
					(String.Compare(address1.FirstName, address2.FirstName) == 0) &&
					(String.Compare(address1.LastName, address2.LastName) == 0) &&
					(String.Compare(address1.Line1, address2.Line1) == 0) &&
					(String.Compare(address1.Line2, address2.Line2) == 0) &&
					(String.Compare(address1.Organization, address2.Organization) == 0) &&
					(String.Compare(address1.RegionCode, address2.RegionCode) == 0) &&
					(String.Compare(address1.RegionName, address2.RegionName) == 0) &&
					(String.Compare(address1.State, address2.State) == 0);
			}

			return addressesEqual;
		}

        /// <summary>
        /// Converts to customer address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static CustomerAddress ConvertToCustomerAddress(OrderAddress address)
        {
            CustomerAddress newAddress = new CustomerAddress();
            newAddress.City = address.City;
            newAddress.CountryCode = address.CountryCode;
            newAddress.CountryName = address.CountryName;
            newAddress.DaytimePhoneNumber = address.DaytimePhoneNumber;
            newAddress.Email = address.Email;
            newAddress.EveningPhoneNumber = address.EveningPhoneNumber;
            newAddress.FaxNumber = address.FaxNumber;
            newAddress.FirstName = address.FirstName;
            newAddress.LastName = address.LastName;
            newAddress.Line1 = address.Line1;
            newAddress.Line2 = address.Line2;
            newAddress.Name = address.Name;
            newAddress.Organization = address.Organization;
            newAddress.PostalCode = address.PostalCode;
            newAddress.RegionName = address.RegionName;
            newAddress.RegionCode = address.RegionCode;
            newAddress.State = address.State;
            return newAddress;
        }

        /// <summary>
        /// Converts to order address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static OrderAddress ConvertToOrderAddress(CustomerAddress address)
        {
            OrderAddress newAddress = new OrderAddress();
            newAddress.City = address.City;
            newAddress.CountryCode = address.CountryCode;
            newAddress.CountryName = address.CountryName;
            newAddress.DaytimePhoneNumber = address.DaytimePhoneNumber;
            newAddress.Email = address.Email;
            newAddress.EveningPhoneNumber = address.EveningPhoneNumber;
            newAddress.FaxNumber = address.FaxNumber;
            newAddress.FirstName = address.FirstName;
            newAddress.LastName = address.LastName;
            newAddress.Line1 = address.Line1;
            newAddress.Line2 = address.Line2;
            newAddress.Name = address.Name;
            newAddress.Organization = address.Organization;
            newAddress.PostalCode = address.PostalCode;
            newAddress.RegionName = address.RegionName;
            newAddress.RegionCode = address.RegionCode;
            newAddress.State = address.State;
            return newAddress;
        }

        /// <summary>
        /// Gets the entry URL.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [Obsolete("This is an obsolete method and offers poor performance")]
        public static string GetEntryUrl(string id)
        {
            int entryid = 0;
            string url = String.Empty;

            if (Int32.TryParse(id, out entryid))
            {
                Entry entry = CatalogContext.Current.GetCatalogEntry(entryid);
                if (entry.SeoInfo != null && entry.SeoInfo.Length > 0)
                {
                    foreach (Seo seo in entry.SeoInfo)
                    {
                        if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            url = "~/" + seo.Uri;
                            break;
                        }
                    }
                }
            }
            else // passed code value
            {
                Entry entry = CatalogContext.Current.GetCatalogEntry(id);
                if (entry.SeoInfo != null && entry.SeoInfo.Length > 0)
                {
                    foreach (Seo seo in entry.SeoInfo)
                    {
                        if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            url = "~/" + seo.Uri;
                            break;
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(url))
                return CMSContext.Current.ResolveUrl(NavigationManager.GetUrl("EntryView", "ec", id));
            else
                return CMSContext.Current.ResolveUrl(url);
        }

        /// <summary>
        /// Gets the node URL.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [Obsolete("This is an obsolete method and offers poor performance")]
        public static string GetNodeUrl(string id)
        {
            string url = String.Empty;

            CatalogNodeDto node = CatalogContext.Current.GetCatalogNodeDto(id);
            if (node.CatalogItemSeo.Count > 0)
            {
                foreach (CatalogNodeDto.CatalogItemSeoRow seo in node.CatalogItemSeo.Rows)
                {
                    if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        url = "~/" + seo.Uri;
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(url))
                return CMSContext.Current.ResolveUrl(NavigationManager.GetUrl("NodeView", "nc", id));
            else
                return CMSContext.Current.ResolveUrl(url);
        }

        /// <summary>
        /// Gets the node URL.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static string GetNodeUrl(CatalogNode node)
        {
            string url = String.Empty;
            Seo seo = GetLanguageSeo(node.SeoInfo);

            if (seo != null)
                url = "~/" + seo.Uri;

            if (String.IsNullOrEmpty(url))
                return CMSContext.Current.ResolveUrl(NavigationManager.GetUrl("NodeView", "nc", node.ID));
            else
                return CMSContext.Current.ResolveUrl(url);
        }

        /// <summary>
        /// Gets the entry URL.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static string GetEntryUrl(Entry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            string url = String.Empty;

            Seo seo = GetLanguageSeo(entry.SeoInfo);

            if(seo != null)
                url = "~/" + seo.Uri;

            if (String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(entry.ID) && entry.ID != "0" && !entry.ID.StartsWith("@"))
                return CMSContext.Current.ResolveUrl(NavigationManager.GetUrl("EntryView", "ec", entry.ID));
            else
                return CMSContext.Current.ResolveUrl(url);
        }

        /// <summary>
        /// Gets the language seo.
        /// </summary>
        /// <param name="seoInfo">The seo info.</param>
        /// <returns></returns>
        public static Seo GetLanguageSeo(Seo[] seoInfo)
        {
            Seo seoReturn = null;
            if (seoInfo != null)
            {
                foreach (Seo seo in seoInfo)
                {
                    if (seo.LanguageCode.Equals(CMSContext.Current.LanguageName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        seoReturn = seo;
                        break;
                    }
                }
            }

            return seoReturn;
        }

        /// <summary>
        /// Gets the sale price. The current calture info currency code will be used.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Price</returns>
        public static Price GetSalePrice(Entry entry, decimal quantity)
        {
            string currencyCode = CMSContext.Current.CurrencyCode;
            if (entry.SalePrices == null || entry.SalePrices.SalePrice.Length == 0)
                return entry.ItemAttributes.ListPrice;

            Price price = null;

            SalePrice[] priceRows = entry.SalePrices.SalePrice;

            // Determine price by using tiers
            if (priceRows.Length > 0)
            {
                foreach (SalePrice priceRow in priceRows)
                {
                    // We only need currency that we are working in
                    if (!currencyCode.Equals(priceRow.UnitPrice.CurrencyCode))
                        continue;

                    // Check inventory first
                    if (priceRow.MinQuantity > quantity)
                        continue; // didn't meet min quantity requirements

                    // Check dates
                    if (priceRow.StartDate > FrameworkContext.Current.CurrentDateTime || priceRow.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue; // falls outside of acceptable range

                    if (priceRow.SaleType == SaleType.TypeKey.AllCustomers.ToString()) // no need to check, applies to everyone
                    {
                        if (price == null || price.Amount > priceRow.UnitPrice.Amount)
                            price = priceRow.UnitPrice;
                    }
                    else if (priceRow.SaleType == SaleType.TypeKey.Customer.ToString()) // check if it applies to a customer
                    {
                        string userName = ProfileContext.Current.UserName;

                        if (String.IsNullOrEmpty(userName))
                            continue;

                        // Check sale code
                        if (userName != priceRow.SaleCode)
                            continue; // didn't match

                        if (price == null || price.Amount > priceRow.UnitPrice.Amount)
                            price = priceRow.UnitPrice;
                    }
                    else if (priceRow.SaleType == SaleType.TypeKey.CustomerPriceGroup.ToString()) // check if it applies to a customer
                    {
                        MembershipUser user = ProfileContext.Current.User;

                        if (user == null)
                            continue;

                        Account account = ProfileContext.Current.GetAccount(user.ProviderUserKey.ToString());
                        if (account == null)
                            continue;

                        // Check sale code
                        if (account.CustomerGroup != priceRow.SaleCode)
                            continue; // didn't match

                        if (price == null || price.Amount > priceRow.UnitPrice.Amount)
                            price = priceRow.UnitPrice;
                    }
                    else // simply check the session for the parameter
                    {
                        if (!String.IsNullOrEmpty(priceRow.SaleType))
                        {
                            if (HttpContext.Current.Session[priceRow.SaleType] != null)
                            {
                                // Check sale code
                                if ((string)HttpContext.Current.Session[priceRow.SaleType] != priceRow.SaleCode)
                                    continue; // didn't match

                                if (price == null || price.Amount > priceRow.UnitPrice.Amount)
                                    price = priceRow.UnitPrice;
                            }
                        }
                    }
                }
            }

            if (price == null)
                return entry.ItemAttributes.ListPrice;

            return price;
        }

        /// <summary>
        /// Gets the discount price by evaluating the discount rules and taking into account segments customer belongs to.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        public static Price GetDiscountPrice(Entry entry, string catalogName)
        {
            if(entry == null)
                throw new NullReferenceException("entry can't be null");

            decimal minQuantity = 1;

            // get min quantity attribute
            if (entry.ItemAttributes != null)
                minQuantity = entry.ItemAttributes.MinQuantity;

            // we can't pass qauntity of 0, so make it default to 1
            if (minQuantity <= 0)
                minQuantity = 1;

            // Get sale price for the current user
            Price price = StoreHelper.GetSalePrice(entry, minQuantity);

            // Create new promotion helper, which will initialize PromotionContext object for us and setup context dictionary
            PromotionHelper helper = new PromotionHelper();

            // Get current context
            Dictionary<string, object> context = MarketingContext.Current.MarketingProfileContext;

            // Create filter
            PromotionFilter filter = new PromotionFilter();
            filter.IgnoreConditions = false;
            filter.IgnorePolicy = false;
            filter.IgnoreSegments = false;
            filter.IncludeCoupons = false;

            // Create new entry
            PromotionEntry promotEntry = new PromotionEntry(catalogName, String.Empty, entry.ID, price.Amount);

            // Populate entry parameters
            ((IPromotionEntryPopulate)MarketingContext.Current.PromotionEntryPopulateFunctionClassInfo.CreateInstance()).Populate(ref promotEntry, entry);

            PromotionEntriesSet sourceSet = new PromotionEntriesSet();
            sourceSet.Entries.Add(promotEntry);

            // Configure promotion context
            helper.PromotionContext.SourceEntriesSet = sourceSet;
            helper.PromotionContext.TargetEntriesSet = sourceSet;
            // Only target entries
            helper.PromotionContext.TargetGroup = PromotionGroup.GetPromotionGroup(PromotionGroup.PromotionGroupKey.Entry).Key;

            // Execute the promotions and filter out basic collection of promotions, we need to execute with cache disabled, so we get latest info from the database
            helper.Eval(filter);
           
            // Check the count, and get new price
            if (helper.PromotionContext.PromotionResult.PromotionRecords.Count > 0)
                return ObjectHelper.CreatePrice(price.Amount - GetDiscountPrice(helper.PromotionContext.PromotionResult), price.CurrencyCode);
            else
                return price;
        }

        /// <summary>
        /// Gets the discount price.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private static decimal GetDiscountPrice(PromotionResult result)
        {
            decimal discountAmount = 0;
            foreach (PromotionItemRecord record in result.PromotionRecords)
            {
                discountAmount += GetDiscountAmount(record, record.PromotionReward);
            }

            return discountAmount;
        }

        /// <summary>
        /// Gets the discount amount for one entry only.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="reward">The reward.</param>
        /// <returns></returns>
        private static decimal GetDiscountAmount(PromotionItemRecord record, PromotionReward reward)
        {
            decimal discountAmount = 0;
            if (reward.RewardType == PromotionRewardType.EachAffectedEntry || reward.RewardType == PromotionRewardType.AllAffectedEntries)
            {
                if (reward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = record.AffectedEntriesSet.TotalCost * reward.AmountOff / 100;
                }
                else // need to split discount between all items
                {
                    discountAmount += reward.AmountOff; // since we assume only one entry in affected items
                }
            }
            return discountAmount;
        }
		
        #region Inventory Helpers
        /// <summary>
        /// Determines whether entry is in stock.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if entry is in stock otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInStock(Entry entry)
        {
            if (entry == null)
                return false;

            if (entry.Inventory == null)
                return false;
            
            Inventory inv = entry.Inventory;

            // If we don't account inventory return true always
            if (!inv.InventoryStatus.ToLower().Equals("enabled"))
                return true;

            if (GetItemsInStock(entry) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the items in stock.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static decimal GetItemsInStock(Entry entry)
        {
            if (entry == null)
                return 0;

            if (entry.Inventory == null)
                return 0;

            Inventory inv = entry.Inventory;
            return inv.InStockQuantity - inv.ReservedQuantity;
        }

        /// <summary>
        /// Gets the inventory status.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static string GetInventoryStatus(Entry entry)
        {
            if (IsInStock(entry))
                return "Item is in stock";

            if (entry == null)
                return "";

            if (entry.Inventory == null)
                return "";

            Inventory inv = entry.Inventory;

            if(IsAvailableForPreorder(entry))
                return "Item is available for preorder";
            else if(inv.AllowPreorder)
                return String.Format("Item will be available for preorder on {0}", inv.PreorderAvailabilityDate.ToString("MMMM dd, yyyy"));

            if (IsAvailableForBackorder(entry))
                return "Item is available for backorder";
            else if (inv.AllowBackorder)
                return String.Format("Item will be available for backorder on {0}", inv.BackorderAvailabilityDate.ToString("MMMM dd, yyyy"));

            return "Item is out of stock";
        }

        /// <summary>
        /// Determines whether [is available for backorder] [the specified entry].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if [is available for backorder] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAvailableForBackorder(Entry entry)
        {
            if (entry == null)
                return false;

            if (entry.Inventory == null)
                return false;

            Inventory inv = entry.Inventory;

            if (inv.AllowBackorder && inv.BackorderQuantity > 0 && inv.BackorderAvailabilityDate <= DateTime.UtcNow)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether [is available for preorder] [the specified entry].
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if [is available for preorder] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAvailableForPreorder(Entry entry)
        {
            if (entry == null)
                return false;

            if (entry.Inventory == null)
                return false;

            Inventory inv = entry.Inventory;

            if (inv.AllowPreorder && inv.PreorderQuantity > 0 && inv.PreorderAvailabilityDate <= DateTime.UtcNow)
                return true;

            return false;
        }
        #endregion

        /// <summary>
        /// Gets the display name of the entry. Returns localized version or the product name if no localized version available.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static string GetEntryDisplayName(Entry entry)
        {
            string name = entry.Name;
			if (entry.ItemAttributes != null && entry.ItemAttributes["DisplayName"] != null)
			{
				string displayName = entry.ItemAttributes["DisplayName"].ToString();
				if (!String.IsNullOrEmpty(displayName))
					name = displayName;
			}

            return name;
        }

        /// <summary>
        /// Gets the display name of the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static string GetNodeDisplayName(CatalogNode node)
        {
            string name = node.Name;
			if (node.ItemAttributes != null && node.ItemAttributes["DisplayName"] != null)
			{
				string displayName = node.ItemAttributes["DisplayName"].ToString();
				if (!String.IsNullOrEmpty(displayName))
					name = displayName;
			}

            return name;
        }

        #region Browse History Management
        /// <summary>
        /// Adds the browse history.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddBrowseHistory(string key, string value)
        {
            int maxHistory = 10;
            NameValueCollection history = GetBrowseHistory();

            string[] values = history.GetValues(key);

            // Remove current key, since we will need to readd it
            history.Remove(key);

            if (values != null)
            {
                List<string> list = new List<string>(values);

                // Remove all items
                while(list.Remove(value));

                // Remove oldest item(s)
                while(list.Count >= maxHistory)
                    list.RemoveAt(list.Count - 1);

                // Add it at the very front, since it is already sorted
                list.Insert(0, value);

                for (int index = 0; index < list.Count; index++)
                {
                    history.Add(key, list[index]);
                }
            }
            else
            {
                history.Add(key, value);
            }

            CommonHelper.SetCookie("BrowseHistory", history, DateTime.Now.AddDays(1));
        }

        /// <summary>
        /// Gets the browse history.
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection GetBrowseHistory()
        {
            NameValueCollection cookie = CommonHelper.GetCookie("BrowseHistory");
            if (cookie == null)
                cookie = new NameValueCollection();
            return cookie;

            /*
            // Record history
            StringCollection historyDic = (StringCollection)Profile["EntryHistory"];

            // Check if the code already exists
            if (historyDic.Contains(Code))
            {
                historyDic.RemoveAt(historyDic.IndexOf(Code));
            }

            // Only keep history of last 5 items visited
            if (historyDic.Count >= 5)
                historyDic.RemoveAt(0);

            historyDic.Add(Code);

            // set value
            Profile["EntryHistory"] = historyDic;
             * */
        }
        #endregion
    }
}

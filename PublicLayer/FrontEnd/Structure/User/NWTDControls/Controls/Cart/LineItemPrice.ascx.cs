using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.Objects;
using System.ComponentModel;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	
	/// <summary>
	/// A user control that displays a Line Item price to an NWTD user
	/// </summary>
	public partial class LineItemPrice : System.Web.UI.UserControl {


		private LineItem _LineItem;
		/// <summary>
		/// Gets or sets the line item.
		/// </summary>
		/// <value>The line item.</value>
		public LineItem LineItem { get { return _LineItem; } set { _LineItem = value; } }


		/// <summary>
		/// The type of price we'll be displaying
		/// </summary>
		public enum EntryPriceType { Discount, NetSchool };

		/// <summary>
		/// The Entry for which the price will be displayed
		/// </summary>
		[Bindable(true)]
		public Entry Entry { get; set; }

		public EntryPriceType PriceType {get;set;}


		protected override void EnsureChildControls() {
			this.BindPrice();
			base.EnsureChildControls();
		}

		/// <summary>
		/// First find out if there's a special price and use it. Then, if the price is 0 and the "NoPriceAvialable" field is set,
		/// indicate that.
		/// </summary>
		protected void BindPrice() {

			if (LineItem == null) return;

			string currencyCode = LineItem.Parent.Parent.BillingCurrency;
			Entry entry = CatalogContext.Current.GetCatalogEntry(LineItem.CatalogEntryId, new Mediachase.Commerce.Catalog.Managers.CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

			if (entry == null) {
				this.litPrice.Text = "This entry no longer exists";
				return;
			}

			decimal price = 0;
			if (this.PriceType == EntryPriceType.Discount) {
				//Originally, ECF used the total stored on the line itme and divided by the quantity
				//Since we allow quantities of zero, we have to grab the price from the entry itself
				price = StoreHelper.GetDiscountPrice(entry, String.Empty/*CatalogName*/).Amount;
				//price = LineItem.ExtendedPrice / LineItem.Quantity;//LineItem.ExtendedPrice;
			} else {
				price = LineItem.ListPrice;
				//price = StoreHelper.GetSalePrice(this.Entry, 1);
			}
			//decimal total = NWTD.Orders.Cart.LineItemTotal(this._lineItem);
			//string currencyCode = LineItem.Parent.Parent.BillingCurrency;

			if (price.Equals(0) && bool.Parse( (entry.ItemAttributes["NoPriceAvailable"].ToString())) ) {
				this.litPrice.Text = "Price Not Available";
			}

			else this.litPrice.Text = CurrencyFormatter.FormatCurrency(price, currencyCode);
		}
	}
}
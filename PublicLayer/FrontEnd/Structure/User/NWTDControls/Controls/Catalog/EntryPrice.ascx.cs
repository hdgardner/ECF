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

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	
	/// <summary>
	/// A user control that displays an Entry price to an NWTD user
	/// </summary>
	public partial class EntryPrice : System.Web.UI.UserControl {
		
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
			Price price;
			if (this.PriceType == EntryPriceType.Discount) {
				price = StoreHelper.GetDiscountPrice(this.Entry, String.Empty/*CatalogName*/);
			} else {
				price = Entry.ItemAttributes.ListPrice;
				//price = StoreHelper.GetSalePrice(this.Entry, 1);
			}
			//decimal total = NWTD.Orders.Cart.LineItemTotal(this._lineItem);
			//string currencyCode = LineItem.Parent.Parent.BillingCurrency;

			if (price.Amount.Equals(0) && bool.Parse( (this.Entry.ItemAttributes["NoPriceAvailable"].ToString())) ) {
				this.litPrice.Text = "Price Not Available";
			}

			else this.litPrice.Text = price.FormattedPrice;
		}
	}
}
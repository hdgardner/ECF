using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;

using NWTD;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// A contron for displaying the total price of a line item
	/// </summary>
	public partial class LineItemTotal : System.Web.UI.UserControl {

		private LineItem _lineItem;
		
		/// <summary>
		/// The line item to get the total for
		/// </summary>
		[Bindable(true)]
		public LineItem LineItem {
			get { return this._lineItem; }
			set { this._lineItem = value; }
		}


		protected override void EnsureChildControls() {
			this.BindTotal();
			base.EnsureChildControls();
		}

		/// <summary>
		/// Calculates the line item total
		/// </summary>
		protected void BindTotal() {
            if(_lineItem == null)
            {
                litTotal.Text = string.Empty;
                return;
            }

			decimal total = NWTD.Orders.Cart.LineItemTotal(this._lineItem);
			string currencyCode = LineItem.Parent.Parent.BillingCurrency;
			this.litTotal.Text = CurrencyFormatter.FormatCurrency(total, currencyCode);
		}
	}
}
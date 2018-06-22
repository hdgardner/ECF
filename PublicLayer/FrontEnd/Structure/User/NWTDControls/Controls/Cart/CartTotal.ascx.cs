using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// Control for displaying the total of a cart
	/// </summary>
	public partial class CartTotal : System.Web.UI.UserControl {
		
		/// <summary>
		/// During this event, rebind the total
		/// </summary>
		protected override void EnsureChildControls() {
			this.BindTotal();
			base.EnsureChildControls();
		}

		/// <summary>
		/// Binds the cart total to the control
		/// </summary>
		protected void BindTotal() {
			decimal total = NWTD.Orders.Cart.CartTotal(this.Cart);
			string currencyCode = this.Cart.BillingCurrency;
			this.litTotal.Text = CurrencyFormatter.FormatCurrency(total, currencyCode);
		}

		/// <summary>
		/// The cart for which the total is being displayed
		/// </summary>
		[Bindable(true)]
		public Mediachase.Commerce.Orders.Cart Cart {get;set;}
	}
}
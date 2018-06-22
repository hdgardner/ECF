using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	public partial class AddToCartButton : System.Web.UI.UserControl {
		public string Code {
			get { return this.hfCode.Value; }
			set { this.hfCode.Value = value; }
		}

		protected void Page_Load(object sender, EventArgs e) {


			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryModal_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.modal.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "AddToCartButton_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/AddToCartButton.js"));

			global::NWTD.Profile.EnsureCustomerCart();


			if (global::NWTD.Orders.Cart.Reminder) {
				this.pnlSelectCart.Visible = true;
				List<Mediachase.Commerce.Orders.Cart> carts = global::NWTD.Orders.Cart.CurrentCustomerCarts;

				this.ddlCarts.DataSource = carts;
				this.ddlCarts.DataBind();

				foreach (ListItem item in this.ddlCarts.Items) {
					if (item.Value.Equals(global::NWTD.Profile.ActiveCart)) {
						item.Selected = true;
					    break;
					}
				}
			}
		
		}
	}
}
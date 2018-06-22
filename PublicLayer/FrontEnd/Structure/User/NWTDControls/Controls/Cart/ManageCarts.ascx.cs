using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility.Commerce;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	public partial class ManageCarts : System.Web.UI.UserControl {
		


		protected void gvUserCarts_RowDataBound(object sender, GridViewRowEventArgs e) {

			//// when data gets added to the list of carts
			//if (e.Row.RowType != DataControlRowType.DataRow) return;
			//Mediachase.Commerce.Orders.Cart cart = e.Row.DataItem as Mediachase.Commerce.Orders.Cart;
			//if (((Mediachase.Commerce.Orders.Cart)e.Row.DataItem).Name == NWTD.Profile.ActiveCart) {
			//    RadioButton rbActiveCart = e.Row.FindControl("rbActiveCart") as RadioButton;
			//    HyperLink hlDeleteCart = e.Row.FindControl("hlDeleteCart") as HyperLink;
			//    hlDeleteCart.Visible = false;
			//    //hlDeleteCart.Parent.Controls.Remove(hlDeleteCart);
			//    rbActiveCart.Checked = true;
			//}




			//format the cell to be the correct currency format using MediaChase's currency formatter
			//e.Row.Cells[3].Text = CurrencyFormatter.FormatCurrency(Decimal.Parse(e.Row.Cells[3].Text), cart.BillingCurrency);

		}

		/// <summary>
		/// When a row gets created, a bunch of controls in it are maniuplated depending on context
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void gvUserCarts_RowCreated(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType != DataControlRowType.DataRow) return;
			Mediachase.Commerce.Orders.Cart cart = e.Row.DataItem as Mediachase.Commerce.Orders.Cart;
			
			HyperLink hlDeleteCart = e.Row.FindControl("hlDeleteCart") as HyperLink;
			HyperLink hlEditCart = e.Row.FindControl("hlEditCart") as HyperLink;
			HyperLink hlCopyCArt = e.Row.FindControl("hlCopyCart") as HyperLink;
			LinkButton lbGoToCart = e.Row.FindControl("lbGoToCart") as LinkButton;
			
			if (((Mediachase.Commerce.Orders.Cart)e.Row.DataItem).Name == NWTD.Profile.ActiveCart) {
				e.Row.CssClass = e.Row.CssClass + " active-cart";
				RadioButton rbActiveCart = e.Row.FindControl("rbActiveCart") as RadioButton;
				hlDeleteCart.Visible = false;
				rbActiveCart.Checked = true;
			} else {
				hlDeleteCart.Visible = true;
			}

			if (!NWTD.Orders.Cart.CartCanBeEdited(cart)) {
				hlDeleteCart.Visible = false;
				hlEditCart.Visible = false;
				hlCopyCArt.Visible = false;
				//lbGoToCart.Visible = false;
				//lbGoToCart.Parent.Controls.Add(new Literal() { Text = lbGoToCart.Text });
			}
		}

		/// <summary>
		/// When a cart gets selected in the UI, we need to provide it with some information for its query.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void dsUserCarts_Selecting(object sender, ObjectDataSourceSelectingEventArgs e) {

			// when the datasource is selected for the list of carts, we need
			// to add a param that represents the current shopper to the select statement
			e.InputParameters["CustomerId"] = ProfileContext.Current.UserId;
		}


		protected void rbActiveCart_CheckedChanged(object sender, EventArgs e) {
			foreach (GridViewRow row in this.gvUserCarts.Rows) {
				RadioButton rbActiveCart = row.FindControl("rbActiveCart") as RadioButton;
				if (rbActiveCart.Equals(sender)) {
					//rbActiveCart.Checked = true;
					//HyperLink hlDeleteCart = row.FindControl("hlDeleteCart") as HyperLink;
					//hlDeleteCart.Visible = false;
					NWTD.Profile.ActiveCart = gvUserCarts.DataKeys[row.RowIndex].Value.ToString();
				} else { 
					//rbActiveCart.Checked = false; 
				}
				
			}
			gvUserCarts.DataBind();
		}

		/// <summary>
		/// During page load, the required JS files are loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {

			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryModal_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.modal.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ManageCarts_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/ManageCarts.js"));

			OakTree.Web.UI.ControlHelper.RegisterControlInClientScript(Page.ClientScript, this, "ManageCarts", "{updatePanelID:'" + this.udpUserCarts.ClientID + "', cartGridID:'"+this.gvUserCarts.ClientID+"'}");
		}

		/// <summary>
		/// When a user clicks the name of the cart, it makes that cart "active" and re-directs them to that cart's page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void lbGoToCart_Command(object sender, CommandEventArgs e) {
			
			if (e.CommandName.Equals("SelectCart")) {
				CartHelper helper = new CartHelper(e.CommandArgument.ToString(), ProfileContext.Current.UserId);
				if (NWTD.Orders.Cart.CartCanBeEdited(helper.Cart)) {
					NWTD.Profile.ActiveCart = e.CommandArgument.ToString();
				}

				Response.Redirect(  NavigationManager.GetUrl("ViewCart", new object[]{"cart",e.CommandArgument.ToString()}));
			
			}
		}



	}
}
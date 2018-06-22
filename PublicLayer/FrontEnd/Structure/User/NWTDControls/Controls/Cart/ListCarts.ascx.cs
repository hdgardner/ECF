using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.Pages;
using Mediachase.Commerce.Orders;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Shared;
using Mediachase.Cms;
using System.Web.UI.HtmlControls;


namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// Control for displaying a list of a customer's carts
	/// </summary>
	public partial class ListCarts : System.Web.UI.UserControl {

		/// <summary>
		/// The currently logged in user's ID
		/// </summary>
		public Guid CurrentUserID {
			get { return ProfileContext.Current.UserId; }
		}

		/// <summary>
		/// During page load, the required JS files are loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			if (this.Page.IsPostBack) {
				
			}
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "NWTD_jquery_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery-1.3.2.min.js"));
			string scriptText = "NWTD.CartList.controlIDs.push('"+this.udpCartViewer.UniqueID+"')";
			Page.ClientScript.RegisterClientScriptInclude("NWTD_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/NWTD.js"));
			Page.ClientScript.RegisterClientScriptInclude("CartList_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/CartList.js"));
			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CartPage", scriptText,true);
		}

		/// <summary>
		/// When a cart gets selected in the UI, we need to provide it with some information for its query.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void dsMyCarts_Selecting(object sender, ObjectDataSourceSelectingEventArgs e) {
			e.InputParameters["CustomerId"] = this.CurrentUserID;
		}

		/// <summary>
		/// Event handler for carts getting bound to the carts grid.
		/// During this process, bind the line item to the sub grid that contains some controls for editing the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void rptrMyCarts_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			GridView gvMyCart =	e.Item.FindControl("gvMyCart") as GridView;
			Mediachase.Commerce.Orders.Cart cart = e.Item.DataItem as Mediachase.Commerce.Orders.Cart;
			LinkButton makeActive = e.Item.FindControl("lbMakeActive") as LinkButton;

			if (cart.Name == NWTD.Profile.ActiveCart) {
				HyperLink viewOrder = e.Item.FindControl("linkViewOrder") as HyperLink;
				HtmlGenericControl header = e.Item.FindControl("CartHeading") as HtmlGenericControl;
				header.Attributes["class"] = "selected";

				viewOrder.Visible = true;
				makeActive.Visible = false;
			} else {
				//makeActive.Click += new EventHandler(makeActive_Click);
			}
			CartHelper helper = new CartHelper(cart);
			gvMyCart.DataSource = helper.LineItems;
			gvMyCart.DataBind();
			
		}

		/// <summary>
		/// Event handler for when you click the button for making the cart active. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void makeActive_Click(object sender, RepeaterCommandEventArgs e) {
			//throw new NotImplementedException();
			//LinkButton linkButton = sender as LinkButton;
			NWTD.Profile.ActiveCart= e.CommandArgument.ToString();
			this.rptrMyCarts.DataBind();
		}

	}
}
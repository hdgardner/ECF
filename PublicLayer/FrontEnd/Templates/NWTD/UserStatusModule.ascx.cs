using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Orders;

namespace Mediachase.Cms.Templates.NWTD
{
    public partial class UserStatusModule : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(Object sender, EventArgs e)
        {

			HyperLink ShoppingCartLink = this.LoginView1.FindControl("ShoppingCartLink") as HyperLink;
			if (ShoppingCartLink != null) {
				string cartname = global::NWTD.Profile.ActiveCart;
				if(cartname.Length > 28) cartname = cartname.Substring(0,25) + "...";
				ShoppingCartLink.Text = cartname;
			}

			Literal litFirstName = this.LoginView1.FindControl("litFirstName") as Literal;

			if (litFirstName != null) {
				string firstName = Mediachase.Commerce.Profile.ProfileContext.Current.Profile.FirstName;
				if (firstName.Length > 13) firstName = firstName.Substring(0, 10) + "...";



				litFirstName.Text = firstName;
			}



			string scriptString = "function UpdateShoppingCart() {\r\n" +
						Page.ClientScript.GetPostBackEventReference(btnShoppingCartUpdate, String.Empty) + ";\r\n" +
					"}";
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "shoppingCart_UpdateScript", scriptString, true);

			//displaying how many items are in the cart.
			//if we ever want to add this functionality back, unccoment below
			//BindShoppingCart();
            DataBind();
        }

		protected void btnShoppingCartUpdate_Click(object sender, EventArgs e)
		{
			//if we ever want to add this functionality back, unccoment below
			//BindShoppingCart();
		}

        /// <summary>
        /// Binds the shopping cart.
        /// </summary>
        private void BindShoppingCart()
        {
			if (HttpContext.Current.User.Identity.IsAuthenticated) {

				CartHelper cart = new CartHelper(global::NWTD.Profile.ActiveCart);

				decimal numberOfItems = cart.GetTotalItemCount();
				((HyperLink)this.LoginView1.FindControl("ShoppingCartLink")).Text = String.Format("{0} item(s) in your active Wish List", (int)numberOfItems);
			}
        }
    }
}
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

namespace Mediachase.Cms.Templates.Default
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
			string scriptString = "function UpdateShoppingCart() {\r\n" +
						Page.ClientScript.GetPostBackEventReference(btnShoppingCartUpdate, String.Empty) + ";\r\n" +
					"}";
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "shoppingCart_UpdateScript", scriptString, true);
            BindShoppingCart();
            DataBind();
        }

		protected void btnShoppingCartUpdate_Click(object sender, EventArgs e)
		{
			BindShoppingCart();
		}

        /// <summary>
        /// Binds the shopping cart.
        /// </summary>
        private void BindShoppingCart()
        {
            CartHelper cart = new CartHelper(Cart.DefaultName);

            decimal numberOfItems = cart.GetTotalItemCount();

            if (numberOfItems == 0)
                ShoppingCartLink.Text = RM.GetString("BASKET_YOUR_LABEL");
            else
                ShoppingCartLink.Text = String.Format("{0} item(s) in your Wish List", (int)numberOfItems);              

        }
    }
}
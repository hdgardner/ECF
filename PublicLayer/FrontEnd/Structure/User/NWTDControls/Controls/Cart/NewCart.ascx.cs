using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility.Commerce;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart
{
    /// <summary>
    /// Control that supplies a "new cart" that shows up in a modal, allowing a user to create a new cart.
    /// </summary>
	public partial class NewCart : System.Web.UI.UserControl {

		/// <summary>
		/// During page load, add all the necessary JavaScript files
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryModal_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.modal.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "NewCart_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/NewCart.js"));
        }

		/// <summary>
		/// The text for the new cart button
		/// </summary>
        public string LinkText {
            get { return this.hlCreateCart.Text; }
            set { this.hlCreateCart.Text = value; }
        }

		/// <summary>
		/// The image for the new cart button
		/// </summary>
        public string LinkImageUrl {
            get { return this.hlCreateCart.ImageUrl; }
            set { this.hlCreateCart.ImageUrl = value; }
        }
    }

}
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
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Profile;

public partial class PO2Go : System.Web.UI.Page
{
    #region Private Fields
    private string _PO2GoParams;
    private string _returnUrl;

    #endregion
    #region Properties
    protected string EncodedPunchOut2GoParams
    {
        get
        {
            return _PO2GoParams;
        }
    }
    protected string ReturnUrl
    {
        get
        {
            return _returnUrl;
        }
    }

    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        this._PO2GoParams = (string)Session["po2go_outbound_params"];
        this._returnUrl = (string)Session["po2go_return_url"];
        NWTD.Profile.EnsureCustomerCart();
        string cartName = NWTD.Profile.ActiveCart;
        CartHelper helper = new CartHelper(cartName, ProfileContext.Current.UserId);
        helper.Cart.Name = helper.Cart.Name + "_" + helper.Cart.Id;
        helper.Cart.Status = "Transferred"; // This may need to be added to the NWTD.Orders namespace...we'll see.
        helper.Cart.AcceptChanges();

        Session.Remove("po2go_pos");
        Session.Remove("po2go_return_url");
        Session.Remove("po2go_params");
        Session.Remove("po2go_outbound_params");
        FormsAuthentication.SignOut();
    }
}

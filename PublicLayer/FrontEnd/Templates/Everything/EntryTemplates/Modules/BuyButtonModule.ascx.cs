using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Cms;

public partial class Templates_Everything_Entry_Modules_BuyButtonModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    private Entry _Entry;
    private bool _IsPopupWindow = false;

    #region Properties
    /// <summary>
    /// Gets or sets the Entry object to inspect.
    /// </summary>
    /// <value>The Entry object.</value>
    public Entry Entry
    {
        get
        {
            return _Entry;
        }
        set
        {
            _Entry = value;
        }
    }

    /// Gets or sets a value indicating whether the BuyButton
    /// can be clicked to perform a post back to the server.
    /// </summary>
    /// <value>true if the control is enabled; otherwise, false. The default is true.</value>
    public bool Enabled
    {
        get
        {
            return PurchaseLink.Enabled;
        }
        set
        {
            PurchaseLink.Enabled = value;
        }
    }
    
    /// <summary>
    /// Gets or sets the PopupWindow property to inspect.
    /// </summary>
    /// <value>The PopupWindow property.</value>
    public bool IsPopupWindow
    {
        get
        {
            return _IsPopupWindow;
        }
        set
        {
            _IsPopupWindow = value;
        }
    }
    #endregion

    #region event: OnClick
    public delegate void ClickHandler(Object sender, EventArgs e, Entry entry, ref bool reject);

    public event ClickHandler Click;

    protected virtual void OnClick(Object sender, EventArgs e, Entry entry, ref bool reject)
    {
        if (Click != null)
        {
            Click(this, e, entry, ref reject);
        }
    }
    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "checkout_js", CommerceHelper.GetAbsolutePath("Scripts/checkout.js"));
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "jquery_js", CommerceHelper.GetAbsolutePath("Scripts/jquery.js"));
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "thickbox_js", CommerceHelper.GetAbsolutePath("Scripts/thickbox.js"));
        Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "thickbox_js_imgLoader", String.Format("var tb_pathToImage = '{0}';", CommerceHelper.GetAbsolutePath("App_Themes/Everything/images/loading_rss.gif")), true);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.DataBinding"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);
    }

    /// <summary>
    /// Handles the Click event of the PurchaseLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    protected void PurchaseLink_Click(object sender, ImageClickEventArgs e)
    {
        if (Entry != null)
        {
            if (Entry.EntryType.Equals("Bundle", StringComparison.InvariantCultureIgnoreCase) || Entry.EntryType.Equals("Product", StringComparison.InvariantCultureIgnoreCase) || Entry.EntryType.Equals("Package", StringComparison.InvariantCultureIgnoreCase))
            {
                Response.Redirect(StoreHelper.GetEntryUrl(Entry));
            }
            else
            {
                string scriptStartup = String.Empty;
                bool reject = false;

                this.Click(sender, (EventArgs)e, Entry, ref reject);

				if (!reject)
                {
                    string productNameTitle = Entry.Name;

                    int resLength = 40;
                    if (Entry.Name.Length > resLength)
                        productNameTitle = Entry.Name.Substring(0, resLength);

                    while (Entry.Name.Length > resLength++)
                    {
                        if (Char.IsWhiteSpace(Entry.Name, resLength - 1))
                        {
                            productNameTitle += " ...";
                            break;
                        }
                        productNameTitle += Entry.Name[resLength - 1];
                    }

                    pnlResponseBlock.Visible = true;

                    string ShoppingCartUrl = ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("ShoppingCart"));
                    if (IsPopupWindow)
                        btnGoToCart.OnClientClick = String.Format("CSCheckout.RedirectFromPopupWindow('{0}');tb_remove();return false;", ShoppingCartUrl);
                    else
                        btnGoToCart.OnClientClick = String.Format("self.parent.document.location.href = '{0}';return false;", ShoppingCartUrl);

                    scriptStartup = String.Format("tb_show('{0}', '#TB_inline?inlineId={1}', false);", productNameTitle, pnlResponseBlock.ClientID);
                }
                else
                {
                    scriptStartup = String.Format("tb_remove();");
                }

                ScriptManager.RegisterClientScriptBlock(uplByButton, typeof(UpdatePanel), uplByButton.UniqueID, scriptStartup, true);
				ScriptManager.RegisterStartupScript(Page, this.GetType(), "updateShoppingCart", "UpdateShoppingCart();", true);
            }
        }
    }
   
}
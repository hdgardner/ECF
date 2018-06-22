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
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms;

public partial class Controls_Cart_SharedModules_BuyButtonModule : System.Web.UI.UserControl
{
    Entry _Entry = null;
    /// <summary>
    /// Gets or sets the entry.
    /// </summary>
    /// <value>The entry.</value>
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

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        PurchaseLink.Click += new EventHandler(PurchaseLink_Click);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
    }

    /// <summary>
    /// Handles the Click event of the PurchaseLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void PurchaseLink_Click(object sender, EventArgs e)
    {
        // Add item to a cart
        new CartHelper(Cart.DefaultName).AddEntry(Entry);

        // Redirect to shopping cart
        Response.Redirect(NavigationManager.GetUrl("ShoppingCart"));
    }
}

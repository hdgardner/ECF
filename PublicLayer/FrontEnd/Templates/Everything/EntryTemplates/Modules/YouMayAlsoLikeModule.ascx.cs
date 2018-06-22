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
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Orders;

public partial class Templates_Everything_Entry_Modules_YouMayAlsoLikeModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    private Entry _Entry;
    private string _CatalogName;
    private string _YouMayAlsoLikeAssociationName;

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
    
    /// <summary>
    /// Gets or sets the CatalogName to pass to our StoreHelper object.
    /// </summary>
    /// <value>The CatalogName string.</value>
    public string CatalogName
    {
        get
        {
            return _CatalogName;
        }
        set
        {
            _CatalogName = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of our You May Also Like Association.
    /// </summary>
    /// <value>The string name of our You May Also Like Association.</value>
    public string YouMayAlsoLikeAssociationName
    {
        get
        {
            return _YouMayAlsoLikeAssociationName;
        }
        set
        {
            _YouMayAlsoLikeAssociationName = value;
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
    }


    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.DataBinding"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);

        if (Entry != null)
        {
            Association[] assocs = Entry.Associations;

            if (assocs != null)
            {
                bool youMayAlsoLikeMatched = false ;

                foreach (Association a in assocs)
                {
                    if (a.EntryAssociations == null)
                        continue;

                    // Check for you may also like association name matches.
                    // This provides for only one (the first) matching association to be bound to the repeater.
                    if (a.Name == YouMayAlsoLikeAssociationName && a.EntryAssociations.Association.Length > 0)
                    {
                        YouMayAlsoLikeRepeater.DataSource = a.EntryAssociations.Association;
                        YouMayAlsoLikeRepeater.DataBind();

                        youMayAlsoLikeMatched = true;
                        break;
                    }
                }

                // Set you may also like Panel
                this.YouMayAlsoLikePanel.Visible = youMayAlsoLikeMatched;
            }
            else
            {
                this.YouMayAlsoLikePanel.Visible = false;
            }
        }
        else
        {
            this.YouMayAlsoLikePanel.Visible = false;
        }
    }

    /// <summary>
    /// Handles the Click event of the AddToCartImageButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    protected void AddToCartImageButton_Click(object sender, ImageClickEventArgs e)
    {
        CartHelper ch = new CartHelper(Cart.DefaultName);

        EntryAssociation[] youMayAlsoLikeAssociations = this.YouMayAlsoLikeRepeater.DataSource as EntryAssociation[];
        if (youMayAlsoLikeAssociations != null)
        {
            // Be careful with array bounds.
            for (int i = 0; i <= this.YouMayAlsoLikeRepeater.Items.Count - 1; i++)
            {
                ImageButton ib = this.YouMayAlsoLikeRepeater.Items[i].FindControl("AddToCartImageButton") as ImageButton;
                if (ib != null)
                {
                    // Check for a match between the sender and the RepeaterItem.
                    if ((ImageButton)sender == ib)
                    {
                        ch.AddEntry(youMayAlsoLikeAssociations[i].Entry, 1); // Add just one.
                    }
                }
            }
        }

        // Redirect to shopping cart.
        Response.Redirect(ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("ShoppingCart")));
    }

    /// <summary>
    /// Handles the Click event of the AddToCartLinkButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void AddToCartLinkButton_Click(object sender, EventArgs e)
    {
        CartHelper ch = new CartHelper(Cart.DefaultName);

        EntryAssociation[] youMayAlsoLikeAssociations = this.YouMayAlsoLikeRepeater.DataSource as EntryAssociation[];
        if (youMayAlsoLikeAssociations != null)
        {
            // Be careful with array bounds.
            for (int i = 0; i <= this.YouMayAlsoLikeRepeater.Items.Count - 1; i++)
            {
                LinkButton lb = this.YouMayAlsoLikeRepeater.Items[i].FindControl("AddToCartLinkButton") as LinkButton;
                if (lb != null)
                {
                    // Check for a match between the sender and the RepeaterItem.
                    if ((LinkButton)sender == lb)
                    {
                        ch.AddEntry(youMayAlsoLikeAssociations[i].Entry, 1); // Add just one.
                    }
                }
            }
        }

        // Redirect to shopping cart.
        Response.Redirect(ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("ShoppingCart")));
    }
}
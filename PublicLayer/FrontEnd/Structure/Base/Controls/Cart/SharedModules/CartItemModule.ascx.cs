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
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms;
using Mediachase.Cms.WebUtility.Commerce;

public partial class Controls_Cart_SharedModules_CartItemModule : System.Web.UI.UserControl
{
    LineItem _LineItem = null;
    /// <summary>
    /// Gets or sets the line item.
    /// </summary>
    /// <value>The line item.</value>
    public LineItem LineItem
    {
        get
        {
            return _LineItem;
        }
        set
        {
            _LineItem = value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Binds a data source to the invoked server control and all its child controls.
    /// </summary>
    public override void DataBind()
    {        
        base.DataBind();
        BindFields();
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
    /// Binds the fields.
    /// </summary>
    void BindFields()
    {
        Entry entry = null;

        if (!String.IsNullOrEmpty(LineItem.ParentCatalogEntryId))
            entry = CatalogContext.Current.GetCatalogEntry(LineItem.ParentCatalogEntryId);
        else
            entry = CatalogContext.Current.GetCatalogEntry(LineItem.CatalogEntryId);

        if (entry != null)
        {
            EntryLink.NavigateUrl = StoreHelper.GetEntryUrl(entry);
        }
    }
}

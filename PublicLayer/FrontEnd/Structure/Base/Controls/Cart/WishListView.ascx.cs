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

using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Marketing;
using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;

public partial class Controls_Cart_WishListView : BaseStoreUserControl, ICmsDataAdapter
{
    private CartHelper _CartHelper = new CartHelper(CartHelper.WishListName);
    /// <summary>
    /// Gets the cart helper.
    /// </summary>
    /// <value>The cart helper.</value>
    public CartHelper CartHelper { get { return _CartHelper; } }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
            BindData();
    }

    /// <summary>
    /// Gets the entry image source.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected Images GetEntryImageSource(LineItem item)
    {
        Entry entry = null;

        if (!String.IsNullOrEmpty(item.ParentCatalogEntryId))
            entry = CatalogContext.Current.GetCatalogEntry(item.ParentCatalogEntryId);
        else
            entry = CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId);

        if (entry == null || entry.ItemAttributes == null)
            return null;

        return entry.ItemAttributes.Images;
    }

    /// <summary>
    /// Gets the entry.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected Entry GetEntry(LineItem item)
    {
        if (item == null)
            return null;

        return CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
    }

    /// <summary>
    /// Adds to cart.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    protected void AddToCart(Object sender, CommandEventArgs e) 
    {
        Entry entry = CatalogContext.Current.GetCatalogEntry(e.CommandArgument.ToString(), new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

        if (entry != null)
        {
            // Remove from Wish list
            CartHelper wishList = new CartHelper(CartHelper.WishListName);
            CartHelper helper = new CartHelper(Cart.DefaultName);

            decimal qty = 1;
            foreach (LineItem item in wishList.LineItems)
            {
                if (item.CatalogEntryId == entry.ID)
                {
                    qty = item.Quantity;
                    item.Delete();
                }
            }

			// If cart is empty, remove it from the database
			if (CartHelper.IsEmpty)
				CartHelper.Delete();

            // Save changes to a wish list
            wishList.Cart.AcceptChanges();

            // add to a cart
            AddToCart(helper, entry, qty);
        }

        // Redirect to shopping cart
        Response.Redirect(NavigationManager.GetUrl("ShoppingCart"));
    }

    /// <summary>
    /// Adds to cart.
    /// </summary>
    /// <param name="helper">The helper.</param>
    /// <param name="entry">The entry.</param>
    /// <param name="qty">The qty.</param>
    private void AddToCart(CartHelper helper, Entry entry, decimal qty)
    {
        bool alreadyExists = false;

        // Check if item exists
        foreach (LineItem item in helper.LineItems)
        {
            if (item.CatalogEntryId.Equals(entry.ID))
            {
                item.Quantity += qty;
                alreadyExists = true;
            }
        }

        if (!alreadyExists)
            helper.AddEntry(entry, qty);
        else
            helper.Cart.AcceptChanges();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        // Make sure to check that prices has not changed
        if (!CartHelper.IsEmpty)
        {
            CartHelper.RunWorkflow("CartValidate");
            CartHelper.Cart.AcceptChanges();
            UpdateCartButton.Visible = true;
        }
        else
        {
            UpdateCartButton.Visible = false;
        }

        ShoppingCart.DataSource = CartHelper.LineItems;
        ShoppingCart.DataBind();
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //BindData();
        if (CMSContext.Current.IsDesignMode)
            divViewer.InnerHtml = "Wish List Control";
    }

    #region Event Handlers
    /// <summary>
    /// Handles the RowDeleting event of the ShoppingCart control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
    protected void ShoppingCart_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow row = ShoppingCart.Rows[e.RowIndex];
        int lineItemId = 0;
        if (Int32.TryParse(ShoppingCart.DataKeys[e.RowIndex].Value.ToString(), out lineItemId))
        {

            foreach (LineItem item in CartHelper.LineItems)
            {
                if (item.LineItemId == lineItemId)
                    item.Delete();
            }

			// If cart is empty, remove it from the database
			if (CartHelper.IsEmpty)
				CartHelper.Delete();

            CartHelper.Cart.AcceptChanges();

            // Refresh the cart
            //RefreshCart();
            Response.Redirect(Request.RawUrl);
        }
        else
            e.Cancel = true;
    }

    /// <summary>
    /// Handles the RowCreated event of the ShoppingCart control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void ShoppingCart_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            
            if (e.Row.DataItem != null)
            {
                LineItem li = (LineItem)e.Row.DataItem;

                TextBox qtyBox = e.Row.FindControl("Quantity") as TextBox;
                if (qtyBox != null)
                {
                    qtyBox.Text = li.Quantity.ToString("#.##");
                    qtyBox.DataBind();
                }

                Label availLabel = e.Row.FindControl("AvailabilityLabel") as Label;
                LinkButton addCartLink = e.Row.FindControl("AddCartLink") as LinkButton;

                Entry entry = CatalogContext.Current.GetCatalogEntry(li.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                if (availLabel != null)
                {                    
                    // Set the inventory status                    
                    availLabel.Text = StoreHelper.GetInventoryStatus(entry);
                }

                if (addCartLink != null)
                {
                    if (StoreHelper.IsInStock(entry) || StoreHelper.IsAvailableForBackorder(entry) || StoreHelper.IsAvailableForPreorder(entry))
                        addCartLink.Visible = true;
                    else
                        addCartLink.Visible = false;
                }
            }
        }
    }

    /// <summary>
    /// Handles the Click event of the UpdateButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        int index = 0;
        foreach (LineItem item in CartHelper.LineItems)
        {
            TextBox qtyBox = ShoppingCart.Rows[index].FindControl("Quantity") as TextBox;

            decimal newQty;
            if (!Decimal.TryParse(qtyBox.Text, out newQty))
                continue;

            if (newQty <= 0) // remove
                item.Delete();
            else if (newQty != item.Quantity) // update
                item.Quantity = newQty;

            index++;
        }

		// If cart is empty, remove it from the database
		if (CartHelper.IsEmpty)
			CartHelper.Delete();
		else
			CartHelper.RunWorkflow("CartValidate");

        CartHelper.Cart.AcceptChanges();
        Response.Redirect(Request.RawUrl);
    }

    /// <summary>
    /// Handles the Click event of the ContinueButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        string lastCatalogPageUrl = Session["LastCatalogPageUrl"] as string;

        if (String.IsNullOrEmpty(lastCatalogPageUrl))
        {
            Response.Redirect(ResolveUrl("~/default.aspx"));
        }
        else
        {
            Response.Redirect(lastCatalogPageUrl);
        }
    }
    #endregion


	#region ICmsDataAdapter Members

    /// <summary>
    /// Sets the param info.
    /// </summary>
    /// <param name="param">The param.</param>
	public void SetParamInfo(object param)
	{
		ControlSettings settings = (ControlSettings)param;

		if (settings != null && settings.Params != null)
		{
			//if (settings.Params["ForumPath"] != null)
			//{
			//    news.ForumPath = settings.Params["ForumPath"].ToString();
			//}

			//    if (settings.Params["HeaderText"] != null)
			//    {
			//        rss.HeaderText = settings.Params["HeaderText"].ToString();
			//    }

			this.DataBind();
		}
	}

	#endregion
}
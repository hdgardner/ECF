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
using Mediachase.Cms.Website.Structure.Base.Controls.Cart.SharedModules;

public partial class Controls_Cart_CartView : BaseStoreUserControl, ICmsDataAdapter
{
    private CartHelper _CartHelper = new CartHelper(Cart.DefaultName);
    /// <summary>
    /// Gets the cart helper.
    /// </summary>
    /// <value>The cart helper.</value>
    public CartHelper CartHelper { get { return _CartHelper; } }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session[MarketingContext.ContextConstants.Coupons] != null)
            SetCouponCode(Session[MarketingContext.ContextConstants.Coupons].ToString());

        //if (!this.IsPostBack)
        BindData(!this.IsPostBack);
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
    /// Handles the Click event of the UpdateButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        UpdateQuantities();
        Response.Redirect(Request.RawUrl);
    }

    /// <summary>
    /// Updates the quantities.
    /// </summary>
    private void UpdateQuantities()
    {
        int index = 0;
        foreach (LineItem item in CartHelper.LineItems)
        {
            EntryQuantityControl qtyBox = ShoppingCart.Rows[index].FindControl("Quantity") as EntryQuantityControl;

            decimal newQty;

            if (qtyBox.Quantity == null)
                continue;

            newQty = (decimal)qtyBox.Quantity;

            if (newQty <= 0) // remove
                item.Delete();
            else if (newQty != item.Quantity) // update
                item.Quantity = newQty;

            index++;
        }

        CartHelper.RunWorkflow("CartValidate");
        CartHelper.Cart.AcceptChanges();
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

    /// <summary>
    /// Handles the Click event of the CheckoutButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void CheckoutButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(ResolveUrl("~/Checkout/checkout.aspx"));
    }

    /// <summary>
    /// Handles the Click event of the ApplyCouponButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ApplyCouponButton_Click(object sender, EventArgs e)
    {
        SetCouponCode(DiscountCouponCode.Text);
        Session[MarketingContext.ContextConstants.Coupons] = DiscountCouponCode.Text;
        BindData();
    }

    /// <summary>
    /// Sets the coupon code.
    /// </summary>
    /// <param name="couponCode">The coupon code.</param>
    private void SetCouponCode(string couponCode)
    {
        List<string> couponList = new List<string>();
        couponList.Add(couponCode);

        if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(MarketingContext.ContextConstants.Coupons))
            MarketingContext.Current.MarketingProfileContext.Add(MarketingContext.ContextConstants.Coupons, couponList);
        else
            MarketingContext.Current.MarketingProfileContext[MarketingContext.ContextConstants.Coupons] = couponList;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        BindData(true);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(bool validate)
    {
        // Make sure to check that prices has not changed
        if (!CartHelper.IsEmpty && validate)
        {
            CartHelper.Reset();
            CartHelper.RunWorkflow("CartValidate");

            // If cart is empty, remove it from the database
            if (CartHelper.IsEmpty)
                CartHelper.Delete();

            CartHelper.Cart.AcceptChanges();
        }

        ShoppingCart.DataSource = CartHelper.LineItems;

        // Calculate sub total
        decimal subTotalPrice = 0;

        foreach (LineItem item in CartHelper.LineItems)
        {
            subTotalPrice += item.ExtendedPrice;
        }

        // Bind Order Level Discounts
        if (CartHelper.OrderForm.Discounts.Count > 0)
        {
            DiscountList.DataSource = CartHelper.OrderForm.Discounts;
            DiscountList.DataBind();
            DiscountTr.Visible = true;
            TotalDiscount.Text = CurrencyFormatter.FormatCurrency(CartHelper.OrderForm.DiscountAmount, CartHelper.Cart.BillingCurrency);
        }
        else
        {
            DiscountTr.Visible = false; 
        }        

        SubTotal.Text = CurrencyFormatter.FormatCurrency(subTotalPrice, CartHelper.Cart.BillingCurrency);

        bool isEmpty = CartHelper.IsEmpty;
        CartSummary.Visible = !isEmpty;
        UpdateCartButton.Visible = !isEmpty;
        CheckoutButton.Visible = !isEmpty;
        CouponSummary.Visible = !isEmpty;

        ShoppingCart.DataBind();
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CMSContext.Current.IsDesignMode)
            divViewer.InnerHtml = "Wish List Control";
    }

    /// <summary>
    /// Binds the quantity.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="lineItem">The line item.</param>
    private void BindQuantity(DropDownList list, LineItem lineItem)
    {
        for(int index = 0; index < 10; index++)
        {
            ListItem item = new ListItem(index.ToString(), index.ToString());
            
            if(lineItem.Quantity == index)
                item.Selected = true;
            
            list.Items.Add(item);
        }
        list.DataBind();
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

            //Cart cart = OrderContext.Current.GetCart(Cart.DefaultName, ProfileContext.Current.UserId);

            if (!CartHelper.IsEmpty)
            {
                foreach (LineItem item in CartHelper.LineItems)
                {
                    if (item.LineItemId == lineItemId)
                        item.Delete();
                }
            }

            // If cart is empty, remove it from the database
            if (CartHelper.IsEmpty)
                CartHelper.Delete();

            CartHelper.Cart.AcceptChanges();
            Response.Redirect(Request.RawUrl);
        }
        else
            e.Cancel = true;
    }

    /// <summary>
    /// Handles the RowEditing event of the ShoppingCart control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
    protected void ShoppingCart_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GridViewRow row = ShoppingCart.Rows[e.NewEditIndex];
        int lineItemId = 0;
        if (Int32.TryParse(ShoppingCart.DataKeys[e.NewEditIndex].Value.ToString(), out lineItemId))
        {
            CartHelper cart = new CartHelper(Cart.DefaultName);
            foreach (LineItem item in cart.LineItems)
            {
                if (item.LineItemId == lineItemId)
                {
                    decimal qty = item.Quantity;
                    CartHelper helper = new CartHelper(CartHelper.WishListName);
                    Entry entry = CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                    if (entry != null)
                    {
                        AddToWishList(helper, entry, qty);
                        item.Delete();
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

			// If cart is empty, remove it from the database
			if (CartHelper.IsEmpty)
				CartHelper.Delete();

            // Save changes
            cart.Cart.AcceptChanges();

            // Redirect to wish list
            Response.Redirect(Mediachase.Cms.NavigationManager.GetUrl("WishList"));
        }
        else
            e.Cancel = true;
    }

    /// <summary>
    /// Adds to wish list.
    /// </summary>
    /// <param name="helper">The helper.</param>
    /// <param name="entry">The entry.</param>
    /// <param name="qty">The qty.</param>
    private void AddToWishList(CartHelper helper, Entry entry, decimal qty)
    {
        bool alreadyExists = false;

        // Check if item exists
        foreach (LineItem item in helper.LineItems)
        {
            if (item.CatalogEntryId.Equals(entry.ID))
            {
                alreadyExists = true;
                item.Quantity += qty;
            }
        }

        if (!alreadyExists)
            helper.AddEntry(entry, qty);
        else
            helper.Cart.AcceptChanges();
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
            EntryQuantityControl qtyBox = e.Row.FindControl("Quantity") as EntryQuantityControl;
            if (qtyBox != null)
            {
                if (e.Row.DataItem != null)
                {
                    LineItem li = (LineItem)e.Row.DataItem;
                    qtyBox.Quantity = li.Quantity;
                    qtyBox.DataBind();
                }
            }

            LinkButton linkButton = e.Row.FindControl("MoveWishList") as LinkButton;
            if (linkButton != null)
            {
                if (e.Row.DataItem != null)
                    linkButton.Visible = !((LineItem)e.Row.DataItem).CatalogEntryId.StartsWith("@");
            }
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
			this.DataBind();
		}
	}

	#endregion
}
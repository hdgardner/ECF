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

public partial class Templates_Everything_Entry_Modules_BuyModule : Mediachase.Cms.WebUtility.BaseStoreUserControl
{
    private Entry _Entry;
    private string _CatalogName;
    private string _RelatedProductsAssociationName;

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
    /// Gets or sets the name of our Related Products Association array.
    /// </summary>
    /// <value>The string name of our Related Products Association.</value>
    public string RelatedProductsAssociationName
    {
        get
        {
            return _RelatedProductsAssociationName;
        }
        set
        {
            _RelatedProductsAssociationName = value;
        }
    }

    private string _AdditionalOptionsAssociationName;
    /// <summary>
    /// Gets or sets the name of our Additional Options Association array.
    /// </summary>
    /// <value>The string name of our Additional Options Association.</value>
    public string AdditionalOptionsAssociationName
    {
        get
        {
            return _AdditionalOptionsAssociationName;
        }
        set
        {
            _AdditionalOptionsAssociationName = value;
        }
    }
    #endregion

    /// <summary>
    /// Bind data to the controls that expose inventory.
    /// </summary>
    void BindInventory()
    {
        AvailabilityLabel.Text = StoreHelper.GetInventoryStatus(Entry);

        // Disable the inventory button
        if (StoreHelper.IsInStock(Entry) || StoreHelper.IsAvailableForBackorder(Entry) || StoreHelper.IsAvailableForPreorder(Entry))
            BuyButton1.Enabled = true;
        else
            BuyButton1.Enabled = false;
    }

    /// <summary>
    /// Binds to the controls that expose pricing.
    /// </summary>
    private void BindPricing()
    {
        Price listPrice = StoreHelper.GetSalePrice(Entry, 1);
        Price salePrice = StoreHelper.GetDiscountPrice(Entry, CatalogName);
        
        // Check for nulls.
        if (listPrice != null && salePrice != null)
        {
            // Compare list and sale prices.
            if (listPrice.Amount > salePrice.Amount)
            {
                ListPriceLabel.Text = listPrice.FormattedPrice;
                PriceLabel.Text = salePrice.FormattedPrice;
                DiscountPrice.Text = CurrencyFormatter.FormatCurrency(listPrice.Amount - salePrice.Amount, salePrice.CurrencyCode);
                this.ProductInfoSavingsDiv.Visible = true;
            }
            else
            {
                this.ProductInfoSavingsDiv.Visible = false;
                PriceLabel.Text = salePrice.FormattedPrice;
            }
        }
        else
        {
            ListPriceLabel.Text = "0";
            PriceLabel.Text = "0";
            this.ProductInfoSavingsDiv.Visible = false;
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
    /// Raises the <see cref="E:System.Web.UI.Control.DataBinding"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnDataBinding(EventArgs e)
    {
        base.OnDataBinding(e);

        if (Entry != null)
        {
            BindInventory();
            BindPricing();

            Association[] assocs = Entry.Associations;

            if (assocs != null)
            {
                bool relatedProductsMatched = false;
                bool additionalOptionsMatched = false;

                foreach (Association assoc in assocs)
                {   
                    // If there is more than one association name match, the last one wins.
                    // We should prevent this in the CMS.

                    // Check for related products association name matches.
                    if (assoc.Name == RelatedProductsAssociationName)
                    {
                        if (assoc.EntryAssociations != null && assoc.EntryAssociations.Association != null)
                        {
                            RelatedProductsRepeater.DataSource = assoc.EntryAssociations.Association;
                            RelatedProductsRepeater.DataBind();

                            relatedProductsMatched = true;
                            //break;
                         }
                    }
                    
                    // Check for additional options association name matches.
                    if (assoc.Name == AdditionalOptionsAssociationName)
                    {
                        if (assoc.EntryAssociations != null && assoc.EntryAssociations.Association != null)
                        {
                            AdditionalOptionsRepeater.DataSource = assoc.EntryAssociations.Association;
                            AdditionalOptionsRepeater.DataBind();

                            additionalOptionsMatched = true;
                            //break;
                        }
                    }
                    
                    // Set related products panel visibility.
                    this.RelatedProductsPanel.Visible = relatedProductsMatched;

                    // Set additional options panel visibility.
                    this.AdditionalOptionsPanel.Visible = additionalOptionsMatched;
                }
            }
            else
            {
                this.RelatedProductsPanel.Visible = false;
                this.AdditionalOptionsPanel.Visible = false;
            }
        }
        else
        {
            this.RelatedProductsPanel.Visible = false;
            this.AdditionalOptionsPanel.Visible = false;
        }
    }

    /// <summary>
    /// Handles the Click event of the PurchaseLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
    protected void BuyButton_Click(object sender, EventArgs e, Entry entry, ref bool reject)
    {
        CartHelper ch = new CartHelper(Cart.DefaultName);

        // Check if Entry Object is null.
        if (Entry != null)
        {
            // Add item to a cart.
            ch.AddEntry(this.Entry, Int32.Parse(QuantityList.Text));
        }

        // Get the checked related products and additional options.

        // Go through the list of Related Products.
        EntryAssociation[] RelatedProductsAssociations = this.RelatedProductsRepeater.DataSource as EntryAssociation[];
        if (RelatedProductsAssociations != null)
        {
            foreach (RepeaterItem item in this.RelatedProductsRepeater.Items)
            {
                CheckBox rpc = item.FindControl("RelatedProductsCheckbox") as CheckBox;
                if (rpc != null)
                {
                    if (rpc.Checked == true)
                    {
                        foreach (EntryAssociation rpa in RelatedProductsAssociations)
                        {
                            // Match against the Entry.ID set at data binding.
                            if (rpa.Entry.ID == rpc.Text)
                            {
                                ch.AddEntry(rpa.Entry, Int32.Parse(QuantityList.Text)); // Add just one.
                            }
                        }
                    }
                }
            }
        }

        // Go through the list of Additional Options.
        EntryAssociation[] AdditionalOptionsAssociations = this.AdditionalOptionsRepeater.DataSource as EntryAssociation[];
        if (AdditionalOptionsAssociations != null)
        {
            foreach (RepeaterItem item in this.AdditionalOptionsRepeater.Items)
            {
                CheckBox aoc = item.FindControl("AdditionalOptionsCheckbox") as CheckBox;
                if (aoc != null)
                {
                    if (aoc.Checked == true)
                    {
                        foreach (EntryAssociation aoa in AdditionalOptionsAssociations)
                        {
                            // Match against the Entry.ID set at data binding.
                            if (aoa.Entry.ID == aoc.Text)
                            {
                                ch.AddEntry(aoa.Entry, Int32.Parse(QuantityList.Text)); // Add as many as main product.
                            }
                        }
                    }
                }
            }
        }

        // Redirect to shopping cart.
        //Response.Redirect(NavigationManager.GetUrl("ShoppingCart"));
    }

    /// <summary>
    /// Handles the Click event of the WishlistLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void WishlistLink_Click(object sender, EventArgs e)
    {
        CartHelper helper = new CartHelper(CartHelper.WishListName);

        // Add main entry
        AddToWishList(helper, Entry);

        // Go through the list of Related Products.
        EntryAssociation[] RelatedProductsAssociations = this.RelatedProductsRepeater.DataSource as EntryAssociation[];
        if (RelatedProductsAssociations != null)
        {
            foreach (RepeaterItem item in this.RelatedProductsRepeater.Items)
            {
                CheckBox rpc = item.FindControl("RelatedProductsCheckbox") as CheckBox;
                if (rpc != null)
                {
                    if (rpc.Checked == true)
                    {
                        foreach (EntryAssociation rpa in RelatedProductsAssociations)
                        {
                            // Match against the Entry.ID set at data binding.
                            if (rpa.Entry.ID == rpc.Text)
                            {
                                AddToWishList(helper, rpa.Entry);
                            }
                        }
                    }
                }
            }
        }

        // Go through the list of Additional Options.
        EntryAssociation[] AdditionalOptionsAssociations = this.AdditionalOptionsRepeater.DataSource as EntryAssociation[];
        if (AdditionalOptionsAssociations != null)
        {
            foreach (RepeaterItem item in this.AdditionalOptionsRepeater.Items)
            {
                CheckBox aoc = item.FindControl("AdditionalOptionsCheckbox") as CheckBox;
                if (aoc != null)
                {
                    if (aoc.Checked == true)
                    {
                        foreach (EntryAssociation aoa in AdditionalOptionsAssociations)
                        {
                            // Match against the Entry.ID set at data binding.
                            if (aoa.Entry.ID == aoc.Text)
                            {
                                AddToWishList(helper, aoa.Entry);
                            }
                        }
                    }
                }
            }
        }

        // Redirect to shopping cart
        Response.Redirect(ResolveUrl(Mediachase.Cms.NavigationManager.GetUrl("WishList")));
    }

    /// <summary>
    /// Adds to wish list.
    /// </summary>
    /// <param name="helper">The helper.</param>
    /// <param name="entry">The entry.</param>
    private void AddToWishList(CartHelper helper, Entry entry)
    {
        bool alreadyExists = false;

        // Check if item exists
        foreach (LineItem item in helper.LineItems)
        {
            if (item.CatalogEntryId.Equals(entry.ID))
            {
                alreadyExists = true;
            }
        }

        if (!alreadyExists)
            helper.AddEntry(entry);
    }
}
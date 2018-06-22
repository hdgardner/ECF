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
using Mediachase.Cms;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog;
using Mediachase.MetaDataPlus;
using System.Threading;
using System.Collections.Generic;
using Mediachase.Commerce.Shared;

/// <summary>
///	This module is responsible for comparing products.
/// </summary>
public partial class Templates_Everything_Compare_ProductsCompareView : BaseStoreUserControl
{
    private string _CurrentMetaClassName;
    private MetaClass _CurrentMetaClass;

    private Entry[] _ProductsToCompare;

    /// <summary>
    /// Gets the metaclass name.
    /// </summary>
    /// <value>The metaclass name.</value>
    public string CurrentMetaClassName
    {
        get
        {
            if (String.IsNullOrEmpty(_CurrentMetaClassName))
                _CurrentMetaClassName = Request.QueryString["mc"];

            if (String.IsNullOrEmpty(_CurrentMetaClassName))
            {
                string[] mcs = CommonHelper.GetCompareMetaClasses();
                if (mcs != null && mcs.Length > 0)
                    _CurrentMetaClassName = mcs[0];
            }

            return _CurrentMetaClassName;
        }
    }

    /// <summary>
    /// Gets the metaclass.
    /// </summary>
    /// <value>The metaclass.</value>
    public MetaClass CurrentMetaClass
    {
        get
        {
            if (_CurrentMetaClass == null)
            {
                //if (MetaDataContext.Current == null)
                //	MetaDataContext.Current = CatalogContext.MetaDataContext;

                try
                {
                    _CurrentMetaClass = MetaClass.Load(CatalogContext.MetaDataContext, CurrentMetaClassName);
                }
                catch { }
            }

            return _CurrentMetaClass;
        }
    }

    /// <summary>
    /// Gets the products to compare.
    /// </summary>
    /// <value>The products to compare.</value>
    public Entry[] ProductsToCompare
    {
        get
        {
            if (_ProductsToCompare == null && !String.IsNullOrEmpty(CurrentMetaClassName))
            {
                Entries prods = CommonHelper.GetCompareProducts(CurrentMetaClassName);
                _ProductsToCompare = prods.Entry;
            }

            return _ProductsToCompare;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CompareProducts_js", CommerceHelper.GetAbsolutePath("/Scripts/CompareProducts.js"));
        Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CompareViewPageUrl", String.Format("CSCompareProducts.CompareViewPageUrl = \"{0}\";", CMSContext.Current.ResolveUrl("~/compare.aspx")), true);

        //if (!IsPostBack)
        {
            if (!String.IsNullOrEmpty(CurrentMetaClassName) && ProductsToCompare != null)
            {
                hfCurrentComparisonGroup.Value = CurrentMetaClassName;
                BindData();
                pnlCompareProducts.Visible = true;
                ClearCompareButton.Text = RM.GetString("COMPAREPRODUCTSMODULE_CLEAR");
            }
            else
                pnlCompareProducts.Visible = false;
        }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        // set current metadata context and metadata language
        /*
		if (MetaDataContext.Current == null)
			MetaDataContext.Current = CatalogContext.MetaDataContext;
         * */
        if (!CatalogContext.MetaDataContext.UseCurrentUICulture)
            CatalogContext.MetaDataContext.Language = Thread.CurrentThread.CurrentCulture.Name;


        EnsureChildControls();

        CompareGroupsRepeater.ItemCreated += new RepeaterItemEventHandler(CompareGroupsRepeater_ItemCreated);

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the ItemCreated event of the CompareGroupsRepeater control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
    void CompareGroupsRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        string metaClassName = e.Item.DataItem as string;
        if (String.IsNullOrEmpty(metaClassName))
            return;

        HyperLink hl = e.Item.FindControl("hlCompareGroup") as HyperLink;
        Label lbl = e.Item.FindControl("lblCompareGroup") as Label;

        MetaClass mc = MetaClass.Load(CatalogContext.MetaDataContext, metaClassName);
        string metaClassFriendlyName = (mc != null) ? mc.FriendlyName : metaClassName;

        if (String.Compare(metaClassName, CurrentMetaClassName, true) == 0)
        {
            lbl.Visible = true;
            lbl.Text = metaClassFriendlyName;
            hl.Visible = false;
        }
        else
        {
            lbl.Visible = false;
            hl.Visible = true;
            hl.Text = metaClassFriendlyName;
        }

        if (!String.IsNullOrEmpty(metaClassName) && hl != null && hl.Visible)
        {
            //hl.NavigateUrl = String.Format("{0}?mc={1}", Page.ResolveUrl(Request.Url.AbsolutePath), metaClassName);
            hl.NavigateUrl = String.Format("javascript:CSCompareProducts.OpenCompareView('{0}');", metaClassName); 
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    public void BindData()
    {
        string[] mcs = CommonHelper.GetCompareMetaClasses();
        CompareGroupsRepeater.DataSource = mcs;
        CompareGroupsRepeater.DataBind();

        rptrHeaderImage.DataBind();
        rptrHeaderPurchaseLink.DataBind();

        List<CompareItem> listCompare = new List<CompareItem>();
        if (CurrentMetaClass != null)
        {
            MetaFieldCollection metaFields = CurrentMetaClass.UserMetaFields;
            if (metaFields != null && metaFields.Count > 0)
            {
                foreach (MetaField metaField in metaFields)
                {
                    string useInComparingAttribute = metaField.Attributes["UseInComparing"];
                    if (!string.IsNullOrEmpty(useInComparingAttribute) && bool.Parse(useInComparingAttribute))
                    {
                        CompareItem compareItem = new CompareItem();
                        compareItem.Title = metaField.FriendlyName;

                        List<string> items = new List<string>();
                        foreach (Entry product in ProductsToCompare)
                        {
                            string compareValue = String.Empty;

                            ItemAttribute item = product.ItemAttributes[metaField.Name];
                            if (item != null && item.Value != null && item.Value.Length > 0)
                            {
                                compareValue = item.Value[0];
                            }
                            items.Add(compareValue);
                        }

                        if (items.Count > 1)
                        {
                            for (int i = 1; i < items.Count; i++)
                            {
                                if (String.Compare(items[i - 1], items[i], true) != 0)
                                    break;

                                if (i == items.Count - 1)
                                    compareItem.EqualValues = true;
                            }
                        }

                        compareItem.Attributes = items.ToArray();
                        listCompare.Add(compareItem);
                    }
                }
            }
        }

        rptrMainTable.DataSource = listCompare;
        rptrMainTable.DataBind();
    }

    /// <summary>
    /// Handles the Command event of the WishListLink control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    public void WishlistLink_Command(object sender, CommandEventArgs e)
    {
        Entry variation = null;
        Entry product = null;
        int productId;

        if (String.Compare(e.CommandName, "AddToWishList") == 0)
        {
            string arg = e.CommandArgument as string;
            if (String.IsNullOrEmpty(arg))
            {
                Response.Redirect(Request.RawUrl);
                return;
            }

            CartHelper helper = new CartHelper(CartHelper.WishListName);

            bool alreadyExists = false;
            productId = Int32.Parse(arg);

            // Check if item exists
            foreach (LineItem item in helper.LineItems)
            {
                if (item.CatalogEntryId.Equals(productId))
                {
                    alreadyExists = true;
                    Response.Redirect(Request.RawUrl);
                    return;
                }
            }

            if (_ProductsToCompare != null && _ProductsToCompare.Length > 0)
            {
                foreach (Entry productTemp in _ProductsToCompare)
                    if (productTemp.CatalogEntryId == productId)
                    {
                        product = productTemp;

                        if (product.EntryType.Equals("Variation") || product.EntryType.Equals("Package"))
                        {
                            variation = product;
                        }
                        else if (product.Entries.Entry != null && product.Entries.Entry.Length > 0)
                        {
                            variation = product.Entries.Entry[0];
                            break;
                        }
                    }

                // if variation is found, add it to cart and redirect to product view page
                if (variation != null)
                {
                    if (!alreadyExists)
                        helper.AddEntry(variation);

                    Response.Redirect(Request.RawUrl);
                }
            }
            else
            {
                Response.Redirect(Request.RawUrl);
                return;
            }
        }

    }

    /// <summary>
    /// Handles the Command event of the removeButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    public void removeButton_Command(object sender, CommandEventArgs e)
    {
        if (String.Compare(e.CommandName, "Remove", true) == 0)
        {
            string[] args = ((string)e.CommandArgument).Split(';');
            if (args.Length == 2)
            {
                int result = CommonHelper.RemoveProductFromCookie(args[0], args[1]);
                //upProducts.Update();
            }
            Response.Redirect(Request.RawUrl);
        }
    }

    /// <summary>
    /// Handles the Click event of the BuyButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void BuyButton_Click(object sender, EventArgs e, Entry entry, ref bool reject)
    {
        CartHelper ch = new CartHelper(Cart.DefaultName);

        // Check if Entry Object is null.
        if (entry != null)
        {
            // Add item to a cart.
            ch.AddEntry(entry);
        } 
    }

    /// <summary>
    /// Handles the Click event of the ClearCompareButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void ClearCompareButton_Click(object sender, EventArgs e)
    {
        CommonHelper.ClearCompareCookie();
        Response.Redirect(Request.RawUrl);
    }

    /// <summary>
    /// Gets the entry price.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <returns></returns>
    public Price GetEntryPrice(Entry product)
    {
        Price price = null;
        if (product != null)
        {
            // ????? return sale price for the current customer, all cusmomers,...?
            //if(product.SalePrices!=null && product.SalePrices.SalePrice!=null && product.SalePrices.SalePrice.Length>0)
            //{
            //    product.SalePrices.SalePrice[0].UnitPrice;
            //}

            // TODO: check if products is available and is in stock
            if (product.EntryType.Equals("Variation") || product.EntryType.Equals("Package"))
            {
                if (product.ItemAttributes != null)
                    price = product.ItemAttributes.ListPrice;
            }
            else
            {
                if (product.Entries.Entry != null && product.Entries.Entry.Length > 0 &&
                    product.Entries.Entry[0].ItemAttributes != null)
                    price = product.Entries.Entry[0].ItemAttributes.ListPrice;
            }

        }
        return price;
    }

    public class CompareItem
    {
        public string Title = String.Empty;
        public bool EqualValues = false;
        public string[] Attributes = new string[] { };
    }
}

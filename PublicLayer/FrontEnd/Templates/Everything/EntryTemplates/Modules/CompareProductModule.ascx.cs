using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus.Configurator;

/// <summary>
///	This module is responsible for comparing products.
/// </summary>
public partial class Templates_Everything_Entry_Modules_CompareProductModule : BaseStoreUserControl
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

            //if (String.IsNullOrEmpty(_CurrentMetaClassName))
            //{
            //    string[] mcs = CommonHelper.GetCompareMetaClasses();
            //    if (mcs != null && mcs.Length > 0)
            //        _CurrentMetaClassName = mcs[0];
            //}

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
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CompareProducts_js", CommerceHelper.GetAbsolutePath("/Scripts/CompareProducts.js"));
        Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CompareViewPageUrl", String.Format("CSCompareProducts.CompareViewPageUrl = \"{0}\";", CMSContext.Current.ResolveUrl("~/compare.aspx")), true);
		BindData();
	}

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
	protected override void OnInit(EventArgs e)
	{
        if (!CatalogContext.MetaDataContext.UseCurrentUICulture)
            CatalogContext.MetaDataContext.Language = Thread.CurrentThread.CurrentCulture.Name;

		EnsureChildControls();

		base.OnInit(e);
	}


    /// <summary>
    /// Handles the ItemCreated event of the CompareEntriesRepeater control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
    public void CompareEntriesRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
	{
        ImageButton removeButton = e.Item.FindControl("ibRemove") as ImageButton;
        if(removeButton != null)
        {
            removeButton.CommandName = "Remove";
            removeButton.Command +=new CommandEventHandler(removeButton_Command);
            removeButton.CommandArgument = String.Format("{0};{1}", ((Entry)e.Item.DataItem).CatalogEntryId, ((EntryGroupCollection)(((RepeaterItem)(((System.Web.UI.Control)sender).Parent)).DataItem)).MetaClassName);
        }
	}

    /// <summary>
    /// Handles the ItemCreated event of the CompareGroupsRepeater control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
    public void CompareGroupsRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
	{
        Button btnCompare = e.Item.FindControl("btnCompare") as Button;
        if(btnCompare != null)
        {
            btnCompare.OnClientClick = String.Format("javascript:CSCompareProducts.OpenCompareView('{0}');", ((EntryGroupCollection)e.Item.DataItem).MetaClassName); 
        }
	}

    /// <summary>
    /// Binds the data.
    /// </summary>
	private void BindData()
	{
        List<EntryGroupCollection> entryGroups = CommonHelper.GetCompareGroupedProducts();
        if (entryGroups.Count > 0)
        {
            CompareGroupsRepeater.DataSource = entryGroups;
            CompareGroupsRepeater.DataBind();
            lblInfo.Visible = false;
            ClearCompareButton.Visible = true;
        }
        else
        {
            lblInfo.Text = "You have no items to compare.";
            lblInfo.Visible = true;
            ClearCompareButton.Visible = false;
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
                Response.Redirect(Request.RawUrl);
			}
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
	private Price GetEntryPrice(Entry product)
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
}

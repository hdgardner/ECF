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
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility;

public partial class Controls_Catalog_SharedModules_PriceModule : BaseStoreUserControl
{
    /// <summary>
    /// Gets or sets the list price.
    /// </summary>
    /// <value>The list price.</value>
    public Price ListPrice { get { return (Price)ViewState["ListPrice"]; } set { ViewState["ListPrice"] = value; } }
    /// <summary>
    /// Gets or sets the sale price.
    /// </summary>
    /// <value>The sale price.</value>
    public Price SalePrice { get { return (Price)ViewState["Price"]; } set { ViewState["Price"] = value; } }


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
        BindTable();
    }

    /// <summary>
    /// Binds the table.
    /// </summary>
    private void BindTable()
    {
        // Bind Price
        if (ListPrice != null && SalePrice != null)
        {
            if (ListPrice.Amount > SalePrice.Amount)
            {
                ListPriceLabel.Text = ListPrice.FormattedPrice;
                PriceLabel.Text = SalePrice.FormattedPrice;
                DiscountPrice.Text = CurrencyFormatter.FormatCurrency(ListPrice.Amount - SalePrice.Amount, SalePrice.CurrencyCode);
                DiscountPricePanel.Visible = true;
                ListPricePanel.Visible = true;
            }
            else
                PriceLabel.Text = SalePrice.FormattedPrice;
        }
        else
        {
            ListPriceLabel.Text = "0";
            PriceLabel.Text = "0";
            DiscountPricePanel.Visible = false;
            ListPricePanel.Visible = false;
        }
    }
}

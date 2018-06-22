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
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Orders;

public partial class Controls_Cart_SharedModules_PriceLineModule : BaseStoreUserControl
{
    private LineItem _LineItem;
    /// <summary>
    /// Gets or sets the line item.
    /// </summary>
    /// <value>The line item.</value>
    public LineItem LineItem { get { return _LineItem; } set { _LineItem = value; } }

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
        if (LineItem != null)
        {
            string currencyCode = LineItem.Parent.Parent.BillingCurrency;
            if (LineItem.LineItemDiscountAmount > 0)
            {
                ListPriceLabel.Text = CurrencyFormatter.FormatCurrency(LineItem.ListPrice, currencyCode);
                //PriceLabel.Text = CurrencyFormatter.FormatCurrency(LineItem.PlacedPrice - LineItem.LineItemDiscountAmount, currencyCode);
				PriceLabel.Text = CurrencyFormatter.FormatCurrency(LineItem.ExtendedPrice / LineItem.Quantity, currencyCode);
                DiscountPrice.Text = CurrencyFormatter.FormatCurrency(LineItem.LineItemDiscountAmount, currencyCode);
                DiscountPricePanel.Visible = true;
                ListPricePanel.Visible = true;
            }
            else
                PriceLabel.Text = CurrencyFormatter.FormatCurrency(LineItem.ListPrice, currencyCode);
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

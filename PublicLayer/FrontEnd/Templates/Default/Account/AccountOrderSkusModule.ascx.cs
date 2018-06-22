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
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
	public partial class AccountOrderSkusModule : BaseStoreUserControl
    {
        private PurchaseOrder _order = null;

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
		public PurchaseOrder Order
        {
            get
            {
                return _order;
            }
            set
            {
                _order = value;
            }
        }

        /// <summary>
        /// Binds the table.
        /// </summary>
		private void BindTable()
		{
			if (Order != null && Order.OrderForms != null && Order.OrderForms.Count > 0)
			{
				OrderForm of = Order.OrderForms[0];
				this.CustomerDiscount.Text = "-" + CurrencyFormatter.FormatCurrency(of.DiscountAmount, Order.BillingCurrency);  //ClientHelper.FormatCurrency(Order.Discount.Amount, Order.Discount.CurrencyCode);
				this.SubTotal.Text = CurrencyFormatter.FormatCurrency(of.DiscountAmount + of.SubTotal, Order.BillingCurrency); //ClientHelper.FormatCurrency(Order.SubTotal.Amount + Order.Discount.Amount, Order.SubTotal.CurrencyCode);
				this.Total.Text = CurrencyFormatter.FormatCurrency(of.SubTotal + of.ShippingTotal + of.HandlingTotal + of.TaxTotal, Order.BillingCurrency);  //ClientHelper.FormatCurrency(Order.SubTotal.Amount + Order.ShippingCost.Amount + Order.Charge.Amount + Order.Tax.Amount, Order.SubTotal.CurrencyCode);

				this.Shipping.Text = CurrencyFormatter.FormatCurrency(of.ShippingTotal + of.HandlingTotal, Order.BillingCurrency); //ClientHelper.FormatCurrency(Order.ShippingCost.Amount + Order.Charge.Amount, Order.ShippingCost.CurrencyCode);
				this.Taxes.Text = CurrencyFormatter.FormatCurrency(of.TaxTotal, Order.BillingCurrency); //ClientHelper.FormatCurrency(Order.Tax.Amount, Order.Tax.CurrencyCode);
								

				BillingAddress.AddressInfo = GetAddress(of.BillingAddressId);
				BillingAddress.DataBind();
			}
		}

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <returns></returns>
		protected OrderAddress GetAddress(string addressId)
		{
            if (addressId == null)
                return null;

            foreach (OrderAddress address in Order.OrderAddresses)
            {
                if (address.Name.ToUpper() == addressId.ToUpper())
                    return address;
            }

            return null;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.BindTable();
		}
	}
}

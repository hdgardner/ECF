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
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
	public partial class AccountOrderInfoModule : BaseStoreUserControl
    {
		private const string _ProcessedStatusString = "Processed";

        private PurchaseOrder _order = null;
		private int _PaymentsRowIndex = 0;

        /// <summary>
        /// Gets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                object id = Request.QueryString["order"];
                if (id == null)
                {
                    object id2 = Request.Form["invoice"];
                    if (id != null)
                        return Int32.Parse(id2.ToString());

                    return 0;
                }
                else
                    return Int32.Parse(id.ToString());
            }
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
		protected PurchaseOrder Order
        {
            get
            {
				if (_order == null)
				{
					if (this.OrderGroupId > 0)
					{
						_order = OrderContext.Current.GetPurchaseOrder(ProfileContext.Current.UserId, OrderGroupId);
					}

					/*if (_order == null)
						throw new AccessDenied();*/
				}

                return _order;
            }
        }

        /// <summary>
        /// Gets the line item.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        protected LineItem GetLineItem(int lineItemId)
        {
			if (Order != null && Order.OrderForms != null)
			{
				foreach (OrderForm of in Order.OrderForms)
				{
					foreach (LineItem li in of.LineItems)
						if (li.LineItemId == lineItemId)
							return li;
				}
			}
            
            return null;
        }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
		protected decimal GetDiscount(int lineItemId)
        {
			decimal totalDiscount = 0;
			
			LineItem item = GetLineItem(lineItemId);
			if (item != null)
				totalDiscount = item.LineItemDiscountAmount;

			return totalDiscount;
        }

        /// <summary>
        /// Makes the sku string.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
		protected string MakeSkuString(int lineItemId)
        {
            LineItem item = null;
            if((item = GetLineItem(lineItemId)) != null)
                return item.DisplayName;
            return String.Empty;
        }

        /// <summary>
        /// Gets the order payment status.
        /// </summary>
        /// <returns></returns>
		protected string GetOrderPaymentStatus()
		{
			// returns "Processed" if all payments' status is "Processed"; otherwise returns "Not Processed"

			//if (Order != null && Order.OrderForms != null && Order.OrderForms.Count > 0)
			{
				bool processed = true;
				if (Order.OrderForms[0].Payments.Count == 0)
					processed = false;
				else
				{
					foreach (Payment payment in Order.OrderForms[0].Payments)
					{
						if (String.Compare(payment.Status, _ProcessedStatusString, true) != 0)
						{
							processed = false;
							break;
						}
					}
				}
				return processed ? "Processed" : "Not Processed";
			}
			//return "N/A";
		}

        /// <summary>
        /// Gets the index of the current payments row.
        /// </summary>
        /// <returns></returns>
		public int GetCurrentPaymentsRowIndex()
		{
		    return _PaymentsRowIndex;
		}

        public string GetExpirationDate(object date)
        {
            if (date == null)
                return "";
            else
                return ((DateTime)date).ToString("MM/yyyy");
        }

		/*
        protected decimal GetTotalItemPrice(ShipmentPackageItemInfo item)
        {
            return item.Price.Amount * item.Quantity + GetDiscount(item.SkuId);
        }*/

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //if (!IsPostBack)
                this.BindData();
        }

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
		}

        /// <summary>
        /// Handles the ItemCreated event of the Payments control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
		void Payments_ItemCreated(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				this._PaymentsRowIndex++;
				HtmlGenericControl div = e.Item.FindControl("divPayment") as HtmlGenericControl;
				if (div != null)
				{
					if (e.Item.ItemType == ListItemType.AlternatingItem)
						div.Attributes["class"] = "ecf-table-item-alt";
					else
						div.Attributes["class"] = "ecf-table-item";
				}
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
			if (Order != null && Order.OrderForms != null && Order.OrderForms.Count > 0)
            {
                DataBind();

				OrderPaymentProcessed.Text = GetOrderPaymentStatus();

				// find billing address
				OrderForm oForm = Order.OrderForms[0];
				if (!String.IsNullOrEmpty(oForm.BillingAddressId))
				{
					OrderAddressCollection addresses = Order.OrderAddresses;
					OrderAddress billingAddress = null;
					foreach (OrderAddress tmpAddress in addresses)
					{
						if (String.Compare(tmpAddress.Name, oForm.BillingAddressId, true) == 0)
						{
							billingAddress = tmpAddress;
							break;
						}
					}
					if (billingAddress != null)
						OrderEmail.Text = billingAddress.Email;
				}

                // Hide skus list if shipments already exist
                /*if (Order.Shipments.Length > 0)
                {
                    SkusPurchased.Visible = false;
                }
                else
                {
                    SkusPurchased.Visible = true;
                }*/
            }
        }

        /// <summary>
        /// Gets the shipping address.
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

		/*
        private decimal BindTaxes(Repeater rept)
        {
            decimal total = 0;
            if (Order.Shipments != null && Order.Shipments.Length > 0)
            {
                SiteListItem[] taxes = ClientList.GetTaxes(Order.Shipments[0].Details.DeliveryAddress);
                if (taxes != null && taxes.Length != 0)
                {
                    foreach (SiteListItem tax in taxes)
                    {
                        total += tax.ItemAttributes.ListPrice.Amount;
                    }
                }
                rept.DataSource = taxes;
                rept.DataBind();
            }

            return total;
        }
		*/

		//public int GetCurrentRowIndex()
		//{
		//    return _RowIndex;
		//}

		/*
        protected void ShippingsRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                this._RowIndex++;
            }
        }

        protected void ShippingsRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            decimal tax = 0;
            Repeater taxRepeater = e.Item.FindControl("TaxList") as Repeater;
            if (taxRepeater != null)
            {
                tax = BindTaxes(taxRepeater);
            }

            ShipmentPackageInfo spi = e.Item.DataItem as ShipmentPackageInfo;

            decimal subtotal = 0;
            Label lblSubtotal = e.Item.FindControl("lblItemsSubtotal") as Label;
            if (lblSubtotal != null)
            {
                if(spi!=null)
                {
                    foreach (ShipmentPackageItemInfo spItem in spi.Details.Items)
                        subtotal += spItem.Quantity * spItem.Price.Amount;
                    lblSubtotal.Text = ClientHelper.FormatCurrency(subtotal, spi.Details.Items[0].Price.CurrencyCode);
                }
            }

            Label lblShTotal = e.Item.FindControl("lblShipmentTotal") as Label;
            if (lblShTotal != null && spi != null)
                lblShTotal.Text = ClientHelper.FormatCurrency(tax + subtotal + spi.ShippingCost.Amount, Order.ShippingCost.CurrencyCode);
        }*/
    }
}

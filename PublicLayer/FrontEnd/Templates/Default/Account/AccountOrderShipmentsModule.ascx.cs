using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
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
	/// <summary>
    ///		Summary description for AccountOrderShipmentsModule.
	/// </summary>
	public partial class AccountOrderShipmentsModule : BaseStoreUserControl
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
        /// Gets the shipment status.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        protected string GetShipmentStatus(int index)
        {
			return ""; //String.Format(RM.GetString("ACCOUNT_ORDER_SHIPMENT_PROCESSING"), index + 1, Order.Shipments[index].ShipmentDate);
        }


        /// <summary>
        /// Gets the order item.
        /// </summary>
        /// <param name="SkuId">The sku id.</param>
        /// <returns></returns>
        protected LineItem GetOrderItem(int SkuId)
        {
            /*if (Order != null && Order.OrderItems != null)
            {
                foreach (OrderItem item in Order.OrderItems)
                {
                    if (item.SkuId == SkuId)
                        return item;
                }
            }*/
            return null;
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			if (Order != null && Order.OrderForms != null && Order.OrderForms.Count > 0)
			{
				OrderForm form = Order.OrderForms[0];
				if (form.Shipments != null & form.Shipments.Count > 0)
				{
					//ShipmentPackageInfo[] shipments = Order.Shipments;
					this.ShipmentList.DataSource = form.Shipments;
					this.ShipmentList.DataBind();
				}
			}
		}

        /// <summary>
        /// Gets the shipping address.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <returns></returns>
		protected OrderAddress GetShippingAddress(string addressId)
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
        /// Gets the shipment line items.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
		protected LineItem[] GetShipmentLineItems(Shipment shipment)
		{
			List<LineItem> items = new List<LineItem>();
			foreach (string key in shipment.LineItemIndexes)
			{
                items.Add(shipment.Parent.LineItems[Int32.Parse(key)]);
			}

			return items.ToArray();
		}


        /// <summary>
        /// Handles the ItemDataBound event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void ShipmentList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
				Shipment sh = e.Item.DataItem as Shipment;

                //decimal subtotal = 0;
                //decimal discount = 0;
                Label lblSubtotal = e.Item.FindControl("SubTotal") as Label;
                if (lblSubtotal != null)
                {
                    if (sh != null)
                    {
						lblSubtotal.Text = GetShipmentSubtotal(sh);
                    }
                }

                Label shippingcost = e.Item.FindControl("ShippingCost") as Label;
                if (shippingcost != null)
                {
                    shippingcost.Text = CurrencyFormatter.FormatCurrency(sh.ShipmentTotal, Order.BillingCurrency); //(spi.ShippingCost.Amount + spi.Charge.Amount, spi.ShippingCost.CurrencyCode);
                }

                HyperLink trackingurl = e.Item.FindControl("TrackingUrl") as HyperLink;
                if (trackingurl != null && !String.IsNullOrEmpty(sh.ShipmentTrackingNumber))
                {
                    trackingurl.NavigateUrl = String.Format("http://www.google.com/search?q={0}", sh.ShipmentTrackingNumber);
					trackingurl.Text = RM.GetString("ACCOUNT_ORDER_SHIPMENT_TRACK");
                    trackingurl.Visible = true;
                }

                Label taxes = e.Item.FindControl("Taxes") as Label;
                if (taxes != null)
                {
					taxes.Text = CurrencyFormatter.FormatCurrency(0m, Order.BillingCurrency);
                }

                Label total = e.Item.FindControl("TotalCost") as Label;
                if (total != null)
                {
					total.Text = GetShipmentTotal(sh); //Tax.Amount + spi.ShippingCost.Amount + spi.Charge.Amount + subtotal - spi.Discount.Amount, spi.ShippingCost.CurrencyCode);
                }

                // Print discount price
                /*Label discountLabel = e.Item.FindControl("ShipmentDiscount") as Label;
                if (discountLabel != null)
                {
					discountLabel.Text = "-" + CurrencyFormatter.FormatCurrency(sh.ShippingDiscountAmount, Order.BillingCurrency); //discount, spi.ShippingCost.CurrencyCode);
                    TableRow discountRow = e.Item.FindControl("ShipmentDiscount") as TableRow;
                    if (discountRow != null)
                    {
                        if (discount == 0)
                            discountRow.Visible = false;
                        else
                            discountRow.Visible = true;
                    }
                }*/

                // Print whole order shipment discount price
                Label discountShipmentLabel = e.Item.FindControl("lblTotalShipmentDiscount") as Label;
                if (discountShipmentLabel != null)
                {
					discountShipmentLabel.Text = "-" + CurrencyFormatter.FormatCurrency(sh.ShippingDiscountAmount, Order.BillingCurrency); //ClientHelper.FormatCurrency(spi.Discount.Amount, spi.Discount.CurrencyCode);
                    TableRow discountRow = e.Item.FindControl("trTotalShipmentDiscount") as TableRow;
                    if (discountRow != null)
                    {
                        if (sh.ShippingDiscountAmount > 0)
                            discountRow.Visible = true;
                        else
                            discountRow.Visible = false;
                    }
                }

				GridView gv = (GridView)e.Item.FindControl("gvOrders");
				if (gv != null && gv.Columns.Count > 1)
				{
					gv.Columns[0].HeaderText = RM.GetString("ACCOUNT_ORDER_SHIPMENT_ITEMS_ORDERED");
					gv.Columns[1].HeaderText = RM.GetString("ACCOUNT_ORDER_SHIPMENT_PRICE");
					gv.DataBind();
				}
			
            }
        }

        /// <summary>
        /// Gets the shipment subtotal.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
		protected string GetShipmentSubtotal(Shipment shipment)
		{
			decimal price = 0m;
			foreach (LineItem item in this.GetShipmentLineItems(shipment))
			{
				price += item.ExtendedPrice;
			}

			return CurrencyFormatter.FormatCurrency(price, Order.BillingCurrency);
		}

        /// <summary>
        /// Gets the shipment total.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
		protected string GetShipmentTotal(Shipment shipment)
		{
			decimal price = 0m;
			foreach (LineItem item in this.GetShipmentLineItems(shipment))
			{
				price += item.ExtendedPrice;
			}
			price += shipment.ShippingDiscountAmount;
			price += shipment.ShipmentTotal;

			return CurrencyFormatter.FormatCurrency(price, Order.BillingCurrency);
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.BindData();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		protected void ApplyLocalization()
		{
			
		}
	}
}

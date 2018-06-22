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
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility.Commerce;
using System.Collections.Generic;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
	/// <summary>
    ///		Summary description for CheckoutOrderShipmentsModule.
	/// </summary>
    public partial class CheckoutOrderShipmentsModule : BaseStoreUserControl
	{
        private OrderForm _order = null;

        /// <summary>
        /// Gets and sets the OrderForm
        /// </summary>
        public OrderForm Order { get { return _order; } set { _order = value; } }

        private OrderGroup _orderGroup = null;

        public OrderGroup OrderGroup 
        { 
            get 
            { 
                return _orderGroup; 
            } 
            set 
            { 
                _orderGroup = value;
                Order = _orderGroup.OrderForms[0];
            } 
        }

        /// <summary>
        /// Gets the total price formatted.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        protected string GetTotalPriceFormatted(LineItem item)
        {
            return CurrencyFormatter.FormatCurrency(item.ListPrice * item.Quantity, OrderGroup.BillingCurrency);
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

            foreach (OrderAddress address in OrderGroup.OrderAddresses)
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
            foreach(string key in shipment.LineItemIndexes)
            {
                items.Add(shipment.Parent.LineItems[Int32.Parse(key)]);
            }            

            return items.ToArray();
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

            return CurrencyFormatter.FormatCurrency(price, OrderGroup.BillingCurrency);
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

            price -= shipment.ShippingDiscountAmount;
            price += shipment.ShipmentTotal;

            return CurrencyFormatter.FormatCurrency(price, OrderGroup.BillingCurrency);
        }

        /// <summary>
        /// Gets the shipment discount total.
        /// </summary>
        /// <param name="shipment">The shipment.</param>
        /// <returns></returns>
        protected string GetShipmentDiscountTotal(Shipment shipment)
        {
            decimal price = 0m;
            foreach (LineItem item in this.GetShipmentLineItems(shipment))
            {
                price += item.LineItemDiscountAmount;
            }

            price += shipment.ShippingDiscountAmount;

            return CurrencyFormatter.FormatCurrency(price, OrderGroup.BillingCurrency);
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.OrderFormList.DataSource = OrderGroup.OrderForms;
            this.OrderFormList.DataBind();
        }

        /// <summary>
        /// Handles the ItemDataBound event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void ShipmentList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {            
            base.DataBind();
            this.BindData();
        }

        /// <summary>
        /// Prepares the step.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void PrepareStep(object sender, EventArgs e)
        {
            
        }
	}
}

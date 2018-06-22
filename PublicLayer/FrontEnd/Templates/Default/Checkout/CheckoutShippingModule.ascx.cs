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
using Mediachase.Cms.WebUtility.Commerce;
using System.Collections.Generic;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CheckoutShippingModule : BaseStoreUserControl
    {
        private CartHelper _CartHelper = null;
        /// <summary>
        /// Gets or sets the cart helper.
        /// </summary>
        /// <value>The cart helper.</value>
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }



        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            
        }

        /// <summary>
        /// Prepares the step.
        /// </summary>
        public void PrepareStep()
        {
            BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        protected void BindData()
        {
            ArrayList addresses = new ArrayList();
            string billingAddressId = CartHelper.OrderForm.BillingAddressId;

            foreach (OrderAddress address in CartHelper.Cart.OrderAddresses)
            {
				// add only shipping addresses to the list
				if (!billingAddressId.Equals(address.Name, StringComparison.OrdinalIgnoreCase))
                    addresses.Add(address);
            }

            ShippingsList.DataSource = addresses;
            ShippingsList.DataBind();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        protected LineItem[] GetItems(OrderAddress address)
        {
            List<LineItem> items = new List<LineItem>();
            foreach (LineItem item in CartHelper.LineItems)
            {
                if (item.ShippingAddressId.Equals(address.Name, StringComparison.OrdinalIgnoreCase))
                {
                    items.Add(item);
                }
            }

            return items.ToArray();
        }
    }
}
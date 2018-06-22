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
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Orders;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    public partial class CheckoutOrderSkusModule : BaseStoreUserControl
    {
        private CartHelper _CartHelper = null;
        /// <summary>
        /// Gets or sets the cart helper.
        /// </summary>
        /// <value>The cart helper.</value>
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }

        /// <summary>
        /// Gets the order form.
        /// </summary>
        /// <value>The order form.</value>
        public OrderForm OrderForm
        {
            get
            {
                return CartHelper.Cart.OrderForms[0];
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
    		
		}
	}
}
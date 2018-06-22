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

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CheckoutThankYouModule : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            DataBind();
            NonRegisteredUsersMessage.Visible = !base.Page.User.Identity.IsAuthenticated;
            RegisteredUsersMessage.Visible = !NonRegisteredUsersMessage.Visible;

            if(Session["LatestOrderId"] == null)
                Response.Redirect("~");

            PurchaseOrder order = OrderContext.Current.GetPurchaseOrder(ProfileContext.Current.UserId, (int)Session["LatestOrderId"]);

            if (order == null)
                Response.Redirect("~");


            OrderNumber.Text = order.TrackingNumber;
            OrderTotal.Text = CurrencyFormatter.FormatCurrency(order.Total, order.BillingCurrency);
        }
    }
}

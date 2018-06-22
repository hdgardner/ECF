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
using Mediachase.Cms.Util;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    public partial class AccountOrdersModule : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                BindTable();
        }

        /// <summary>
        /// Binds the table.
        /// </summary>
        private void BindTable()
        {
			OrdersList.Columns[0].HeaderText = RM.GetString("ACCOUNT_ORDERS_ID_LABEL");
            OrdersList.Columns[1].HeaderText = RM.GetString("GENERAL_TOTAL_LABEL");
            OrdersList.Columns[3].HeaderText = RM.GetString("GENERAL_CREATED_LABEL");

            PurchaseOrder[] orders = OrderContext.Current.GetPurchaseOrders(ProfileContext.Current.UserId);

			if (orders != null)
			{
				// convert created time for every order from UTC to server time
				foreach (PurchaseOrder order in orders)
					order.Created = CommonHelper.GetUserDateTime(order.Created);

				// bind orders table
				OrdersList.DataSource = orders;
				OrdersList.DataBind();
			}
        }
    }
}
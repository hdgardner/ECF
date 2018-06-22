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

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// Checkout Confirmation page. This is the last page of the checkout wizard. It displays all the totals
    /// and provides a button to place order. It will also submit order to the data service for processing.
    /// </summary>
    public partial class CheckoutConfirmModule : BaseStoreUserControl
    {
        #region Private Variables
        //private Cart _Cart = null;
        #endregion

        private CartHelper _CartHelper = null;
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            //BindData();
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
        private void BindData()
        {
            //if (!this.Visible)
            //    return;

            DataBind();
        }
    }
}
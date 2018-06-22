namespace Mediachase.Commerce.Base.Controls.PaymentGatways.Generic
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using Mediachase.Cms.WebUtility;
    using Mediachase.Cms.WebUtility.Commerce;
    using Mediachase.Commerce.Orders;

	/// <summary>
	///	Implements User interface for generic gateway
	/// </summary>
	public partial class PaymentMethod : BaseStoreUserControl, IPaymentOption
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void Page_Load(object sender, System.EventArgs e)
		{
		}

        #region IPaymentOption Members

        /// <summary>
        /// Validates input data for the control. In case of Credit card pre authentication will be the best way to
        /// validate. The error message if any should be displayed within a control itself.
        /// </summary>
        /// <returns>Returns false if validation is failed.</returns>
        public bool ValidateData()
        {
            this.Page.Validate(this.ID);
            return this.Page.IsValid;
        }

        /// <summary>
        /// This method is called before the order is completed. This method should check all the parameters
        /// and validate the credit card or other parameters accepted.
        /// </summary>
        /// <param name="form"></param>
        /// <returns>bool</returns>
        public Payment PreProcess(OrderForm form)
        {
            OtherPayment otherPayment = new OtherPayment();
            return (Payment)otherPayment;
        }

        /// <summary>
        /// This method is called after the order is placed. This method should be used by the gateways that want to
        /// redirect customer to their site.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <returns></returns>
        public bool PostProcess(OrderForm form)
        {
            return true;
        }

        #endregion
    }
}

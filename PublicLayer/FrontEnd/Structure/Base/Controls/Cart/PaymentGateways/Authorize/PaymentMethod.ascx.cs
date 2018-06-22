namespace Mediachase.eCF.PublicStore.Plugins.PaymentGateways.Authorize
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
	///	Implements User interface for Authorize.NET gateway
	/// </summary>
	public partial class PaymentMethod : BaseStoreUserControl, IPaymentOption
	{
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
            CreditCardPayment cardPayment = new CreditCardPayment();
            //if (form.Payments.Count == 0)
            {
                DateTime expiratonDate = new DateTime(int.Parse(creditCardExpireYear.SelectedValue == null ? "0" : creditCardExpireYear.SelectedValue),
                    int.Parse(creditCardExpireMonth.SelectedValue == null ? "0" : creditCardExpireMonth.SelectedValue), 1);

                cardPayment.BillingAddressId = form.BillingAddressId;
                cardPayment.CustomerName = creditCardName.Text;
                cardPayment.CreditCardNumber = creditCardNumber.Text;
                cardPayment.ExpirationMonth = expiratonDate.Month;
                cardPayment.ExpirationYear = expiratonDate.Year;
                cardPayment.CreditCardSecurityCode = creditCardCSC.Text;
            }

            /*
            CreditCardPayment ccPayment = 
            info.CreditCardName = creditCardName.Text;
            info.CreditCardNumber = creditCardNumber.Text;
            info.CreditCardExpired = new DateTime(int.Parse(creditCardExpireYear.SelectedValue == null ? "0" : creditCardExpireYear.SelectedValue),
                int.Parse(creditCardExpireMonth.SelectedValue == null ? "0" : creditCardExpireMonth.SelectedValue), 1).ToString("MMyyyy");
            info.CreditCardCSC = creditCardCSC.Text;
             * */
            return (Payment)cardPayment;
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

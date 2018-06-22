namespace Mediachase.Commerce.Plugins.Payment
{
    using System;
    using System.Data;
    using System.Web;
    using System.Configuration;
    using System.Text;
    using System.Resources;
    using Mediachase.Commerce.Orders;
    using System.Collections.Generic;

    /// <summary>
    /// Generic Payment Gateway should be used in the place of gateways that do no provide real time credit card 
    /// charges. For instance pay by phone, pay by fax, invoice or gateways that redirect to the gateway website,
    /// like PayPal.
    /// </summary>
    public class GenericPaymentGateway : AbstractPaymentGateway
    {
        #region IPaymentGateway Members
        /// <summary>
        /// Charges the order, no recurrence pattern is checked.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">the message passed back, most often an error if transaction failed</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// Implemented by IChargePaymentGateway and ECheckPaymentGateway
        /// </remarks>
        public override bool ProcessPayment(Payment payment, ref string message)
        {
            return true;
        }
        #endregion
    }
}
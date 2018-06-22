using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Orders;
using System.Collections;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Payment Gateway Interface. Every payment option should implement this interface.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Returns the configuration data associated with a gateway.
        /// Sets the configuration gateway data. This data typically includes
        /// information like gateway URL, account info and so on.
        /// </summary>
        /// <value>The settings.</value>
        /// <returns></returns>
        IDictionary<string, string> Settings { get; set;}

        /// <summary>
        /// Processes the payment. Can be used for both positive and negative transactions.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool ProcessPayment(Payment payment, ref string message);
    }
}

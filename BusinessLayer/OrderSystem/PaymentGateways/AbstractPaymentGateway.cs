using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Plugins.Payment
{
    public abstract class AbstractPaymentGateway : IPaymentGateway
    {
        #region IPaymentGateway Members
        IDictionary<string, string> _ConfigData;

        /// <summary>
        /// Returns the configuration data associated with a gateway.
        /// Sets the configuration gateway data. This data typically includes
        /// information like gateway URL, account info and so on.
        /// </summary>
        /// <value>The settings.</value>
        /// <returns></returns>
        public virtual IDictionary<string, string> Settings
        {
            get
            {
                return _ConfigData;
            }
            set
            {
                _ConfigData = value; 
            }
        }

        /// <summary>
        /// Processes the payment. Can be used for both positive and negative transactions.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public abstract bool ProcessPayment(Mediachase.Commerce.Orders.Payment payment, ref string message);
        #endregion
    }
}

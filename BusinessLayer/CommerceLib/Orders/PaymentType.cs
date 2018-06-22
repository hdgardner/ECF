using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Defines the general categories of payment types supported by the Order System
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Typical credit card payment.
        /// </summary>
        CreditCard,
        /// <summary>
        /// Cash card is a company issued card for making purchases.
        /// </summary>
        CashCard,
        /// <summary>
        /// Typically business to business payment type.
        /// </summary>
        Invoice, 
        /// <summary>
        /// Gift card issued by the store.
        /// </summary>
        GiftCard, 
        /// <summary>
        /// Custom implementation.
        /// </summary>
        Other
    }
}

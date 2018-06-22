using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Credit Card type of payment.
    /// </summary>
    [Serializable]
    public class CreditCardPayment : Payment
    {
        private static MetaClass _MetaClass;
        /// <summary>
        /// Gets the credit card payment meta class.
        /// </summary>
        /// <value>The credit card payment meta class.</value>
        public static MetaClass CreditCardPaymentMetaClass
        {
            get
            {
                if(_MetaClass == null)
                {
                    _MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "CreditCardPayment");
                }

                return _MetaClass;
            }
        }

        /// <summary>
        /// Gets or sets the type of the card. Types typically are VISA, MasterCard, AMEX.
        /// </summary>
        /// <value>The type of the card.</value>
        public string CardType
        {
            get
            {
                return GetString("CardType");
            }
            set
            {
                this["CardType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the credit card number. The field is not encrypted by default. Encryption should be handled by 
        /// the layer calling the property.
        /// </summary>
        /// <value>The credit card number.</value>
        public string CreditCardNumber
        {
            get
            {
                return GetString("CreditCardNumber");
            }
            set
            {
                this["CreditCardNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the credit card security code.
        /// </summary>
        /// <value>The credit card security code.</value>
        public string CreditCardSecurityCode
        {
            get
            {
                return GetString("CreditCardSecurityCode");
            }
            set
            {
                this["CreditCardSecurityCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration month. Goes from 1 to 12.
        /// </summary>
        /// <value>The expiration month.</value>
        public int ExpirationMonth
        {
            get
            {
                return GetInt32("ExpirationMonth");
            }
            set
            {
                if (value < 1 || value > 12)
                    throw new ArgumentOutOfRangeException("ExpirationMonth", "ExpirationMonth should be between 1 and 12");

                this["ExpirationMonth"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the expiration year.
        /// </summary>
        /// <value>The expiration year.</value>
        public int ExpirationYear
        {
            get
            {
                return GetInt32("ExpirationYear");
            }
            set
            {
                this["ExpirationYear"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardPayment"/> class.
        /// </summary>
        public CreditCardPayment() : base(CreditCardPaymentMetaClass)
        {
            this.PaymentType = PaymentType.CreditCard;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardPayment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected CreditCardPayment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            this.PaymentType = PaymentType.CreditCard;
        }
    }
}
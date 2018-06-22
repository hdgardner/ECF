using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Gift Card type of payment.
    /// </summary>
    [Serializable]
    public class GiftCardPayment : Payment
    {
        private static MetaClass _MetaClass;

        /// <summary>
        /// Gets the gift card payment meta class.
        /// </summary>
        /// <value>The gift card payment meta class.</value>
        public static MetaClass GiftCardPaymentMetaClass
        {
            get
            {
                if(_MetaClass == null)
                {
                    _MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "GiftCardPayment");
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
        /// Gets or sets the gift card number.
        /// </summary>
        /// <value>The gift card number.</value>
        public string GiftCardNumber
        {
            get
            {
                return GetString("GiftCardNumber");
            }
            set
            {
                this["GiftCardNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the gift card security code.
        /// </summary>
        /// <value>The gift card security code.</value>
        public string GiftCardSecurityCode
        {
            get
            {
                return GetString("GiftCardSecurityCode");
            }
            set
            {
                this["GiftCardSecurityCode"] = value;
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
        /// Initializes a new instance of the <see cref="GiftCardPayment"/> class.
        /// </summary>
        public GiftCardPayment() : base(GiftCardPaymentMetaClass) 
        {
            this.PaymentType = PaymentType.GiftCard;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GiftCardPayment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected GiftCardPayment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            this.PaymentType = PaymentType.GiftCard;
        }
    }
}
